using MedLabAInsights.Api.Contracts.Finalize;
using MedLabAInsights.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Api.Controllers;

[ApiController]
[Route("api/visits")]
public sealed class FinalizeController : ControllerBase
{
    private readonly MedlabAinsightDbContext _db;

    public FinalizeController(MedlabAinsightDbContext db)
    {
        _db = db;
    }

    // POST: api/visits/{visitId}/finalize
    [HttpPost("{visitId:int}/finalize")]
    public async Task<ActionResult<FinalizeVisitReportResponse>> Finalize(int visitId, [FromBody] FinalizeVisitReportRequest req, CancellationToken ct)
    {
        // 0) Visit exists?
        var visitExists = await _db.Visits.AsNoTracking().AnyAsync(v => v.VisitId == visitId, ct);
        if (!visitExists)
            return NotFound(new { message = $"Visit {visitId} not found" });

        // 1) Ensure evaluation exists (panel summary + interpretations)
        var panelSummary = await _db.VisitPanelSummaries
            .FirstOrDefaultAsync(x => x.VisitId == visitId, ct);

        if (panelSummary is null)
            return BadRequest(new { message = $"Panel summary not found for Visit {visitId}. Run evaluate first." });

        // Load interpretations keyed by TestId for this visit
        var interpretations = await (
            from i in _db.VisitTestInterpretations
            join r in _db.VisitTestResults on i.VisitTestResultId equals r.VisitTestResultId
            where r.VisitId == visitId
            select new
            {
                TestId = r.TestId,
                Interpretation = i
            }
        ).ToListAsync(ct);

        if (interpretations.Count == 0)
            return BadRequest(new { message = $"Test interpretations not found for Visit {visitId}. Run evaluate first." });

        var interpByTestId = interpretations
            .GroupBy(x => x.TestId)
            .ToDictionary(g => g.Key, g => g.First().Interpretation);

        // 2) Validate requested testIds exist in this visit
        var requestedIds = req.TestRevisions.Select(x => x.TestId).Distinct().ToList();
        var missing = requestedIds.Where(id => !interpByTestId.ContainsKey(id)).ToList();

        if (missing.Count > 0)
            return BadRequest(new
            {
                message = "Some TestIds are not part of this visit / not evaluated.",
                missingTestIds = missing
            });

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        // 3) Update panel revised summary
        var panelText = req.PanelRevisedSummary?.Trim();
        if (string.IsNullOrWhiteSpace(panelText))
        {
            panelSummary.RevisedSummary = null;
            panelSummary.IsRevised = false;
        }
        else
        {
            panelSummary.RevisedSummary = panelText;
            panelSummary.IsRevised = true;
        }

        // 4) Update test revised reports
        var updatedTests = 0;

        foreach (var item in req.TestRevisions)
        {
            var entity = interpByTestId[item.TestId];

            var revised = item.RevisedReport?.Trim();

            if (string.IsNullOrWhiteSpace(revised))
            {
                if (entity.IsRevised || entity.RevisedReport != null)
                {
                    entity.RevisedReport = null;
                    entity.IsRevised = false;
                    updatedTests++;
                }
            }
            else
            {
                // update only if changed (optional)
                if (!entity.IsRevised || entity.RevisedReport != revised)
                {
                    entity.RevisedReport = revised;
                    entity.IsRevised = true;
                    updatedTests++;
                }
            }
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Ok(new FinalizeVisitReportResponse
        {
            VisitId = visitId,
            UpdatedTests = updatedTests,
            PanelUpdated = true
        });
    }
}