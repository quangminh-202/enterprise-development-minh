namespace Polyclinic.Application.Contracts;

/// <summary>
/// Data Transfer Object for analytics queries about patients.
/// </summary>
public record PatientAnalyticsDto
(
    int Id,
    string FullName,
    int Age,
    int DoctorCount
);