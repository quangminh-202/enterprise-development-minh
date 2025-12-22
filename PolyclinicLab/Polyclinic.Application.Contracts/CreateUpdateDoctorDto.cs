namespace Polyclinic.Application.Contracts;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data Transfer Object (DTO) used for creating and updating doctors.
/// This DTO does not include the Id field as it's auto-generated.
/// </summary>
public class CreateUpdateDoctorDto
{
    [Required(ErrorMessage = "Passport is required")]
    public string Passport { get; set; } = "";

    [Required(ErrorMessage = "Full name is required")]
    public string FullName { get; set; } = "";

    [Range(1900, 2100, ErrorMessage = "Birth year must be valid")]
    public int BirthYear { get; set; } = DateTime.Now.Year - 30;

    [Required(ErrorMessage = "Specialization is required")]
    public string Specialization { get; set; } = "";

    [Range(0, 100, ErrorMessage = "Experience must not be negative")]
    public int Experience { get; set; } = 0;
}