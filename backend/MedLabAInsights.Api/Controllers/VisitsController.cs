using MedLabAInsights.Api.Contracts.Visits;
using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Api.Controllers;

[ApiController]
[Route("api/members/{memberId:int}/visits")]
public class VisitsController : ControllerBase
{
    private readonly MedlabAinsightDbContext _db;

    public VisitsController(MedlabAinsightDbContext db)
    {
        _db = db;
    }

    // POST: api/members/{memberId}/visits
    [HttpPost]
    public async Task<ActionResult<CreateVisitResponse>> CreateVisit(
        int memberId,
        [FromBody] CreateVisitRequest request,
        CancellationToken ct)
    {
        // 1) Validate member exists
        var memberExists = await _db.Members
            .AsNoTracking()
            .AnyAsync(m => m.MemberId == memberId, ct);

        if (!memberExists)
            return NotFound(new { message = $"Member {memberId} not found" });

        // 2) Validate panel exists
        var panelExists = await _db.Panels
            .AsNoTracking()
            .AnyAsync(p => p.PanelId == request.PanelId, ct);

        if (!panelExists)
            return BadRequest(new { message = $"Invalid PanelId {request.PanelId}" });

        // 3) Validate testIds exist (including extra tests)
        var distinctTestIds = request.Results
            .Select(r => r.TestId)
            .Distinct()
            .ToList();

        if (distinctTestIds.Count == 0)
            return BadRequest(new { message = "At least one test result is required." });

        var existingTestIds = await _db.Tests
            .AsNoTracking()
            .Where(t => distinctTestIds.Contains(t.TestId))
            .Select(t => t.TestId)
            .ToListAsync(ct);

        var missingTestIds = distinctTestIds.Except(existingTestIds).ToList();
        if (missingTestIds.Count > 0)
            return BadRequest(new { message = "Some TestIds are invalid.", missingTestIds });

        // 4) Create visit + results in a transaction
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var visit = new Visit
        {
            MemberId = memberId,
            PanelId = request.PanelId,
            VisitDateTime = request.VisitDateTime ?? DateTime.UtcNow,

            Height = request.Height,
            Weight = request.Weight,
            Systolic = request.Systolic,
            Diastolic = request.Diastolic,
            Notes = request.Notes
        };

        _db.Visits.Add(visit);
        await _db.SaveChangesAsync(ct); // needed to get VisitId

        var now = DateTime.UtcNow;

        // If UI sends duplicates for same TestId, we keep the last one.
        var latestByTest = request.Results
            .GroupBy(r => r.TestId)
            .Select(g => g.Last())
            .ToList();

        foreach (var r in latestByTest)
        {
            _db.VisitTestResults.Add(new VisitTestResult
            {
                VisitId = visit.VisitId,
                TestId = r.TestId,
                RawValue = r.RawValue.Trim(),
                EnteredAt = now
            });
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Ok(new CreateVisitResponse { VisitId = visit.VisitId });
    }
}