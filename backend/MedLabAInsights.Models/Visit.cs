namespace MedLabAInsights.Models
{
    public class Visit
    {
        public int VisitId { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;

        public DateTime VisitDateTime { get; set; }

        public int PanelId { get; set; }
        public Panel Panel { get; set; } = null!;

        public int? Height { get; set; }
        public int? Weight { get; set; }

        public int? Systolic { get; set; }
        public int? Diastolic { get; set; }

        public string? Notes { get; set; }
    }
}
