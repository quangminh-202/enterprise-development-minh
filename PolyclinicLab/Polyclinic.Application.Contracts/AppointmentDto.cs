namespace Polyclinic.Application.Contracts;

/// <summary>
/// Data Transfer Object (DTO) used to represent information 
/// about a patient's appointment with a doctor.
/// Contains basic appointment details such as date, room, 
/// whether it is a repeated visit, and names of the doctor and patient.
/// </summary>
/// <param name="Id">Unique identifier of the appointment.</param>
/// <param name="Date">Date and time of the appointment.</param>
/// <param name="Room">Room number where the appointment takes place.</param>
/// <param name="IsRepeated">Indicates whether this appointment is a repeated visit.</param>
/// <param name="DoctorName">Full name of the attending doctor.</param>
/// <param name="PatientName">Full name of the patient.</param>
public record AppointmentDto(
    int Id,
    DateTime Date,
    int Room,
    bool IsRepeated,
    string DoctorName,
    string PatientName
);