using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Polyclinic.ServiceDefaults;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class Extensions
{
    // Called in Program.cs: builder.AddServiceDefaults();
    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
            .AddCheck<MongoDbHealthCheck>("mongodb", tags: ["ready"]);

        return builder;
    }

    // Called in Program.cs: app.MapDefaultEndpoints();
    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = WriteResponse
        });

        app.MapHealthChecks("/alive", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live"),
            ResponseWriter = WriteResponse
        });

        return app;
    }

    // Function to create JSON response for /health and /alive endpoints
    private static Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var json = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            results = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        });

        return context.Response.WriteAsync(json);
    }
}

// Custom class to check MongoDB connection health
public sealed class MongoDbHealthCheck(IMongoClient client) : IHealthCheck
{
    private readonly IMongoClient _client = client;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _client.GetDatabase("polyclinic");
            var cmd = new BsonDocument("ping", 1);
            await db.RunCommandAsync<BsonDocument>(cmd, cancellationToken: cancellationToken);
            return HealthCheckResult.Healthy("MongoDB OK");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB unavailable", ex);
        }
    }
}
