using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Polyclinic.Infrastructure.Mongo;

namespace Polyclinic.Infrastructure.Mongo.Migrations;

/// <summary>
/// Migration to create collections (tables) in MongoDB database.
/// In MongoDB, collections are created automatically on first insert,
/// but this migration explicitly creates them to satisfy the requirement.
/// </summary>
public sealed class Migration_000_CreateCollections : IMongoMigration
{
    public int Version => 0;

    public async Task Up(PolyclinicDbContext ctx, CancellationToken ct)
    {
        var mongoClient = ctx.Database.GetService<IMongoClient>();
        var database = mongoClient?.GetDatabase("polyclinic") 
            ?? throw new InvalidOperationException("MongoDB client not configured");
        
        var doctorsExists = await CollectionExistsAsync(database, "Doctors", ct);
        if (!doctorsExists)
        {
            await database.CreateCollectionAsync("Doctors", cancellationToken: ct);
            Console.WriteLine("Collection 'Doctors' created.");
        }

        var patientsExists = await CollectionExistsAsync(database, "Patients", ct);
        if (!patientsExists)
        {
            await database.CreateCollectionAsync("Patients", cancellationToken: ct);
            Console.WriteLine("Collection 'Patients' created.");
        }

        var appointmentsExists = await CollectionExistsAsync(database, "Appointments", ct);
        if (!appointmentsExists)
        {
            await database.CreateCollectionAsync("Appointments", cancellationToken: ct);
            Console.WriteLine("Collection 'Appointments' created.");
        }

        Console.WriteLine("All collections created successfully.");
    }

    private static async Task<bool> CollectionExistsAsync(IMongoDatabase database, string collectionName, CancellationToken ct)
    {
        var filter = new BsonDocument("name", collectionName);
        var collections = await database.ListCollectionNamesAsync(new ListCollectionNamesOptions { Filter = filter }, ct);
        return await collections.AnyAsync(ct);
    }
}

