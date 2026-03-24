namespace MedLabAInsights.Models
{
    public class Member
    {
        public int MemberId { get; set; }

        public string Name { get; set; } = null!;

        public Gender Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public BloodGroup BloodGroup { get; set; }

        public long Contact { get; set; }

        public string? Address { get; set; }
    }
}
