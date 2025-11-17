using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polyclinic.Infrastructure.Mongo;
using Polyclinic.Infrastructure.Mongo.Repositories;
using Polyclinic.Domain.Models;

namespace Polyclinic.Tests;

public class PolyclinicTests
{
    private readonly PolyclinicDbContext _context;
    private readonly DoctorEfRepository _doctorRepo;
    private readonly PatientEfRepository _patientRepo;
    private readonly AppointmentEfRepository _appointmentRepo;

    public PolyclinicTests()
    {
        // Create in-memory DI container
        var services = new ServiceCollection();

        // Register EF Core MongoDB Context
        services.AddDbContext<PolyclinicDbContext>(options =>
        {
            options.UseMongoDB("mongodb://localhost:27017", "polyclinic_test");
        });

        // Register repositories
        services.AddScoped<DoctorEfRepository>();
        services.AddScoped<PatientEfRepository>();
        services.AddScoped<AppointmentEfRepository>();

        // Build provider
        var provider = services.BuildServiceProvider();

        _context = provider.GetRequiredService<PolyclinicDbContext>();
        _doctorRepo = provider.GetRequiredService<DoctorEfRepository>();
        _patientRepo = provider.GetRequiredService<PatientEfRepository>();
        _appointmentRepo = provider.GetRequiredService<AppointmentEfRepository>();
        
        // Seed test data
        SeedTestData();
    }
    
    private void SeedTestData()
    {
        // Clear existing data
        var existingAppointments = _context.Appointments.ToList();
        _context.Appointments.RemoveRange(existingAppointments);
        
        var existingDoctors = _context.Doctors.ToList();
        _context.Doctors.RemoveRange(existingDoctors);
        
        var existingPatients = _context.Patients.ToList();
        _context.Patients.RemoveRange(existingPatients);
        
        _context.SaveChanges();
        
        // Add test data
        var fixture = new Polyclinic.Domain.Data.PolyclinicFixture();
        
        _context.Doctors.AddRange(fixture.Doctors);
        _context.Patients.AddRange(fixture.Patients);
        _context.SaveChanges();
        
        _context.Appointments.AddRange(fixture.Appointments);
        _context.SaveChanges();
    }

    [Fact]
    public void DoctorsWithTenYearsExperience()
    {
        var expected = new List<string> {
            "Dr. Charlie", "Dr. Bravo", "Dr. Alpha",
            "Dr. Foxtrot", "Dr. Golf", "Dr. Hotel"
        };

        var actual = _doctorRepo.ReadAll()
            .Where(d => d.Experience >= 10)
            .Select(d => d.FullName)
            .ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void PatientsByDoctorSortedByName()
    {
        var expected = new List<string> { "Bob", "Even", "Henry", "Jack" };

        // Find doctor with passport D2
        var doctor = _doctorRepo.ReadAll().FirstOrDefault(d => d.Passport == "D2");
        Assert.NotNull(doctor);

        // Get patient IDs from appointments
        var patientIds = _appointmentRepo.ReadAll()
            .Where(a => a.DoctorId == doctor.Id)
            .Select(a => a.PatientId)
            .Distinct()
            .ToList();

        // Get patient names
        var actual = _patientRepo.ReadAll()
            .Where(p => patientIds.Contains(p.Id))
            .Select(p => p.FullName)
            .OrderBy(n => n)
            .ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CountRepeatedAppointmentsLastMonth()
    {
        var expected = 3;

        var now = DateTime.Now;
        var oneMonthAgo = now.AddMonths(-1);

        var actual = _appointmentRepo.ReadAll()
            .Count(a => a.IsRepeated && a.Date >= oneMonthAgo && a.Date <= now);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void PatientsOlderThanThirtyWithMultipleDoctors()
    {
        var expected = new List<string> { "Bob", "Henry", "Jack" };

        var today = DateTime.Today;
        var cutoffDate = today.AddYears(-30);

        // Get patients with multiple doctors
        var patientAppointments = _appointmentRepo.ReadAll()
            .GroupBy(a => a.PatientId)
            .Where(g => g.Select(a => a.DoctorId).Distinct().Count() > 1)
            .Select(g => g.Key)
            .ToList();

        // Filter by age and sort
        var actual = _patientRepo.ReadAll()
            .Where(p => p.BirthDate <= cutoffDate && patientAppointments.Contains(p.Id))
            .OrderBy(p => p.BirthDate)
            .Select(p => p.FullName)
            .ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AppointmentsCurrentMonthInSelectedRoom()
    {
        // Expected: appointments in room 101 in current month
        // Based on fixture: Id=2 (Even, -15 days), Id=5 (Alice, -2 days), 
        // Id=8 (Charlie, +1 day), Id=13 (Henry, +6 days)
        // Id=1 (Jack, -20 days) may be in previous month depending on current date
        var expected = new List<string> { "Alice", "Charlie", "Even", "Henry" };

        var today = DateTime.Today;
        var first = new DateTime(today.Year, today.Month, 1);
        var last = first.AddMonths(1).AddDays(-1);

        // Get patient IDs from appointments in room 101 this month
        var patientIds = _appointmentRepo.ReadAll()
            .Where(a => a.Date >= first && a.Date <= last && a.Room == 101)
            .Select(a => a.PatientId)
            .ToList();

        // Get patient names
        var actual = _patientRepo.ReadAll()
            .Where(p => patientIds.Contains(p.Id))
            .Select(p => p.FullName)
            .OrderBy(n => n)
            .ToList();

        Assert.Equal(expected, actual);
    }
}
