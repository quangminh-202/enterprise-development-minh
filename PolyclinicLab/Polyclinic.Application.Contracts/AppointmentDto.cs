namespace Polyclinic.Application.Contracts;

public record AppointmentDto(int Id, DateTime Date, int Room, bool IsRepeated, string DoctorName, string PatientName);
