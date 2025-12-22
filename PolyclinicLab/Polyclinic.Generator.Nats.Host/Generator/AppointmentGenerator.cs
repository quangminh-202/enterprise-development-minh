using Bogus;
using Polyclinic.Application.Contracts;

namespace Polyclinic.Generator.Nats.Host.Generator;

/// <summary>
/// Provides functionality for generating random <see cref="CreateUpdateAppointmentDto"/> contracts.
/// </summary>
public static class AppointmentGenerator
{
    /// <summary>
    /// Generates a collection of randomly populated <see cref="CreateUpdateAppointmentDto"/> objects.
    /// </summary>
    /// <param name="count">The number of appointment contracts to generate.</param>
    /// <returns>A list of randomly generated <see cref="CreateUpdateAppointmentDto"/> instances.</returns>
    public static IList<CreateUpdateAppointmentDto> GenerateAppointments(int count) =>
        new Faker<CreateUpdateAppointmentDto>()
            .CustomInstantiator(f => new CreateUpdateAppointmentDto
            {
                Date = f.Date.Between(DateTime.Now.AddDays(-90), DateTime.Now.AddDays(90)),
                Room = f.Random.Int(101, 200), 
                IsRepeated = f.Random.Bool(0.4f),
                DoctorId = f.Random.Int(1, 10),
                PatientId = f.Random.Int(1, 10) // Database chỉ có 10 patients (ID: 1-10)
            })
            .Generate(count);
}
