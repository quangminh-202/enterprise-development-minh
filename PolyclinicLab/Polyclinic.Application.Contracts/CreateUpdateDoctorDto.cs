namespace Polyclinic.Application.Contracts;

/// <summary>
/// Data Transfer Object (DTO) used for creating and updating doctors.
/// This DTO does not include the Id field as it's auto-generated.
/// </summary>
/// <param name="FullName">Full name of the doctor.</param>
/// <param name="Specialization">Doctor's medical specialization (e.g., therapist, cardiologist).</param>
/// <param name="Experience">Number of years of professional experience.</param>
public record CreateUpdateDoctorDto(
    string FullName,
    string Specialization,
    int Experience
);





