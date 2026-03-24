using MedLabAInsights.Api.Contracts.Members;
using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Api.Controllers;

[ApiController]
[Route("api/members")]
public class MembersController : ControllerBase
{
    private readonly MedlabAinsightDbContext _db;

    public MembersController(MedlabAinsightDbContext db)
    {
        _db = db;
    }

    // GET: api/members
    [HttpGet]
    public async Task<ActionResult<List<MemberListItemDto>>> GetMembers(CancellationToken ct)
    {
        var members = await _db.Members
            .AsNoTracking()
            .OrderByDescending(m => m.MemberId)
            .Select(m => new MemberListItemDto
            {
                MemberId = m.MemberId,
                Name = m.Name,
                Gender = m.Gender.ToString(),
                Contact = m.Contact,
                Age = CalculateAge(m.DateOfBirth)
            })
            .ToListAsync(ct);

        return Ok(members);
    }

    // GET: api/members/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MemberDetailDto>> GetMemberById(
        int id,
        CancellationToken ct)
    {
        var member = await _db.Members
            .AsNoTracking()
            .Where(m => m.MemberId == id)
            .Select(m => new MemberDetailDto
            {
                MemberId = m.MemberId,
                Name = m.Name,
                Gender = m.Gender.ToString(),
                DateOfBirth = m.DateOfBirth,
                Age = CalculateAge(m.DateOfBirth),
                BloodGroup = m.BloodGroup.ToString(),
                Contact = m.Contact,
                Address = m.Address
            })
            .FirstOrDefaultAsync(ct);

        if (member is null)
            return NotFound(new { message = $"Member {id} not found" });

        return Ok(member);
    }

    // POST: api/members
    [HttpPost]
    public async Task<ActionResult<MemberDetailDto>> CreateMember(
        [FromBody] CreateMemberRequest request,
        CancellationToken ct)
    {
        // Convert string → enum safely
        if (!Enum.TryParse<Gender>(request.Gender, true, out var gender))
            return BadRequest(new { message = "Invalid gender value" });

        if (!Enum.TryParse<BloodGroup>(request.BloodGroup, true, out var bloodGroup))
            return BadRequest(new { message = "Invalid blood group value" });

        var entity = new Member
        {
            Name = request.Name.Trim(),
            Gender = gender,
            DateOfBirth = request.DateOfBirth,
            BloodGroup = bloodGroup,
            Contact = request.Contact,
            Address = request.Address?.Trim()
        };

        _db.Members.Add(entity);
        await _db.SaveChangesAsync(ct);

        var response = new MemberDetailDto
        {
            MemberId = entity.MemberId,
            Name = entity.Name,
            Gender = entity.Gender.ToString(),
            DateOfBirth = entity.DateOfBirth,
            Age = CalculateAge(entity.DateOfBirth),
            BloodGroup = entity.BloodGroup.ToString(),
            Contact = entity.Contact,
            Address = entity.Address
        };

        return CreatedAtAction(nameof(GetMemberById), new { id = response.MemberId }, response); 
    }

    private static int CalculateAge(DateTime dob)
    {
        var today = DateTime.Today;
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age)) age--;
        return age;
    }
}
