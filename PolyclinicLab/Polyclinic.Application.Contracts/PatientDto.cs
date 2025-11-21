namespace Polyclinic.Application.Contracts;

/// <summary>
/// Data Transfer Object (DTO) used to represent information about a patient.
/// Contains key details such as full name, demographic information, medical data, and contact phone.
/// </summary>
/// <param name="Id">Unique identifier of the patient.</param>
/// <param name="Passport">Passport number of the patient.</param>
/// <param name="FullName">Full name of the patient.</param>
/// <param name="Gender">Gender of the patient.</param>
/// <param name="BirthDate">Birth date of the patient.</param>
/// <param name="Address">Address of the patient.</param>
/// <param name="BloodType">Blood type of the patient.</param>
/// <param name="RhFactor">Rh factor of the patient.</param>
/// <param name="Phone">Contact phone number of the patient.</param>
public record PatientDto(
    int Id,
    string Passport,
    string FullName,
    string Gender,
    DateTime BirthDate,
    string Address,
    string BloodType,
    string RhFactor,
    string Phone
);
