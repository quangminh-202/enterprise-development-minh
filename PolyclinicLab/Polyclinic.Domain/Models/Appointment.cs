namespace Polyclinic.Domain.Models;

/// <summary>
/// Represents a medical appointment in the polyclinic.
/// </summary>
public class Appointment
{
    /// <summary>
    /// Internal ID for in-memory storage.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The patient ID (foreign key).
    /// </summary>
    public int PatientId { get; set; }

    /// <summary>
    /// The doctor ID (foreign key).
    /// </summary>
    public int DoctorId { get; set; }

    /// <summary>
    /// The date and time of the appointment.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// The room number where the appointment takes place
    /// </summary>
    public required int Room { get; set; }

    /// <summary>
    /// Indicates whether this is a repeated (follow-up) appointment (true)
    /// or the first visit (false).
    /// </summary>
    public bool IsRepeated { get; set; }

    /// <summary>
    /// The patient attending the appointment.
    /// </summary>
    public Patient? Patient { get; set; }

    /// <summary>
    /// The doctor assigned to the appointment.
    /// </summary>
    public Doctor? Doctor { get; set; }
}