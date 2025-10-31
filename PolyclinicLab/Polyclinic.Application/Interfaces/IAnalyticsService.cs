using Polyclinic.Application.Contracts;

namespace Polyclinic.Application.Interfaces;

/// <summary>
/// Interface for analytics-related business operations.
/// Provides methods for generating various analytics reports and statistics.
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Gets doctors with specified minimum years of experience.
    /// </summary>
    /// <param name="minExperience">Minimum years of experience required</param>
    /// <returns>List of experienced doctors</returns>
    public List<DoctorDto> GetExperiencedDoctors(int minExperience);

    /// <summary>
    /// Checks if a doctor exists by ID.
    /// </summary>
    /// <param name="doctorId">The doctor ID to check</param>
    /// <returns>True if doctor exists, false otherwise</returns>
    public bool DoctorExists(int doctorId);

    /// <summary>
    /// Gets patients who visited a specific doctor, ordered by name.
    /// </summary>
    /// <param name="doctorId">The doctor ID</param>
    /// <returns>List of patients ordered by name</returns>
    public List<PatientDto> GetPatientsByDoctor(int doctorId);

    /// <summary>
    /// Gets count of repeated appointments in the specified number of months.
    /// </summary>
    /// <param name="months">Number of months to look back</param>
    /// <returns>Analytics data about repeated appointments</returns>
    public RepeatedAppointmentsAnalyticsDto GetRepeatedAppointments(int months);

    /// <summary>
    /// Gets patients older than specified age with appointments with more than one distinct doctor.
    /// </summary>
    /// <param name="age">Minimum age threshold</param>
    /// <returns>List of patient analytics data</returns>
    public List<PatientAnalyticsDto> GetPatientsOlderThanWithMultipleDoctors(int age);

    /// <summary>
    /// Gets appointments in a specific room within the current month.
    /// </summary>
    /// <param name="room">Room number</param>
    /// <returns>List of appointments in the specified room</returns>
    public List<AppointmentDto> GetAppointmentsInRoom(int room);
}

