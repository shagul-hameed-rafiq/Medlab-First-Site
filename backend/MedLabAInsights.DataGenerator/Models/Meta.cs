namespace MedLabAInsights.DataGenerator.Models
{
    public class Meta
    {
        public Range? Range { get; set; }              // null for categorical
        public int? Precision { get; set; }           // null for categorical
        public List<string>? CustomValues { get; set; } // for categorical; null for numeric
    }
}
