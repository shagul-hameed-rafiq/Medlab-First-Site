namespace MedLabAInsights.Models
{
    public class VisitTestResult
    {
        public int VisitTestResultId { get; set; }

        public int VisitId { get; set; }
        public Visit Visit { get; set; } = null!;

        public int TestId { get; set; }
        public Test Test { get; set; } = null!;

        public string RawValue { get; set; } = null!;
        public DateTime? EnteredAt { get; set; }
    }
}
