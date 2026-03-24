using MedLabAInsights.Api.Contracts.Tests;
using MedLabAInsights.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Api.Controllers;

[ApiController]
[Route("api/tests")]
public class TestsController : ControllerBase
{
    private readonly MedlabAinsightDbContext _db;

    public TestsController(MedlabAinsightDbContext db)
    {
        _db = db;
    }

    // GET: api/tests?search=hb&skip=0&take=50
    // For "Add Extra Tests" dropdown (searchable + paged)
    [HttpGet]
    public async Task<ActionResult<List<TestLookupDto>>> GetTests(
        [FromQuery] string? search,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken ct = default)
    {
        skip = Math.Max(0, skip);
        take = Math.Clamp(take, 1, 200);

        var query = _db.Tests.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(t =>
                t.TestName.Contains(s) ||
                t.TestCode.Contains(s));
        }

        var tests = await query
            .OrderBy(t => t.TestName)
            .Skip(skip)
            .Take(take)
            .Select(t => new TestLookupDto
            {
                TestId = t.TestId,
                TestName = t.TestName,
                TestCode = t.TestCode,
                Unit = t.Unit,
                MinValue = t.MinValue,
                MaxValue = t.MaxValue
            })
            .ToListAsync(ct);

        return Ok(tests);
    }
}