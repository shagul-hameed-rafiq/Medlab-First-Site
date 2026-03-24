using MedLabAInsights.Api.Contracts.Visits;
using MedLabAInsights.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Api.Controllers;

[ApiController]
[Route("api/members/{memberId:int}/visits")]
public sealed class MemberVisitsController : ControllerBase
{
    private readonly MedlabAinsightDbContext _db;

    public MemberVisitsController(MedlabAinsightDbContext db)
    {
        _db = db;
    }

    // GET: api/members/{memberId}/visits
    [HttpGet]
    public async Task<ActionResult<List<VisitListItemDto>>> GetVisits(int memberId, CancellationToken ct)
    {
        var memberExists = await _db.Members
            .AsNoTracking()
            .AnyAsync(m => m.MemberId == memberId, ct);

        if (!memberExists)
            return NotFound(new { message = $"Member {memberId} not found" });

        var visits = await (
            from v in _db.Visits.AsNoTracking()
            join p in _db.Panels.AsNoTracking() on v.PanelId equals p.PanelId
            where v.MemberId == memberId
            orderby v.VisitDateTime descending
            select new VisitListItemDto
            {
                VisitId = v.VisitId,
                VisitDateTime = v.VisitDateTime,
                PanelId = p.PanelId,
                PanelName = p.PanelName,

                Height = v.Height,
                Weight = v.Weight,
                Systolic = v.Systolic,
                Diastolic = v.Diastolic,

                Status = _db.VisitPanelSummaries.Any(s => s.VisitId == v.VisitId)
                    ? "Submitted"
                    : "Draft"
            }
        ).ToListAsync(ct);

        return Ok(visits);
    }
}