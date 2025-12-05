using MongoDB.Driver;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Polyclinic.Domain.Models;

namespace Polyclinic.Infrastructure.Mongo.Migrations;

/// <summary>
/// Migration to create unique indexes on Passport fields for Doctor and Patient collections.
/// Ensures data integrity by preventing duplicate passport numbers.
/// </summary>
public sealed class Migration_001_InitIndexes(IMongoClient mongoClient) : IMongoMigration
{
    public int Version => 1;

    public async Task Up(PolyclinicDbContext ctx, CancellationToken ct)
    {
        var database = mongoClient.GetDatabase("polyclinic");
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
