using Polyclinic.Domain.Models;
using Polyclinic.Tests; // để dùng PolyclinicFixture

namespace Polyclinic.Infrastructure.InMemory;

/// <summary>
/// Provides a way to seed in-memory repositories with initial data
/// from the PolyclinicFixture.
/// </summary>
public static class DataSeeder
{
    public static void Seed(
        IRepository<Doctor, int> doctorRepo,
        IRepository<Patient, int> patientRepo,
        IRepository<Appointment, int> appointmentRepo)
    {
        var fixture = new PolyclinicFixture();

        // Seed Doctors
        foreach (var d in fixture.Doctors)
            doctorRepo.Create(d);

        // Seed Patients
        foreach (var p in fixture.Patients)
            patientRepo.Create(p);

        // Seed Appointments
        foreach (var a in fixture.Appointments)
            appointmentRepo.Create(a);
    }
}
