namespace Polyclinic.Domain.Models;

/// <summary>
/// Represents a doctor working in the polyclinic.
/// </summary>
public class Doctor
{
    /// <summary>
    /// The passport number of the doctor.
    /// </summary>
    public required string Passport { get; set; }

    /// <summary>
    /// The full name of the doctor.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// The birth year of the doctor.
    /// </summary>
    public int BirthYear { get; set; }

    /// <summary>
    /// The medical specialization of the doctor
    /// </summary>
    public required string Specialization { get; set; }

    /// <summary>
    /// The total years of professional experience.
    /// </summary>
    public int Experience { get; set; }
}
