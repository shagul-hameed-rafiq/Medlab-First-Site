namespace MedLabAInsights.Models
{
    public class VisitPanelSummary
    {
        public int VisitPanelSummaryId { get; set; }

        public int VisitId { get; set; }
        public Visit Visit { get; set; } = null!;

        public int PanelRuleId { get; set; }
        public PanelRuleSummary PanelRuleSummary { get; set; } = null!;

        public string StandardSummarySnapshot { get; set; } = null!;

        public string? RevisedSummary { get; set; }
        public bool IsRevised { get; set; }

        public DateTime EvaluatedAt { get; set; }
    }
}
