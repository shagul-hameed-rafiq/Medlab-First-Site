namespace MedLabAInsights.Api.Contracts.Evaluation;

public sealed class EvaluateVisitResponse
{
    public int VisitId { get; init; }
    public int PanelId { get; init; }
    public int MaxSeverity { get; init; }

    public PanelSummaryDto PanelSummary { get; init; } = null!;
    public List<TestInterpretationDto> Tests { get; init; } = new();
}

public sealed class TestInterpretationDto
{
    public int TestId { get; init; }
    public string TestName { get; init; } = null!;
    public string RawValue { get; init; } = null!;

    public int BandId { get; init; }
    public string BandName { get; init; } = null!;
    public int Severity { get; init; }
    public string StandardReport { get; init; } = null!;
}

public sealed class PanelSummaryDto
{
    public int PanelRuleId { get; init; }
    public string PanelRuleName { get; init; } = null!;
    public int MinSeverity { get; init; }
    public int MaxSeverity { get; init; }
    public string StandardSummary { get; init; } = null!;
}