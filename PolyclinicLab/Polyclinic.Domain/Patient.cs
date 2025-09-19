namespace Polyclinic.Domain;

/// <summary>
/// Represents the gender of a patient.
/// </summary>
public enum Gender { Male, Female}

/// <summary>
/// Represents the blood type of a patient.
/// </summary>
public enum BloodType { A, B, AB, O}

/// <summary>
/// Represents the Rh factor of a patient.
/// </summary>
public enum RhFactor { Positive, Negative }


/// <summary>
/// Represents a patient of the polyclinic
/// </summary>
public class Patient
{
    /// <summary>
    /// The passport number of the patient.
    /// </summary>
    public string Passport { get; set; }

    /// <summary>
    /// The full name of the patient.
    /// </summary>
    public string FullName { get; set; }

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
    public string Address { get; set; }

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
    public string Phone { get; set; }
}
