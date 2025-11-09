using MongoDB.Driver;
using Polyclinic.Domain.Data;
using Polyclinic.Infrastructure.Mongo.Context;

namespace Polyclinic.Infrastructure.Mongo.Migrations;

/// <summary>
/// Migration to seed initial data into the database.
/// </summary>
public sealed class Migration_002_SeedData : IMongoMigration
{
    public int Version => 2;

    public async Task Up(MongoDbContext ctx, CancellationToken ct)
    {
        // Check if data already exists
        var hasDoctors = await ctx.Doctors.CountDocumentsAsync(FilterDefinition<Polyclinic.Domain.Models.Doctor>.Empty, cancellationToken: ct) > 0;
        var hasPatients = await ctx.Patients.CountDocumentsAsync(FilterDefinition<Polyclinic.Domain.Models.Patient>.Empty, cancellationToken: ct) > 0;
        var hasAppointments = await ctx.Appointments.CountDocumentsAsync(FilterDefinition<Polyclinic.Domain.Models.Appointment>.Empty, cancellationToken: ct) > 0;

        if (hasDoctors || hasPatients || hasAppointments)
        {
            Console.WriteLine("Database already contains data. Skipping seed migration.");
            return;
        }

        var fixture = new PolyclinicFixture();

        // Insert doctors
        if (fixture.Doctors.Count > 0)
        {
            await ctx.Doctors.InsertManyAsync(fixture.Doctors, cancellationToken: ct);
            Console.WriteLine($"Inserted {fixture.Doctors.Count} doctors.");
        }

        // Insert patients
        if (fixture.Patients.Count > 0)
        {
            await ctx.Patients.InsertManyAsync(fixture.Patients, cancellationToken: ct);
            Console.WriteLine($"Inserted {fixture.Patients.Count} patients.");
        }

        // Insert appointments
        if (fixture.Appointments.Count > 0)
        {
            await ctx.Appointments.InsertManyAsync(fixture.Appointments, cancellationToken: ct);
            Console.WriteLine($"Inserted {fixture.Appointments.Count} appointments.");
        }

        Console.WriteLine("Initial data seeding completed.");
    }
}