using Microsoft.AspNetCore.Mvc;
using Polyclinic.Domain.Models;
using Polyclinic.Infrastructure.InMemory;

namespace Polyclinic.Api.Host.Controllers;

/// <summary>
/// Provides analytical endpoints that work with live in-memory data (same as CRUD operations).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController(
    IRepository<Doctor, int> doctorRepo,
    IRepository<Patient, int> patientRepo,
    IRepository<Appointment, int> appointmentRepo) : ControllerBase
{
    /// <summary>
    /// (1) Returns all doctors with at least 10 years of experience.
    /// </summary>
    [HttpGet("experienced-doctors")]
    public IActionResult GetExperiencedDoctors()
    {
        var result = doctorRepo.ReadAll()
            .Where(d => d.Experience >= 10)
            .Select(d => d.FullName)
            .ToList();

        return Ok(result);
    }

    /// <summary>
    /// (2) Returns all patients who visited a specific doctor,
    /// ordered alphabetically by full name.
    /// </summary>
    [HttpGet("patients-by-doctor/{doctorId}")]
    public IActionResult GetPatientsByDoctor(int doctorId)
    {
        var result = appointmentRepo.ReadAll()
            .Where(a => a.DoctorId == doctorId)
            .Select(a => a.Patient.FullName)
            .OrderBy(n => n)
            .ToList();

        return Ok(result);
    }

    /// <summary>
    /// (3) Counts all repeated appointments that occurred within the last month.
    /// </summary>
    [HttpGet("repeated-appointments-count")]
    public IActionResult GetRepeatedAppointmentsCount()
    {
        var now = DateTime.Now;
        var oneMonthAgo = now.AddMonths(-1);

        var count = appointmentRepo.ReadAll()
            .Count(a => a.IsRepeated && a.Date >= oneMonthAgo && a.Date <= now);

        return Ok(count);
    }

    /// <summary>
    /// (4) Returns all patients older than 30 years
    /// who have appointments with more than one distinct doctor.
    /// </summary>
    [HttpGet("patients-older-than-thirty-with-multiple-doctors")]
    public IActionResult GetPatientsOlderThanThirtyWithMultipleDoctors()
    {
        var now = DateTime.Now;

        var patients = patientRepo.ReadAll()
            .Where(p => (now.Year - p.BirthDate.Year) > 30)
            .Where(p => appointmentRepo.ReadAll()
                .Where(a => a.Patient.Id == p.Id)
                .Select(a => a.Doctor.Id)
                .Distinct()
                .Count() > 1)
            .OrderBy(p => p.BirthDate)
            .Select(p => p.FullName)
            .ToList();

        return Ok(patients);
    }

    /// <summary>
    /// (5) Returns all appointments scheduled in a specific room (default = 101)
    /// within the current month.
    /// </summary>
    [HttpGet("appointments-in-room")]
    public IActionResult GetAppointmentsInRoom([FromQuery] int room = 101)
    {
        var today = DateTime.Today;
        var firstDay = new DateTime(today.Year, today.Month, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);

        var result = appointmentRepo.ReadAll()
            .Where(a => a.Room == room && a.Date >= firstDay && a.Date <= lastDay)
            .Select(a => a.Patient.FullName)
            .ToList();

        return Ok(result);
    }
}
