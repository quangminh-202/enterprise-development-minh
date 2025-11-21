namespace Polyclinic.Application.Contracts;

/// <summary>
/// Data Transfer Object (DTO) used to represent information about a doctor.
/// Contains basic details such as full name, specialization, and years of experience.
/// </summary>
/// <param name="Id">Unique identifier of the doctor.</param>
/// <param name="Passport">Passport number of the doctor.</param>
/// <param name="FullName">Full name of the doctor.</param>
/// <param name="BirthYear">Birth year of the doctor.</param>
/// <param name="Specialization">Doctor's medical specialization (e.g., therapist, cardiologist).</param>
/// <param name="Experience">Number of years of professional experience.</param>
public record DoctorDto(
    int Id,
    string Passport,
    string FullName,
    int BirthYear,
    string Specialization,
    int Experience
);
