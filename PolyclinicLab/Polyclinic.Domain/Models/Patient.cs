using Polyclinic.Domain.Enums;

namespace Polyclinic.Domain.Models;

/// <summary>
/// Represents a patient of the polyclinic
/// </summary>
public class Patient
{
    /// <summary>
    /// The passport number of the patient.
    /// </summary>
    public required string Passport { get; set; }

    /// <summary>
    /// The full name of the patient.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// The gender of the patient.
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// The date of birth of the patient
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// The home address of the patient
    /// </summary>
    public required string Address { get; set; }

    /// <summary>
    /// The blood type of the patient
    /// </summary>
    public BloodType BloodType { get; set; }

    /// <summary>
    /// The Rh factor of the patient.
    /// </summary>
    public RhFactor RhFactor { get; set; }

    /// <summary>
    /// The contact phone number of the patient.
    /// </summary>
    public required string Phone { get; set; }
}