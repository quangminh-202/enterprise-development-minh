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
            .CustomInstantiator(f => new CreateUpdateAppointmentDto(
                f.Date.Between(DateTime.Now.AddDays(-90), DateTime.Now.AddDays(90)),
                f.Random.Int(0, 200),
                f.Random.Bool(0.4f),
                f.Random.Int(1, 10),
                f.Random.Int(1, 10)
            ))
            .Generate(count);
}
