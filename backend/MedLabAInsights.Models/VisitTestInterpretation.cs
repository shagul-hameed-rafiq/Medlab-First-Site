namespace MedLabAInsights.Models
{
    public class VisitTestInterpretation
    {
        public int VisitTestInterpretationId { get; set; }

        public int VisitTestResultId { get; set; }
        public VisitTestResult VisitTestResult { get; set; } = null!;

        public int BandId { get; set; }
        public BandRuleReport BandRuleReport { get; set; } = null!;

        public string StandardReportSnapshot { get; set; } = null!;

        public string? RevisedReport { get; set; }
        public bool IsRevised { get; set; }

        public DateTime EvaluatedAt { get; set; }
    }
}
