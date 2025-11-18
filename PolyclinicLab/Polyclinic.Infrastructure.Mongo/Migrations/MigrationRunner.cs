using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Polyclinic.Infrastructure.Mongo.Migrations;

/// <summary>
/// Executes MongoDB database migrations in version order.
/// Tracks applied migrations in a special "__migrations" collection to prevent duplicate execution.
/// </summary>
public sealed class MigrationRunner(
    PolyclinicDbContext ctx,
    IEnumerable<IMongoMigration> migrations,
    ILogger<MigrationRunner>? log = null)
{
    private readonly PolyclinicDbContext _ctx = ctx;

    private readonly IMongoMigration[] _migrations = [.. migrations.OrderBy(m => m.Version)];

    private readonly ILogger<MigrationRunner>? _log = log;

    public async Task RunAsync(CancellationToken ct = default)
    {
        var mongoClient = _ctx.Database.GetService<IMongoClient>();
        var database = mongoClient?.GetDatabase("polyclinic") 
            ?? throw new InvalidOperationException("MongoDB client not configured");
        var col = database.GetCollection<BsonDocument>("__migrations");

        var index = new CreateIndexModel<BsonDocument>(
            Builders<BsonDocument>.IndexKeys.Ascending("v"),
            new CreateIndexOptions { Unique = true, Name = "ux_migrations_v" });
        await col.Indexes.CreateOneAsync(index, cancellationToken: ct);

        var appliedSet = (await col
            .Distinct<int>("v", FilterDefinition<BsonDocument>.Empty, cancellationToken: ct)
            .ToListAsync(ct))
            .ToHashSet();

        foreach (var m in _migrations)
        {
            ct.ThrowIfCancellationRequested();
            if (appliedSet.Contains(m.Version)) continue;

            _log?.LogInformation("Running migration {Version}…", m.Version);
            await m.Up(_ctx, ct);

            var doc = new BsonDocument
            {
                { "v", m.Version },
                { "appliedAt", DateTime.UtcNow }
            };

            try
            {
                await col.InsertOneAsync(doc, cancellationToken: ct);
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                _log?.LogWarning(ex, "Migration {Version} already recorded.", m.Version);
            }
        }
    }
}