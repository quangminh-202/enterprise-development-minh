namespace Polyclinic.Application.Contracts;

/// <summary>
/// Data Transfer Object (DTO) used for creating and updating patients.
/// This DTO does not include the Id field as it's auto-generated.
/// </summary>
/// <param name="FullName">Full name of the patient.</param>
/// <param name="Gender">Gender of the patient.</param>
/// <param name="BirthDate">Birth date of the patient.</param>
/// <param name="Address">Address of the patient.</param>
/// <param name="BloodType">Blood type of the patient.</param>
/// <param name="RhFactor">Rh factor of the patient.</param>
/// <param name="Phone">Contact phone number of the patient.</param>
public record CreateUpdatePatientDto(
    string FullName,
    string Gender,
    DateTime BirthDate,
    string Address,
    string BloodType,
    string RhFactor,
    string Phone
);