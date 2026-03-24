using MedLabAInsights.Api.Contracts.Reports;
using MedLabAInsights.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Api.Controllers;

[ApiController]
[Route("api/visits")]
public sealed class ReportsController : ControllerBase
{
    private readonly MedlabAinsightDbContext _db;

    public ReportsController(MedlabAinsightDbContext db)
    {
        _db = db;
    }

    // GET: api/visits/{visitId}/report
    [HttpGet("{visitId:int}/report")]
    public async Task<ActionResult<VisitReportDto>> GetVisitReport(int visitId, CancellationToken ct)
    {
        // 1) Header: Visit + Member + Panel + vitals
        var header = await (
            from v in _db.Visits.AsNoTracking()
            join m in _db.Members.AsNoTracking() on v.MemberId equals m.MemberId
            join p in _db.Panels.AsNoTracking() on v.PanelId equals p.PanelId
            where v.VisitId == visitId
            select new
            {
                v.VisitId,
                v.VisitDateTime,

                Member = new
                {
                    m.MemberId,
                    m.Name,
                    Gender = m.Gender.ToString(),
                    m.DateOfBirth,
                    BloodGroup = m.BloodGroup.ToString(),
                    m.Contact,
                    m.Address
                },

                Panel = new
                {
                    p.PanelId,
                    p.PanelName,
                    p.PanelCode
                },

                Vitals = new
                {
                    v.Height,
                    v.Weight,
                    v.Systolic,
                    v.Diastolic
                }
            }
        ).FirstOrDefaultAsync(ct);

        if (header is null)
            return NotFound(new { message = $"Visit {visitId} not found" });

        // 2) Panel summary (must exist for "Submitted")
        var panelSummary = await (
            from vps in _db.VisitPanelSummaries.AsNoTracking()
            join prs in _db.PanelRuleSummaries.AsNoTracking() on vps.PanelRuleId equals prs.PanelRuleId
            where vps.VisitId == visitId
            select new
            {
                vps.VisitPanelSummaryId,
                prs.PanelRuleId,
                prs.PanelRuleName,
                prs.MinSeverity,
                prs.MaxSeverity,
                StandardSummary = vps.StandardSummarySnapshot,
                vps.RevisedSummary,
                vps.IsRevised
            }
        ).FirstOrDefaultAsync(ct);

        var status = panelSummary is null ? "Draft" : "Submitted";

        // If Draft, you can still return partial response (no panel summary/tests interpretations)
        if (panelSummary is null)
        {
            return Ok(new VisitReportDto
            {
                VisitId = header.VisitId,
                VisitDateTime = header.VisitDateTime,
                Status = status,
                Member = new MemberHeaderDto
                {
                    MemberId = header.Member.MemberId,
                    Name = header.Member.Name,
                    Gender = header.Member.Gender,
                    Age = CalculateAge(header.Member.DateOfBirth, header.VisitDateTime),
                    BloodGroup = header.Member.BloodGroup,
                    Contact = header.Member.Contact,
                    Address = header.Member.Address
                },
                Panel = new PanelHeaderDto
                {
                    PanelId = header.Panel.PanelId,
                    PanelName = header.Panel.PanelName,
                    PanelCode = header.Panel.PanelCode
                },
                Vitals = new VitalsDto
                {
                    Height = header.Vitals.Height,
                    Weight = header.Vitals.Weight,
                    Systolic = header.Vitals.Systolic,
                    Diastolic = header.Vitals.Diastolic
                },
                PanelSummary = new PanelSummaryReportDto
                {
                    PanelRuleId = 0,
                    PanelRuleName = "",
                    MinSeverity = 0,
                    MaxSeverity = 0,
                    StandardSummary = "",
                    RevisedSummary = null,
                    IsRevised = false,
                    EffectiveSummary = ""
                },
                Tests = new List<TestReportDto>()
            });
        }

        // 3) Test reports: raw + interpretation + band + test master
        var tests = await (
            from r in _db.VisitTestResults.AsNoTracking()
            join t in _db.Tests.AsNoTracking() on r.TestId equals t.TestId
            join i in _db.VisitTestInterpretations.AsNoTracking() on r.VisitTestResultId equals i.VisitTestResultId
            join b in _db.BandRuleReports.AsNoTracking() on i.BandId equals b.BandId
            where r.VisitId == visitId
            select new TestReportDto
            {
                TestId = t.TestId,
                TestName = t.TestName,
                TestCode = t.TestCode,
                Unit = t.Unit,
                MinValue = t.MinValue,
                MaxValue = t.MaxValue,

                RawValue = r.RawValue,

                BandId = b.BandId,
                BandName = b.BandName,
                Severity = b.Severity,

                StandardReport = i.StandardReportSnapshot,
                RevisedReport = i.RevisedReport,
                IsRevised = i.IsRevised,

                EffectiveReport = i.IsRevised && i.RevisedReport != null && i.RevisedReport != ""
                    ? i.RevisedReport
                    : i.StandardReportSnapshot
            }
        ).ToListAsync(ct);

        // Order: worst first, then name
        tests = tests
            .OrderByDescending(x => x.Severity)
            .ThenBy(x => x.TestName)
            .ToList();

        // 4) Compose response
        var response = new VisitReportDto
        {
            VisitId = header.VisitId,
            VisitDateTime = header.VisitDateTime,
            Status = status,

            Member = new MemberHeaderDto
            {
                MemberId = header.Member.MemberId,
                Name = header.Member.Name,
                Gender = header.Member.Gender,
                Age = CalculateAge(header.Member.DateOfBirth, header.VisitDateTime),
                BloodGroup = header.Member.BloodGroup,
                Contact = header.Member.Contact,
                Address = header.Member.Address
            },

            Panel = new PanelHeaderDto
            {
                PanelId = header.Panel.PanelId,
                PanelName = header.Panel.PanelName,
                PanelCode = header.Panel.PanelCode
            },

            Vitals = new VitalsDto
            {
                Height = header.Vitals.Height,
                Weight = header.Vitals.Weight,
                Systolic = header.Vitals.Systolic,
                Diastolic = header.Vitals.Diastolic
            },

            PanelSummary = new PanelSummaryReportDto
            {
                PanelRuleId = panelSummary.PanelRuleId,
                PanelRuleName = panelSummary.PanelRuleName,
                MinSeverity = panelSummary.MinSeverity,
                MaxSeverity = panelSummary.MaxSeverity,
                StandardSummary = panelSummary.StandardSummary,
                RevisedSummary = panelSummary.RevisedSummary,
                IsRevised = panelSummary.IsRevised,
                EffectiveSummary = panelSummary.IsRevised && !string.IsNullOrWhiteSpace(panelSummary.RevisedSummary)
                    ? panelSummary.RevisedSummary!
                    : panelSummary.StandardSummary
            },

            Tests = tests
        };

        return Ok(response);
    }

    private static int CalculateAge(DateTime dob, DateTime onDate)
    {
        var age = onDate.Year - dob.Year;
        if (dob.Date > onDate.Date.AddYears(-age)) age--;
        return age < 0 ? 0 : age;
    }
}