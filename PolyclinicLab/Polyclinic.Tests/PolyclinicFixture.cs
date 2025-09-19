using Polyclinic.Domain;

namespace Polyclinic.Tests;


/// <summary>
/// Fixture that provides seeded data used for unit tests.
/// Contains three collections:
/// - <see cref="Doctors"/>: seeded doctors (at least 10).
/// - <see cref="Patients"/>: seeded patients (at least 10).
/// - <see cref="Appointments"/>: seeded appointments between doctors and patients.
/// </summary>
public class PolyclinicFixture
{
    public List<Doctor> Doctors { get; }
    public List<Patient> Patients { get; }
    public List<Appointment> Appointments { get; }

    public PolyclinicFixture()
    {
        // Seed Doctors
        Doctors = new()
        {
            new() { Passport="D1", FullName="Dr. Charlie", BirthYear=1970, Specialization="Therapist", Experience=20 },
            new() { Passport="D2", FullName="Dr. Bravo", BirthYear=1975, Specialization="Cardiologist", Experience=15 },
            new() { Passport="D3", FullName="Dr. Alpha", BirthYear=1980, Specialization="Surgeon", Experience=12 },
            new() { Passport="D4", FullName="Dr. Delta", BirthYear=1985, Specialization="Dentist", Experience=9 },
            new() { Passport="D5", FullName="Dr. Echo", BirthYear=1990, Specialization="Neurologist", Experience=7 },
            new() { Passport="D6", FullName="Dr. Foxtrot", BirthYear=1972, Specialization="Therapist", Experience=25 },
            new() { Passport="D7", FullName="Dr. Golf", BirthYear=1978, Specialization="Surgeon", Experience=18 },
            new() { Passport="D8", FullName="Dr. Hotel", BirthYear=1983, Specialization="Pediatrician", Experience=11 },
            new() { Passport="D9", FullName="Dr. India", BirthYear=1988, Specialization="Cardiologist", Experience=6 },
            new() { Passport="D10", FullName="Dr. Juliet", BirthYear=1992, Specialization="Therapist", Experience=5 }
        };

        // Seed Patients
        Patients = new()
        {
            new() { Passport="P1", FullName="Jack", Gender=Gender.Female, BirthDate=new(1985,1,3), Address="Addr1", BloodType=BloodType.A, RhFactor=RhFactor.Positive, Phone="+79613831297" },
            new() { Passport="P2", FullName="Even", Gender=Gender.Male, BirthDate=new(1990,2,2), Address="Addr2", BloodType=BloodType.B, RhFactor=RhFactor.Negative, Phone="+79613831286" },
            new() { Passport="P3", FullName="Henry", Gender=Gender.Male, BirthDate=new(1975,3,18), Address="Addr3", BloodType=BloodType.AB, RhFactor=RhFactor.Positive, Phone="+79613831211" },
            new() { Passport="P4", FullName="Diana", Gender=Gender.Female, BirthDate=new(2000,4,11), Address="Addr4", BloodType=BloodType.O, RhFactor=RhFactor.Negative, Phone="+79613831213" },
            new() { Passport="P5", FullName="Alice", Gender=Gender.Female, BirthDate=new(1982,5,6), Address="Addr5", BloodType=BloodType.B, RhFactor=RhFactor.Positive, Phone="+79613831214" },
            new() { Passport="P6", FullName="Frank", Gender=Gender.Male, BirthDate=new(1995,6,12), Address="Addr6", BloodType=BloodType.O, RhFactor=RhFactor.Negative, Phone="+79613831215" },
            new() { Passport="P7", FullName="Grace", Gender=Gender.Female, BirthDate=new(1978,10,25), Address="Addr7", BloodType=BloodType.A, RhFactor=RhFactor.Positive, Phone="+79613831216" },
            new() { Passport="P8", FullName="Charlie", Gender=Gender.Male, BirthDate=new(1988,8,15), Address="Addr8", BloodType=BloodType.AB, RhFactor=RhFactor.Negative, Phone="+79613831217" },
            new() { Passport="P9", FullName="Ivy", Gender=Gender.Female, BirthDate=new(1993,9,4), Address="Addr9", BloodType=BloodType.O, RhFactor=RhFactor.Positive, Phone="+79613831218" },
            new() { Passport="P10", FullName="Bob", Gender=Gender.Male, BirthDate=new(1970,10,23), Address="Addr10", BloodType=BloodType.A, RhFactor=RhFactor.Negative, Phone="+79613831219" }
        };

        // Seed Appointments
        var now = DateTime.Now;
        Appointments = new()
        {
            new() { Date=now.AddDays(-20), Room="101", IsRepeated=false, Patient=Patients[0], Doctor=Doctors[0] },
            new() { Date=now.AddDays(-15), Room="101", IsRepeated=true, Patient=Patients[1], Doctor=Doctors[1] },
            new() { Date=now.AddDays(-10), Room="102", IsRepeated=false, Patient=Patients[2], Doctor=Doctors[2] },
            new() { Date=now.AddDays(-5),  Room="103", IsRepeated=true, Patient=Patients[3], Doctor=Doctors[3] },
            new() { Date=now.AddDays(-2),  Room="101", IsRepeated=false, Patient=Patients[4], Doctor=Doctors[4] },
            new() { Date=now.AddDays(-1),  Room="102", IsRepeated=true, Patient=Patients[5], Doctor=Doctors[5] },
            new() { Date=now,             Room="103", IsRepeated=false, Patient=Patients[6], Doctor=Doctors[6] },
            new() { Date=now.AddDays(1),  Room="101", IsRepeated=true, Patient=Patients[7], Doctor=Doctors[7] },
            new() { Date=now.AddDays(2),  Room="102", IsRepeated=false, Patient=Patients[8], Doctor=Doctors[8] },
            new() { Date=now.AddDays(3),  Room="103", IsRepeated=true, Patient=Patients[9], Doctor=Doctors[9] },
            new() { Date=now.AddDays(4),  Room="104", IsRepeated=false, Patient=Patients[0], Doctor=Doctors[1] },
            new() { Date=now.AddDays(5),  Room="103", IsRepeated=true, Patient=Patients[9], Doctor=Doctors[1] },
            new() { Date=now.AddDays(6),  Room="101", IsRepeated=false, Patient=Patients[2], Doctor=Doctors[1] }
        };
    }
}
