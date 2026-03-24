namespace MedLabAInsights.Api.Contracts.Visits;

public sealed class VisitListItemDto
{
    public int VisitId { get; init; }
    public DateTime VisitDateTime { get; init; }

    public int PanelId { get; init; }
    public string PanelName { get; init; } = null!;

    public int? Height { get; init; }
    public int? Weight { get; init; }
    public int? Systolic { get; init; }
    public int? Diastolic { get; init; }

    // "Draft" or "Submitted"
    public string Status { get; init; } = null!;
}