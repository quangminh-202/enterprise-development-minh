using Bogus;
using Polyclinic.Application.Contracts;
using Polyclinic.Domain.Enums;

namespace Polyclinic.Generator.Nats.Host.Services;

/// <summary>
/// Static generator for creating fake contracts
/// </summary>
public static class ContractGenerator
{
    private static readonly Faker _faker = new();

    /// <summary>
    /// Generates a list of fake patient contracts
    /// </summary>
    /// <param name="count">Number of patients to generate</param>
    /// <returns>List of patient DTOs</returns>
    public static IList<CreateUpdatePatientDto> GeneratePatients(int count)
    {
        var patients = new List<CreateUpdatePatientDto>();
        for (int i = 0; i < count; i++)
        {
            patients.Add(new CreateUpdatePatientDto(
                Passport: _faker.Random.AlphaNumeric(10),
                FullName: _faker.Name.FullName(),
                Gender: _faker.PickRandom<Gender>(),
                BirthDate: _faker.Date.Past(50, DateTime.Now.AddYears(-18)),
                Address: _faker.Address.FullAddress(),
                BloodType: _faker.PickRandom<BloodType>(),
                RhFactor: _faker.PickRandom<RhFactor>(),
                Phone: _faker.Phone.PhoneNumber()
            ));
        }
        return patients;
    }

    /// <summary>
    /// Generates a list of fake doctor contracts
    /// </summary>
    /// <param name="count">Number of doctors to generate</param>
    /// <returns>List of doctor DTOs</returns>
    public static IList<CreateUpdateDoctorDto> GenerateDoctors(int count)
    {
        var doctors = new List<CreateUpdateDoctorDto>();
        for (int i = 0; i < count; i++)
        {
            doctors.Add(new CreateUpdateDoctorDto(
                Passport: _faker.Random.AlphaNumeric(10),
                FullName: _faker.Name.FullName(),
                BirthYear: _faker.Random.Int(1950, 1995),
                Specialization: _faker.PickRandom("Cardiology", "Neurology", "Pediatrics", "Orthopedics", "Dermatology"),
                Experience: _faker.Random.Int(1, 40)
            ));
        }
        return doctors;
    }

    /// <summary>
    /// Generates a list of fake appointment contracts
    /// </summary>
    /// <param name="count">Number of appointments to generate</param>
    /// <returns>List of appointment DTOs</returns>
    public static IList<CreateUpdateAppointmentDto> GenerateAppointments(int count)
    {
        var appointments = new List<CreateUpdateAppointmentDto>();
        for (int i = 0; i < count; i++)
        {
            appointments.Add(new CreateUpdateAppointmentDto(
                Date: _faker.Date.Future(1),
                Room: _faker.Random.Int(1, 50),
                IsRepeated: _faker.Random.Bool(),
                DoctorId: _faker.Random.Int(1, 10),
                PatientId: _faker.Random.Int(1, 20)
            ));
        }
        return appointments;
    }
}
