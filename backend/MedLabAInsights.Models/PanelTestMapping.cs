namespace MedLabAInsights.Models
{
    public class PanelTestMapping
    {
        public int PanelTestId { get; set; }

        public int PanelId { get; set; }
        public Panel Panel { get; set; } = null!;

        public int TestId { get; set; }
        public Test Test { get; set; } = null!;

        public int ImportanceLevel { get; set; } // 1,2,3
    }
}
