namespace Polyclinic.Application.Contracts;

/// <summary>
/// Data Transfer Object (DTO) used for creating and updating appointments.
/// This DTO does not include the Id field as it's auto-generated.
/// </summary>
/// <param name="Date">Date and time of the appointment.</param>
/// <param name="Room">Room number where the appointment takes place.</param>
/// <param name="IsRepeated">Indicates whether this appointment is a repeated visit.</param>
/// <param name="DoctorId">ID of the attending doctor.</param>
/// <param name="PatientId">ID of the patient.</param>
public record CreateUpdateAppointmentDto(
    DateTime Date,
    int Room,
    bool IsRepeated,
    int DoctorId,
    int PatientId
);