namespace Polyclinic.Application.Contracts;

/// <summary>
/// Data Transfer Object (DTO) used for creating and updating appointments.
/// This DTO does not include the Id field as it's auto-generated.
/// </summary>
public class CreateUpdateAppointmentDto
{
    /// <summary>
    /// Date and time of the appointment.
    /// </summary>
    public required DateTime Date { get; set; }

    /// <summary>
    /// Room number where the appointment takes place.
    /// </summary>
    public required int Room { get; set; }

    /// <summary>
    /// Indicates whether this appointment is a repeated visit.
    /// </summary>
    public required bool IsRepeated { get; set; }

    /// <summary>
    /// ID of the attending doctor.
    /// </summary>
    public required int DoctorId { get; set; }

    /// <summary>
    /// ID of the patient.
    /// </summary>
    public required int PatientId { get; set; }
}