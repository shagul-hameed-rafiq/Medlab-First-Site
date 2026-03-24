namespace MedLabAInsights.Api.Contracts.Panels;

public sealed class PanelListItemDto
{
    public int PanelId { get; init; }
    public string PanelName { get; init; } = null!;
    public string PanelCode { get; init; } = null!;
    public int TestCount { get; init; }
}