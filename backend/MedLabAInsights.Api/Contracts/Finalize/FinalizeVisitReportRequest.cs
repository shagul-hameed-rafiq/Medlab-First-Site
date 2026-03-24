using System.ComponentModel.DataAnnotations;

namespace MedLabAInsights.Api.Contracts.Finalize;

public sealed class FinalizeVisitReportRequest
{
    // Optional. If null/empty => clear panel revised summary
    [MaxLength(10000)]
    public string? PanelRevisedSummary { get; init; }

    [Required]
    public List<TestRevisionItem> TestRevisions { get; init; } = new();
}

public sealed class TestRevisionItem
{
    [Required]
    public int TestId { get; init; }

    // Optional. If null/empty => clear revised report for this test
    [MaxLength(5000)]
    public string? RevisedReport { get; init; }
}