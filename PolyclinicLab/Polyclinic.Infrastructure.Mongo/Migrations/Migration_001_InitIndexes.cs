using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Polyclinic.Domain.Models;
using Polyclinic.Infrastructure.Mongo;

namespace Polyclinic.Infrastructure.Mongo.Migrations;

/// <summary>
/// Migration to create unique indexes on Passport fields for Doctor and Patient collections.
/// Ensures data integrity by preventing duplicate passport numbers.
/// </summary>
public sealed class Migration_001_InitIndexes : IMongoMigration
{
    public int Version => 1;

    public async Task Up(PolyclinicDbContext ctx, CancellationToken ct)
    {
        var mongoClient = ctx.Database.GetService<IMongoClient>();
        var database = mongoClient?.GetDatabase("polyclinic") 
            ?? throw new InvalidOperationException("MongoDB client not configured");
        var doctorsCollection = database.GetCollection<Doctor>("Doctors");
        var patientsCollection = database.GetCollection<Patient>("Patients");
        
        var ixDoctor = new CreateIndexModel<Doctor>(
            Builders<Doctor>.IndexKeys.Ascending(d => d.Passport),
            new CreateIndexOptions { Unique = true });

        var ixPatient = new CreateIndexModel<Patient>(
            Builders<Patient>.IndexKeys.Ascending(p => p.Passport),
            new CreateIndexOptions { Unique = true });

        await doctorsCollection.Indexes.CreateOneAsync(ixDoctor, cancellationToken: ct);
        await patientsCollection.Indexes.CreateOneAsync(ixPatient, cancellationToken: ct);

        Console.WriteLine("Indexes created for Doctor and Patient.");
    }
}
