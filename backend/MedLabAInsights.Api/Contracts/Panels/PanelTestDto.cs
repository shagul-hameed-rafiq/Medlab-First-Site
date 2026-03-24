namespace MedLabAInsights.Api.Contracts.Panels;

public sealed class PanelTestDto
{
    public int TestId { get; init; }
    public string TestName { get; init; } = null!;
    public string TestCode { get; init; } = null!;

    public double? MinValue { get; init; }
    public double? MaxValue { get; init; }

    public string? Unit { get; init; }
    public int ImportanceLevel { get; init; }
}