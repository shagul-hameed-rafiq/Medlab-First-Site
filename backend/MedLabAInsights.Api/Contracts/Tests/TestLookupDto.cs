namespace MedLabAInsights.Api.Contracts.Tests;

public sealed class TestLookupDto
{
    public int TestId { get; init; }
    public string TestName { get; init; } = null!;
    public string TestCode { get; init; } = null!;
    public string? Unit { get; init; }

    public double? MinValue { get; init; }
    public double? MaxValue { get; init; }
}