namespace Polyclinic.Application.Contracts;

/// <summary>
/// Data Transfer Object (DTO) used to represent information about a patient.
/// Contains key details such as full name, passport number, and contact phone.
/// </summary>
/// <param name="Id">Unique identifier of the patient.</param>
/// <param name="FullName">Full name of the patient.</param>
/// <param name="Passport">Passport number of the patient, used as a unique personal identifier.</param>
/// <param name="Phone">Contact phone number of the patient.</param>
public record PatientDto(
    int Id,
    string FullName,
    string Gender,
    DateTime BirthDate,
    string Address,
    string BloodType,
    string RhFactor,
    string Phone
);