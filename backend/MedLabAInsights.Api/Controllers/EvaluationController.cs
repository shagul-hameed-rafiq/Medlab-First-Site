using MedLabAInsights.Api.Contracts.Evaluation;
using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MedLabAInsights.Api.Controllers;

[ApiController]
[Route("api/visits")]
public sealed class EvaluationController : ControllerBase
{
    private readonly MedlabAinsightDbContext _db;

    public EvaluationController(MedlabAinsightDbContext db)
    {
        _db = db;
    }

    // POST: api/visits/{visitId}/evaluate
    [HttpPost("{visitId:int}/evaluate")]
    public async Task<ActionResult<EvaluateVisitResponse>> EvaluateVisit(int visitId, CancellationToken ct)
    {
        // 1) Load visit
        var visit = await _db.Visits
            .AsNoTracking()
            .Where(v => v.VisitId == visitId)
            .Select(v => new { v.VisitId, v.PanelId })
            .FirstOrDefaultAsync(ct);

        if (visit is null)
            return NotFound(new { message = $"Visit {visitId} not found" });

        // 2) Load raw results + test names
        var rawResults = await (
            from r in _db.VisitTestResults.AsNoTracking()
            join t in _db.Tests.AsNoTracking() on r.TestId equals t.TestId
            where r.VisitId == visitId
            select new
            {
                r.VisitTestResultId,
                r.TestId,
                TestName = t.TestName,
                r.RawValue
            }
        ).ToListAsync(ct);

        if (rawResults.Count == 0)
            return BadRequest(new { message = "No test results found for this visit." });

        // 3) Evaluate each test -> find BandRuleReport
        var now = DateTime.UtcNow;

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        // Optional: remove previous evaluations (so re-evaluate is clean)
        // If you prefer “only evaluate once”, tell me and we’ll block if already exists.
        var existingInterps = await _db.VisitTestInterpretations
            .Where(x => rawResults.Select(r => r.VisitTestResultId).Contains(x.VisitTestResultId))
            .ToListAsync(ct);

        if (existingInterps.Count > 0)
            _db.VisitTestInterpretations.RemoveRange(existingInterps);

        var existingPanelSummaries = await _db.VisitPanelSummaries
            .Where(x => x.VisitId == visitId)
            .ToListAsync(ct);

        if (existingPanelSummaries.Count > 0)
            _db.VisitPanelSummaries.RemoveRange(existingPanelSummaries);

        await _db.SaveChangesAsync(ct);

        var testInterpretationsForResponse = new List<TestInterpretationDto>();
        var severities = new List<int>();

        foreach (var r in rawResults)
        {
            // Parse raw value to double
            if (!TryParseDouble(r.RawValue, out var value))
            {
                // If parsing fails, we treat as “cannot interpret”.
                // For now: return error. Later we can store a special band / message.
                await tx.RollbackAsync(ct);
                return BadRequest(new { message = $"Invalid numeric value for TestId {r.TestId}: '{r.RawValue}'" });
            }

            // Find matching band (RangeMin <= value <= RangeMax)
            // Use OrderBy RangeMin to make deterministic if overlaps
            var band = await _db.BandRuleReports
                .AsNoTracking()
                .Where(b => b.TestId == r.TestId && b.RangeMin <= value && value <= b.RangeMax)
                .OrderBy(b => b.RangeMin)
                .Select(b => new
                {
                    b.BandId,
                    b.BandName,
                    b.Severity,
                    b.StandardReport
                })
                .FirstOrDefaultAsync(ct);

            if (band is null)
            {
                await tx.RollbackAsync(ct);
                return BadRequest(new
                {
                    message = $"No band rule found for TestId {r.TestId} value {value}. Check band ranges."
                });
            }

            severities.Add(band.Severity);

            // Save VisitTestInterpretation
            _db.VisitTestInterpretations.Add(new VisitTestInterpretation
            {
                VisitTestResultId = r.VisitTestResultId,
                BandId = band.BandId,
                StandardReportSnapshot = band.StandardReport,
                RevisedReport = null,
                IsRevised = false,
                EvaluatedAt = now
            });

            testInterpretationsForResponse.Add(new TestInterpretationDto
            {
                TestId = r.TestId,
                TestName = r.TestName,
                RawValue = r.RawValue,
                BandId = band.BandId,
                BandName = band.BandName,
                Severity = band.Severity,
                StandardReport = band.StandardReport
            });
        }

        // 4) Panel summary based on max severity
        var maxSeverity = severities.Max();

        var panelRule = await _db.PanelRuleSummaries
            .AsNoTracking()
            .Where(p => p.PanelId == visit.PanelId &&
                        p.MinSeverity <= maxSeverity &&
                        maxSeverity <= p.MaxSeverity)
            .OrderBy(p => p.MinSeverity)
            .Select(p => new
            {
                p.PanelRuleId,
                p.PanelRuleName,
                p.MinSeverity,
                p.MaxSeverity,
                p.StandardSummary
            })
            .FirstOrDefaultAsync(ct);

        if (panelRule is null)
        {
            await tx.RollbackAsync(ct);
            return BadRequest(new
            {
                message = $"No panel summary rule found for PanelId {visit.PanelId} with maxSeverity={maxSeverity}."
            });
        }

        // Save VisitPanelSummary
        _db.VisitPanelSummaries.Add(new VisitPanelSummary
        {
            VisitId = visitId,
            PanelRuleId = panelRule.PanelRuleId,
            StandardSummarySnapshot = panelRule.StandardSummary,
            RevisedSummary = null,
            IsRevised = false,
            EvaluatedAt = now
        });

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        // 5) Return response
        return Ok(new EvaluateVisitResponse
        {
            VisitId = visitId,
            PanelId = visit.PanelId,
            MaxSeverity = maxSeverity,
            PanelSummary = new PanelSummaryDto
            {
                PanelRuleId = panelRule.PanelRuleId,
                PanelRuleName = panelRule.PanelRuleName,
                MinSeverity = panelRule.MinSeverity,
                MaxSeverity = panelRule.MaxSeverity,
                StandardSummary = panelRule.StandardSummary
            },
            Tests = testInterpretationsForResponse
                .OrderByDescending(x => x.Severity)
                .ThenBy(x => x.TestName)
                .ToList()
        });
    }

    private static bool TryParseDouble(string raw, out double value)
    {
        raw = raw.Trim();

        // 1) Invariant culture (dot decimals)
        if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            return true;

        // 2) Current culture (for comma decimals if user enters)
        if (double.TryParse(raw, NumberStyles.Float, CultureInfo.CurrentCulture, out value))
            return true;

        value = 0;
        return false;
    }
}