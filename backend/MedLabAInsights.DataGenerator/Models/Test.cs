namespace MedLabAInsights.DataGenerator.Models
{
    public class Test
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
        public required string DataType { get; set; }
        public required string Unit { get; set; }    
        public List<string>? Groups { get; set; }  
        public required Meta Meta { get; set; }
    }
}
