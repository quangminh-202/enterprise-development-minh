using Polyclinic.Domain.Data;

namespace Polyclinic.Infrastructure.Mongo.Migrations;

/// <summary>
/// Migration to seed initial data into the database.
/// </summary>
public sealed class Migration_002_SeedData : IMongoMigration
{
    public int Version => 2;

    public async Task Up(PolyclinicDbContext ctx, CancellationToken ct)
    {
        Console.WriteLine("=== Migration_002: Seed Data ===");

        var fixture = new PolyclinicFixture();

        // Insert doctors
        if (fixture.Doctors.Count > 0)
        {
            await ctx.Doctors.AddRangeAsync(fixture.Doctors, ct);
            await ctx.SaveChangesAsync(ct);
            Console.WriteLine($"Inserted {fixture.Doctors.Count} doctors.");
        }

        // Insert patients
        if (fixture.Patients.Count > 0)
        {
            await ctx.Patients.AddRangeAsync(fixture.Patients, ct);
            await ctx.SaveChangesAsync(ct);
            Console.WriteLine($"Inserted {fixture.Patients.Count} patients.");
        }

        // Insert appointments - clear navigation properties to avoid issues
        if (fixture.Appointments.Count > 0)
        {
            foreach (var appointment in fixture.Appointments)
            {
                appointment.Doctor = null;
                appointment.Patient = null;
            }
            await ctx.Appointments.AddRangeAsync(fixture.Appointments, ct);
            await ctx.SaveChangesAsync(ct);
            Console.WriteLine($"Inserted {fixture.Appointments.Count} appointments.");
        }

        Console.WriteLine("Initial data seeding completed.");
    }
}