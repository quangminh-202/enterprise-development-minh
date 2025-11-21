namespace Polyclinic.Application.Contracts;
using System.ComponentModel.DataAnnotations;
using Polyclinic.Domain.Enums;

/// <summary>
/// Data Transfer Object (DTO) used for creating and updating patients.
/// This DTO does not include the Id field as it's auto-generated.
/// </summary>
/// <param name="Passport">Passport number of the patient.</param>
/// <param name="FullName">Full name of the patient.</param>
/// <param name="Gender">Gender of the patient.</param>
/// <param name="BirthDate">Birth date of the patient.</param>
/// <param name="Address">Address of the patient.</param>
/// <param name="BloodType">Blood type of the patient.</param>
/// <param name="RhFactor">Rh factor of the patient.</param>
/// <param name="Phone">Contact phone number of the patient.</param>
public record CreateUpdatePatientDto(
    [Required(ErrorMessage = "Passport is required")] string Passport,
    [Required(ErrorMessage = "Full name is required")] string FullName,
    [Required(ErrorMessage = "Gender is required")] Gender Gender,
    [Required(ErrorMessage = "Birth date is required")] DateTime BirthDate,
    [Required(ErrorMessage = "Address is required")] string Address,
    [Required(ErrorMessage = "Blood type is required")] BloodType BloodType,
    [Required(ErrorMessage = "Rh factor is required")] RhFactor RhFactor,
    [Required(ErrorMessage = "Phone is required")] string Phone
);