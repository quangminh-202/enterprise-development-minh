using Microsoft.AspNetCore.Mvc;
using Polyclinic.Application.Contracts;
using Polyclinic.Application.Interfaces;

namespace Polyclinic.Api.Host.Controllers;

/// <summary>
/// Provides analytical endpoints that work with live in-memory data (same as CRUD operations).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger) : ControllerBase
{
    /// <summary>
    /// (1) Returns all doctors with at least 10 years of experience.
    /// </summary>
    [HttpGet("experienced-doctors")]
    public ActionResult<List<DoctorDto>> GetExperiencedDoctor([FromQuery] int minExperience = 10)
    {
        try
        {
            if (minExperience < 0 || minExperience > 100)
            {
                logger.LogWarning("Invalid minExperience value: {MinExperience}", minExperience);
                return BadRequest("Experience value must be between 0 and 100.");
            }

            var result = analyticsService.GetExperiencedDoctors(minExperience);
            logger.LogInformation("Retrieved {Count} experienced doctors with min {MinExperience} years", result.Count, minExperience);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting experienced doctors");
            return StatusCode(500, "An error occurred while retrieving experienced doctors");
        }
    }

    /// <summary>
    /// (2) Returns all patients who visited a specific doctor,
    /// ordered alphabetically by full name.
    /// </summary>
    [HttpGet("patients-by-doctor/{doctorId}")]
    public ActionResult<List<PatientDto>> GetPatientsByDoctor(int doctorId)
    {
        try
        {
            if (doctorId <= 0)
            {
                logger.LogWarning("Invalid doctor ID provided: {DoctorId}", doctorId);
                return BadRequest("Doctor ID must be greater than 0");
            }

            var doctorExists = analyticsService.DoctorExists(doctorId);
            if (!doctorExists)
            {
                logger.LogWarning("Doctor with ID {DoctorId} not found", doctorId);
                return NotFound($"Doctor with ID {doctorId} not found.");
            }

            var result = analyticsService.GetPatientsByDoctor(doctorId);
            logger.LogInformation("Retrieved {Count} patients for doctor {DoctorId}", result.Count, doctorId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting patients by doctor {DoctorId}", doctorId);
            return StatusCode(500, "An error occurred while retrieving patients");
        }
    }

    /// <summary>
    /// (3) Returns all repeated appointments that occurred within the last N months.
    /// Default = 1.
    /// </summary>
    [HttpGet("repeated-appointments-count")]
    public ActionResult<RepeatedAppointmentsAnalyticsDto> GetRepeatedAppointments([FromQuery] int months = 1)
    {
        try
        {
            if (months <= 0 || months > 12)
            {
                logger.LogWarning("Invalid months parameter: {Months}", months);
                return BadRequest("The number of months must be between 1 and 12.");
            }

            var result = analyticsService.GetRepeatedAppointments(months);
            logger.LogInformation("Retrieved {Count} repeated appointments within last {Months} months", 
                result.TotalCount, months);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting repeated appointments");
            return StatusCode(500, "An error occurred while retrieving repeated appointments");
        }
    }


    /// <summary>
    /// (4) Returns all patients older than 30 years
    /// who have appointments with more than one distinct doctor.
    /// </summary>
    [HttpGet("patients-older-than-with-multiple-doctors")]
    public ActionResult<List<PatientAnalyticsDto>> GetPatientsOlderThanWithMultipleDoctors([FromQuery] int age = 30)
    {
        try
        {
            if (age < 0 || age > 100)
            {
                logger.LogWarning("Invalid age parameter: {Age}", age);
                return BadRequest("Age must be between 0 and 100.");
            }

            var result = analyticsService.GetPatientsOlderThanWithMultipleDoctors(age);
            logger.LogInformation("Retrieved {Count} patients older than {Age} with multiple doctors", 
                result.Count, age);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting patients older than {Age}", age);
            return StatusCode(500, "An error occurred while retrieving patients");
        }
    }

        /// <summary>
    /// (5) Returns all appointments scheduled in a specific room (default = 101)
    /// within the current month.
    /// </summary>
    [HttpGet("appointments-in-room")]
    public ActionResult<List<AppointmentDto>> GetAppointmentsInRoom([FromQuery] int room = 101)
    {
        try
        {
            if (room <= 0)
            {
                logger.LogWarning("Invalid room number: {Room}", room);
                return BadRequest("Room number must be greater than 0");
            }

            var result = analyticsService.GetAppointmentsInRoom(room);

            if (result == null || result.Count == 0)
            {
                logger.LogInformation("No appointments found in room {Room} for the current month", room);
                return NotFound($"No appointments found in room {room} for the current month.");
            }
        
            logger.LogInformation("Retrieved {Count} appointments in room {Room}", result.Count, room);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting appointments in room {Room}", room);
            return StatusCode(500, "An error occurred while retrieving appointments");
        }
    }
}
