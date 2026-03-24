namespace MedLabAInsights.Api.Contracts.Members;

public sealed class MemberListItemDto
{
    public int MemberId { get; init; }
    public string Name { get; init; } = null!;
    public string Gender { get; init; } = null!;
    public int Age { get; init; }
    public long Contact { get; init; }
}
