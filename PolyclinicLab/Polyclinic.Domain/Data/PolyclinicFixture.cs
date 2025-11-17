using Polyclinic.Domain.Models;
using Polyclinic.Domain.Enums;

namespace Polyclinic.Domain.Data;

/// <summary>
/// Fixture that provides seeded data used for testing and development.
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
        Doctors =
        [
            new() {Id=1, Passport="D1", FullName="Dr. Charlie", BirthYear=1970, Specialization="Therapist", Experience=20 },
            new() {Id=2, Passport="D2", FullName="Dr. Bravo", BirthYear=1975, Specialization="Cardiologist", Experience=15 },
            new() {Id=3, Passport="D3", FullName="Dr. Alpha", BirthYear=1980, Specialization="Surgeon", Experience=12 },
            new() {Id=4, Passport="D4", FullName="Dr. Delta", BirthYear=1985, Specialization="Dentist", Experience=9 },
            new() {Id=5, Passport="D5", FullName="Dr. Echo", BirthYear=1990, Specialization="Neurologist", Experience=7 },
            new() {Id=6, Passport="D6", FullName="Dr. Foxtrot", BirthYear=1972, Specialization="Therapist", Experience=25 },
            new() {Id=7, Passport="D7", FullName="Dr. Golf", BirthYear=1978, Specialization="Surgeon", Experience=18 },
            new() {Id=8, Passport="D8", FullName="Dr. Hotel", BirthYear=1983, Specialization="Pediatrician", Experience=11 },
            new() {Id=9, Passport="D9", FullName="Dr. India", BirthYear=1988, Specialization="Cardiologist", Experience=6 },
            new() {Id=10, Passport="D10", FullName="Dr. Juliet", BirthYear=1992, Specialization="Therapist", Experience=5 }
        ];

        // Seed Patients
        Patients =
        [
            new() {Id=1, Passport="P1", FullName="Jack", Gender=Gender.Female, BirthDate=new(1985,1,3), Address="Addr1", BloodType=BloodType.A, RhFactor=RhFactor.Positive, Phone="+79613831297" },
            new() {Id=2, Passport="P2", FullName="Even", Gender=Gender.Male, BirthDate=new(1990,2,2), Address="Addr2", BloodType=BloodType.B, RhFactor=RhFactor.Negative, Phone="+79613831286" },
            new() {Id=3, Passport="P3", FullName="Henry", Gender=Gender.Male, BirthDate=new(1975,3,18), Address="Addr3", BloodType=BloodType.Ab, RhFactor=RhFactor.Positive, Phone="+79613831211" },
            new() {Id=4, Passport="P4", FullName="Diana", Gender=Gender.Female, BirthDate=new(2000,4,11), Address="Addr4", BloodType=BloodType.O, RhFactor=RhFactor.Negative, Phone="+79613831213" },
            new() {Id=5, Passport="P5", FullName="Alice", Gender=Gender.Female, BirthDate=new(1982,5,6), Address="Addr5", BloodType=BloodType.B, RhFactor=RhFactor.Positive, Phone="+79613831214" },
            new() {Id=6, Passport="P6", FullName="Frank", Gender=Gender.Male, BirthDate=new(1995,6,12), Address="Addr6", BloodType=BloodType.O, RhFactor=RhFactor.Negative, Phone="+79613831215" },
            new() {Id=7, Passport="P7", FullName="Grace", Gender=Gender.Female, BirthDate=new(1978,10,25), Address="Addr7", BloodType=BloodType.A, RhFactor=RhFactor.Positive, Phone="+79613831216" },
            new() {Id=8, Passport="P8", FullName="Charlie", Gender=Gender.Male, BirthDate=new(1988,8,15), Address="Addr8", BloodType=BloodType.Ab, RhFactor=RhFactor.Negative, Phone="+79613831217" },
            new() {Id=9, Passport="P9", FullName="Ivy", Gender=Gender.Female, BirthDate=new(1993,9,4), Address="Addr9", BloodType=BloodType.O, RhFactor=RhFactor.Positive, Phone="+79613831218" },
            new() {Id=10, Passport="P10", FullName="Bob", Gender=Gender.Male, BirthDate=new(1970,10,23), Address="Addr10", BloodType=BloodType.A, RhFactor=RhFactor.Negative, Phone="+79613831219" }
        ];

        // Seed Appointments with navigation properties populated
        var now = DateTime.Now;
        Appointments =
        [
            new() {Id = 1, PatientId=1, DoctorId=1, Patient=Patients[0], Doctor=Doctors[0], Date=now.AddDays(-20), Room=101, IsRepeated=false },
            new() {Id = 2, PatientId=2, DoctorId=2, Patient=Patients[1], Doctor=Doctors[1], Date=now.AddDays(-15), Room=101, IsRepeated=true },
            new() {Id = 3, PatientId=3, DoctorId=3, Patient=Patients[2], Doctor=Doctors[2], Date=now.AddDays(-10), Room=102, IsRepeated=false },
            new() {Id = 4, PatientId=4, DoctorId=4, Patient=Patients[3], Doctor=Doctors[3], Date=now.AddDays(-5),  Room=103, IsRepeated=true },
            new() {Id = 5, PatientId=5, DoctorId=5, Patient=Patients[4], Doctor=Doctors[4], Date=now.AddDays(-2),  Room=101, IsRepeated=false },
            new() {Id = 6, PatientId=6, DoctorId=6, Patient=Patients[5], Doctor=Doctors[5], Date=now.AddDays(-1),  Room=102, IsRepeated=true },
            new() {Id = 7, PatientId=7, DoctorId=7, Patient=Patients[6], Doctor=Doctors[6], Date=now,             Room=103, IsRepeated=false },
            new() {Id = 8, PatientId=8, DoctorId=8, Patient=Patients[7], Doctor=Doctors[7], Date=now.AddDays(1),  Room=101, IsRepeated=true },
            new() {Id = 9, PatientId=9, DoctorId=9, Patient=Patients[8], Doctor=Doctors[8], Date=now.AddDays(2),  Room=102, IsRepeated=false },
            new() {Id = 10, PatientId=10, DoctorId=10, Patient=Patients[9], Doctor=Doctors[9], Date=now.AddDays(3),  Room=103, IsRepeated=true },
            new() {Id = 11, PatientId=1, DoctorId=2, Patient=Patients[0], Doctor=Doctors[1], Date=now.AddDays(4),  Room=104, IsRepeated=false },
            new() {Id = 12, PatientId=10, DoctorId=2, Patient=Patients[9], Doctor=Doctors[1], Date=now.AddDays(5),  Room=103, IsRepeated=true },
            new() {Id = 13, PatientId=3, DoctorId=2, Patient=Patients[2], Doctor=Doctors[1], Date=now.AddDays(6),  Room=101, IsRepeated=false }
        ];
    }
}

