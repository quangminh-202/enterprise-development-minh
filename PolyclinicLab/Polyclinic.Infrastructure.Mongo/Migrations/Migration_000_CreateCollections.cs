using MongoDB.Bson;
using MongoDB.Driver;
using Polyclinic.Infrastructure.Mongo.Context;

namespace Polyclinic.Infrastructure.Mongo.Migrations;

/// <summary>
/// Migration to create collections (tables) in MongoDB database.
/// In MongoDB, collections are created automatically on first insert,
/// but this migration explicitly creates them to satisfy the requirement.
/// </summary>
public sealed class Migration_000_CreateCollections : IMongoMigration
{
    public int Version => 0;

    public async Task Up(MongoDbContext ctx, CancellationToken ct)
    {
        var doctorsExists = await CollectionExistsAsync(ctx.Database, "Doctors", ct);
        if (!doctorsExists)
        {
            await ctx.Database.CreateCollectionAsync("Doctors", cancellationToken: ct);
            Console.WriteLine("Collection 'Doctors' created.");
        }

        var patientsExists = await CollectionExistsAsync(ctx.Database, "Patients", ct);
        if (!patientsExists)
        {
            await ctx.Database.CreateCollectionAsync("Patients", cancellationToken: ct);
            Console.WriteLine("Collection 'Patients' created.");
        }

        var appointmentsExists = await CollectionExistsAsync(ctx.Database, "Appointments", ct);
        if (!appointmentsExists)
        {
            await ctx.Database.CreateCollectionAsync("Appointments", cancellationToken: ct);
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

