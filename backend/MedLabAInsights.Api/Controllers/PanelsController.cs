using MedLabAInsights.Api.Contracts.Panels;
using MedLabAInsights.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Api.Controllers;

[ApiController]
[Route("api/panels")]
public class PanelsController : ControllerBase
{
    private readonly MedlabAinsightDbContext _db;

    public PanelsController(MedlabAinsightDbContext db)
    {
        _db = db;
    }

    // GET: api/panels
    // For "Choose Panel" cards (includes test count)
    [HttpGet]
    public async Task<ActionResult<List<PanelListItemDto>>> GetPanels(CancellationToken ct)
    {
        var panels = await _db.Panels
            .AsNoTracking()
            .OrderBy(p => p.PanelName)
            .Select(p => new PanelListItemDto
            {
                PanelId = p.PanelId,
                PanelName = p.PanelName,
                PanelCode = p.PanelCode,
                TestCount = _db.PanelTestMappings.Count(m => m.PanelId == p.PanelId)
            })
            .ToListAsync(ct);

        return Ok(panels);
    }

    // GET: api/panels/{panelId}/tests
    // For "Enter Results" default tests for selected panel
    [HttpGet("{panelId:int}/tests")]
    public async Task<ActionResult<List<PanelTestDto>>> GetPanelTests(int panelId, CancellationToken ct)
    {
        var panelExists = await _db.Panels
            .AsNoTracking()
            .AnyAsync(p => p.PanelId == panelId, ct);

        if (!panelExists)
            return NotFound(new { message = $"Panel {panelId} not found" });

        // Join to avoid requiring navigation properties in mapping entity
        var tests = await (
            from m in _db.PanelTestMappings.AsNoTracking()
            join t in _db.Tests.AsNoTracking() on m.TestId equals t.TestId
            where m.PanelId == panelId
            orderby m.ImportanceLevel, t.TestName
            select new PanelTestDto
            {
                TestId = t.TestId,
                TestName = t.TestName,
                TestCode = t.TestCode,
                MinValue = t.MinValue,
                MaxValue = t.MaxValue,
                Unit = t.Unit,
                ImportanceLevel = m.ImportanceLevel
            }
        ).ToListAsync(ct);

        return Ok(tests);
    }
}