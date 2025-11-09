using Polyclinic.Infrastructure.Mongo.Context;

namespace Polyclinic.Infrastructure.Mongo.Migrations;

/// <summary>
/// Interface for MongoDB database migrations.
/// Each migration represents a versioned database schema change or data transformation.
/// </summary>
public interface IMongoMigration
{
    public int Version { get; }
    public Task Up(MongoDbContext ctx, CancellationToken ct);
}
