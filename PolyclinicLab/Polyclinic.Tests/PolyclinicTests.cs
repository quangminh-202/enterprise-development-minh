using System.Net.NetworkInformation;
using Polyclinic.Domain;

namespace Polyclinic.Tests;

/// <summary>
/// Unit tests for Polyclinic queries.
/// Each test compares hard-coded expected results (based on seeded data)
/// with actual results from LINQ queries over the fixture collections.
/// </summary>
public class PolyclinicTests(PolyclinicFixture fixture) : IClassFixture<PolyclinicFixture>
{
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

        var actual = fixture.Doctors
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

        var actual = fixture.Appointments
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
        var actual = fixture.Appointments
            .Count(a => a.IsRepeated && a.Date >= oneMonthAgo && a.Date <= now);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// (4) Return all patients older than 30 years who have appointments
    /// with more than one distinct doctor. Sort results by birth date.
    /// Expected: Bob, Henry, Jack.
    /// Actual: LINQ query filtering by age and counting distinct doctors.
    /// </summary>
    [Fact]
    public void PatientsOlderThanThirtyWithMultipleDoctors()
    {
        var expected = new List<string> { "Bob", "Henry", "Jack" };

        var now = DateTime.Now;
        var actual = fixture.Patients
            .Where(p => (now.Year - p.BirthDate.Year) > 30)
            .Where(p => fixture.Appointments
                .Where(a => a.Patient == p)
                .Select(a => a.Doctor.Passport)
                .Distinct()
                .Count() > 1)
            .OrderBy(p => p.BirthDate)
            .Select(p => p.FullName)
            .ToList();

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// (5) Return all appointments scheduled in room "101"
    /// within the current month. Select patient names.
    /// Expected: Jack, Even, Alice.
    /// Actual: LINQ query filtering by room and date range.
    /// </summary>
    [Fact]
    public void AppointmentsCurrentMonthInSelectedRoom()
    {
        var expected = new List<string> {"Jack", "Even", "Alice", "Charlie", "Henry"};

        var today = DateTime.Today;
        var firstDay = new DateTime(today.Year, today.Month, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);
        var actual = fixture.Appointments
            .Where(a => a.Date >= firstDay && a.Date <= lastDay && a.Room == "101")
            .Select(a => a.Patient.FullName)
            .ToList();

        Assert.Equal(expected, actual);
    }
}
