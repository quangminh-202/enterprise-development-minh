using MongoDB.Driver;
using Polyclinic.Domain.Models;
using Polyclinic.Infrastructure.Mongo.Context;

namespace Polyclinic.Infrastructure.Mongo.Migrations;

/// <summary>
/// Migration to create unique indexes on Passport fields for Doctor and Patient collections.
/// Ensures data integrity by preventing duplicate passport numbers.
/// </summary>
public sealed class Migration_001_InitIndexes : IMongoMigration
{
    public int Version => 1;

    public async Task Up(MongoDbContext ctx, CancellationToken ct)
    {
        var ixDoctor = new CreateIndexModel<Doctor>(
            Builders<Doctor>.IndexKeys.Ascending(d => d.Passport),
            new CreateIndexOptions { Unique = true });

        var ixPatient = new CreateIndexModel<Patient>(
            Builders<Patient>.IndexKeys.Ascending(p => p.Passport),
            new CreateIndexOptions { Unique = true });

        await ctx.Doctors.Indexes.CreateOneAsync(ixDoctor, cancellationToken: ct);
        await ctx.Patients.Indexes.CreateOneAsync(ixPatient, cancellationToken: ct);

        Console.WriteLine("Indexes created for Doctor and Patient.");
    }
}
