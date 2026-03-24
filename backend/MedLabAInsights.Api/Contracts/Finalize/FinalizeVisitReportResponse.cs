namespace MedLabAInsights.Api.Contracts.Finalize;

public sealed class FinalizeVisitReportResponse
{
    public int VisitId { get; init; }
    public int UpdatedTests { get; init; }
    public bool PanelUpdated { get; init; }
}