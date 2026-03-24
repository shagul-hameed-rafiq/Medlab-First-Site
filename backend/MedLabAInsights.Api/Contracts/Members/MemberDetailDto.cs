namespace MedLabAInsights.Api.Contracts.Members;

public sealed class MemberDetailDto
{
    public int MemberId { get; init; }
    public string Name { get; init; } = null!;
    public string Gender { get; init; } = null!;
    public DateTime DateOfBirth { get; init; }
    public int Age { get; init; }
    public string BloodGroup { get; init; } = null!;
    public long Contact { get; init; }
    public string? Address { get; init; }
}
