using System.ComponentModel.DataAnnotations;

namespace MedLabAInsights.Api.Contracts.Visits;

public sealed class CreateVisitRequest
{
    [Required]
    public int PanelId { get; init; }

    public DateTime? VisitDateTime { get; init; } // optional. if null -> server uses now

    public int? Height { get; init; }
    public int? Weight { get; init; }
    public int? Systolic { get; init; }
    public int? Diastolic { get; init; }

    public string? Notes { get; init; }

    [Required]
    public List<CreateVisitTestResultItem> Results { get; init; } = new();
}

public sealed class CreateVisitTestResultItem
{
    [Required]
    public int TestId { get; init; }

    [Required, MaxLength(50)]
    public string RawValue { get; init; } = null!;
}