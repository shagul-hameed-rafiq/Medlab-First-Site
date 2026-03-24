using System.ComponentModel.DataAnnotations;

namespace MedLabAInsights.Api.Contracts.Members;

public sealed class CreateMemberRequest
{
    [Required, MaxLength(150)]
    public string Name { get; init; } = null!;

    [Required]
    public string Gender { get; init; } = null!;

    [Required]
    public DateTime DateOfBirth { get; init; }

    [Required]
    public string BloodGroup { get; init; } = null!;

    [Required]
    public long Contact { get; init; }

    [MaxLength(500)]
    public string? Address { get; init; }
}
