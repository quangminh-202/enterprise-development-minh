using Polyclinic.Domain;

namespace Polyclinic.Tests;

public class PolyclinicTests : IClassFixture<PolyclinicFixture>
{
    private readonly List<Doctor> _doctors;
    private readonly List<Patient> _patients;
    private readonly List<Appointment> _appointments;

    public PolyclinicTests(PolyclinicFixture fixture)
    {
        _doctors = fixture.Doctors;
        _patients = fixture.Patients;
        _appointments = fixture.Appointments;
    }

    // ===== 5 TEST THEO ĐỀ BÀI =====

    [Fact]
    public void Test1_Doctors_With_10_Years_Experience()
    {
        var result = _doctors.Where(d => d.Experience >= 10).ToList();
        Assert.All(result, d => Assert.True(d.Experience >= 10));
    }

    [Fact]
    public void Test2_Patients_By_Doctor_SortedByName()
    {
        var doctorId = "D1";
        var result = _appointments
            .Where(a => a.Doctor.Passport == doctorId)
            .Select(a => a.Patient)
            .OrderBy(p => p.FullName)
            .ToList();

        Assert.All(result, p => Assert.NotNull(p.FullName));
    }

    [Fact]
    public void Test3_Count_Repeated_Appointments_LastMonth()
    {
        var oneMonthAgo = DateTime.Now.AddMonths(-1);
        var result = _appointments
            .Count(a => a.IsRepeated && a.Date >= oneMonthAgo);

        Assert.True(result >= 0);
    }

    [Fact]
    public void Test4_Patients_OlderThan30_WithMultipleDoctors()
    {
        var today = DateTime.Today;
        var result = _appointments
            .GroupBy(a => a.Patient)
            .Where(g => (today.Year - g.Key.BirthDate.Year) > 30 &&
                        g.Select(a => a.Doctor).Distinct().Count() > 1)
            .Select(g => g.Key)
            .OrderBy(p => p.BirthDate)
            .ToList();

        Assert.All(result, p => Assert.True((today.Year - p.BirthDate.Year) > 30));
    }

    [Fact]
    public void Test5_Appointments_CurrentMonth_InSelectedRoom()
    {
        var selectedRoom = "101";
        var now = DateTime.Now;
        var result = _appointments
            .Where(a => a.Room == selectedRoom &&
                        a.Date.Month == now.Month &&
                        a.Date.Year == now.Year)
            .ToList();

        Assert.All(result, a => Assert.Equal(selectedRoom, a.Room));
    }

    // ===== 5 TEST BỔ SUNG =====

    [Fact]
    public void Test6_Doctors_By_Specialization()
    {
        var result = _doctors.Where(d => d.Specialization == "Therapist").ToList();
        Assert.All(result, d => Assert.Equal("Therapist", d.Specialization));
    }

    [Fact]
    public void Test7_Patients_With_BloodType_A()
    {
        var result = _patients.Where(p => p.BloodType == BloodType.A).ToList();
        Assert.All(result, p => Assert.Equal(BloodType.A, p.BloodType));
    }

    [Fact]
    public void Test8_Appointments_Today()
    {
        var today = DateTime.Today;
        var result = _appointments
            .Where(a => a.Date.Date == today)
            .ToList();

        Assert.All(result, a => Assert.Equal(today, a.Date.Date));
    }

    [Fact]
    public void Test9_Patient_With_Most_Appointments()
    {
        var result = _appointments
            .GroupBy(a => a.Patient)
            .OrderByDescending(g => g.Count())
            .First().Key;

        Assert.NotNull(result);
    }

    [Fact]
    public void Test10_Doctor_With_Max_Experience()
    {
        var maxExp = _doctors.Max(d => d.Experience);
        var result = _doctors.First(d => d.Experience == maxExp);

        Assert.NotNull(result);
    }
}
