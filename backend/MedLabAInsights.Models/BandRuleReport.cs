namespace MedLabAInsights.Models
{
    public class BandRuleReport
    {
        public int BandId { get; set; }

        public int TestId { get; set; }
        public Test Test { get; set; } = null!;

        public string BandName { get; set; } = null!;
        public string BandCode { get; set; } = null!;

        public double RangeMin { get; set; }
        public double RangeMax { get; set; }

        public int Severity { get; set; }

        public string? CustomValue { get; set; }

        public string StandardReport { get; set; } = null!;
    }
}
