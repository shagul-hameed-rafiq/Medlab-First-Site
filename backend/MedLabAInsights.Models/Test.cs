namespace MedLabAInsights.Models
{
    public class Test
    {
        public int TestId { get; set; }                 // PK, auto increment
        public string TestName { get; set; } = null!;   // NOT NULL
        public string? TestCode { get; set; }           // NULL

        public double? MinValue { get; set; }           // NULL allowed
        public double? MaxValue { get; set; }           // NULL allowed

        public string? CustomValues { get; set; }       // NULL (like "Positive/Negative", etc.)
        public string? Unit { get; set; }               // NULL
    }
}
