namespace Polyclinic.Application.Contracts;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data Transfer Object (DTO) used for creating and updating doctors.
/// This DTO does not include the Id field as it's auto-generated.
/// </summary>
/// <param name="Passport">Passport number of the doctor.</param>
/// <param name="FullName">Full name of the doctor.</param>
/// <param name="BirthYear">Birth year of the doctor.</param>
/// <param name="Specialization">Doctor's medical specialization (e.g., therapist, cardiologist).</param>
/// <param name="Experience">Number of years of professional experience.</param>
public record CreateUpdateDoctorDto(
    [Required(ErrorMessage = "Passport is required")] string Passport,
    [Required(ErrorMessage = "Full name is required")] string FullName,
    [Range(1900, 2025, ErrorMessage = "Birth year must be valid")] int BirthYear,
    [Required(ErrorMessage = "Specialization is required")] string Specialization,
    [Range(0, 100, ErrorMessage = "Experience must not be negative")] int Experience
);