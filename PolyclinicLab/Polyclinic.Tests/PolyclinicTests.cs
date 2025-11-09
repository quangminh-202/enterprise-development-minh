using MongoDB.Driver;
using Polyclinic.Infrastructure.Mongo.Context;
using Polyclinic.Infrastructure.Mongo.Repositories;
using Polyclinic.Infrastructure.Mongo.Migrations;

namespace Polyclinic.Tests;

/// <summary>
/// Integration tests for Polyclinic queries using MongoDB.
/// Each test compares hard-coded expected results (based on seeded data from migrations)
/// with actual results from LINQ queries over MongoDB collections.
/// </summary>
public class PolyclinicTests
{
    private static readonly object _migrationLock = new object();
    private static bool _migrationsRun = false;

    private readonly MongoDbContext _context;
    private readonly DoctorMongoRepository _doctorRepo;
    private readonly AppointmentMongoRepository _appointmentRepo;

    public PolyclinicTests()
    {
        // Use the actual MongoDB database from AppHost
        // This assumes MongoDB is running on localhost (from Aspire or standalone)
        var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") 
            ?? "mongodb://localhost:27017";
        var mongoClient = new MongoClient(connectionString);
        _context = new MongoDbContext(mongoClient.GetDatabase("polyclinic"));
        _doctorRepo = new DoctorMongoRepository(_context);
        _appointmentRepo = new AppointmentMongoRepository(_context);

        // Run migrations once before all tests
        lock (_migrationLock)
        {
            if (!_migrationsRun)
            {
                var migrations = new IMongoMigration[]
                {
                    new Migration_000_CreateCollections(),
                    new Migration_001_InitIndexes(),
                    new Migration_002_SeedData()
                };
                var migrationRunner = new MigrationRunner(_context, migrations);
                migrationRunner.RunAsync().GetAwaiter().GetResult();
                _migrationsRun = true;
            }
        }
    }

    /// <summary>
    /// (1) Verify that all doctors with at least 10 years of experience are returned.
    /// Expected: six doctors (Charlie, Bravo, Alpha, Foxtrot, Golf, Hotel).
    /// Actual: LINQ query filtering doctors by Experience >= 10.
    /// </summary>
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

    /// <summary>
    /// (2) Verify that all patients who visited Dr. Bravo (D2) are returned,
    /// sorted alphabetically by full name.
    /// Expected: Bob, Even, Henry, Jack.
    /// Actual: LINQ query filtering Appointments by doctor passport "D2".
    /// </summary>
    [Fact]
    public void PatientsByDoctorSortedByName()
    {
        var expected = new List<string> { "Bob", "Even", "Henry", "Jack" };

        var actual = _appointmentRepo.ReadAll()
            .Where(a => a.Doctor.Passport == "D2")
            .Select(a => a.Patient.FullName)
            .OrderBy(n => n)
            .ToList();

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// (3) Count all repeated appointments (IsRepeated = true)
    /// that took place within the last month.
    /// Expected: 3 (Bob, Diana, Frank).
    /// Actual: LINQ query counting appointments by date range and IsRepeated flag.
    /// </summary>
    [Fact]
    public void CountRepeatedAppointmentsLastMonth()
    {
        var expected = 3; // Even(-15), Diana(-5), Frank(-1)

        var now = DateTime.Now;
        var oneMonthAgo = now.AddMonths(-1);
        var actual = _appointmentRepo.ReadAll()
            .Count(a => a.IsRepeated && a.Date >= oneMonthAgo && a.Date <= now);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// (4) Return all patients older than 30 years who have appointments
    /// with more than one distinct doctor. Sort results by birth date.
    /// Expected: (empty list - no patients match the criteria)
    /// Actual: LINQ query filtering by age and counting distinct doctors.
    /// OPTIMIZED: Query starts from Appointments and groups by Patient (as per code review).
    /// </summary>
    [Fact]
    public void PatientsOlderThanThirtyWithMultipleDoctors()
    {
        var expected = new List<string> { "Bob", "Henry", "Jack" };

        var today = DateTime.Today;
        var cutoffDate = today.AddYears(-30);
        
        var actual = _appointmentRepo.ReadAll()
            .GroupBy(a => a.Patient.Passport) // Group by Passport instead of Patient object
            .Where(g =>
            {
                var patient = g.First().Patient; // Get patient from first appointment in group
                return patient.BirthDate <= cutoffDate &&
                       g.Select(a => a.Doctor.Passport).Distinct().Count() > 1;
            })
            .OrderBy(g => g.First().Patient.BirthDate)
            .Select(g => g.First().Patient.FullName)
            .ToList();

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// (5) Return all appointments scheduled in room "101"
    /// within the current month. Select patient names.
    /// Expected: Alice, Charlie, Henry.
    /// Actual: LINQ query filtering by room and date range.
    /// </summary>
    [Fact]
    public void AppointmentsCurrentMonthInSelectedRoom()
    {
        var expected = new List<string> {"Alice", "Charlie", "Henry"};

        var today = DateTime.Today;
        var firstDay = new DateTime(today.Year, today.Month, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);
        var allAppointments = _appointmentRepo.ReadAll();
        var actual = allAppointments
            .Where(a => a.Date >= firstDay && a.Date <= lastDay && a.Room == 101)
            .Select(a => a.Patient.FullName)
            .ToList();

        Assert.Equal(expected, actual);
    }
}