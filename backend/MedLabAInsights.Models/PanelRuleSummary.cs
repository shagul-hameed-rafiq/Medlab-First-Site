namespace MedLabAInsights.Models
{
    public class PanelRuleSummary
    {
        public int PanelRuleId { get; set; }

        public int PanelId { get; set; }
        public Panel Panel { get; set; } = null!;

        public string PanelRuleName { get; set; } = null!;
        public string PanelRuleCode { get; set; } = null!;

        public int MinSeverity { get; set; }
        public int MaxSeverity { get; set; }

        public string StandardSummary { get; set; } = null!;
    }
}
