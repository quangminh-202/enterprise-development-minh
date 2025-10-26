namespace Polyclinic.Application.Contracts;

/// <summary>
/// DTO for repeated appointments analytics result.
/// </summary>
public record RepeatedAppointmentsAnalyticsDto(
    int TotalCount,
    List<AppointmentDto> Appointments
);
