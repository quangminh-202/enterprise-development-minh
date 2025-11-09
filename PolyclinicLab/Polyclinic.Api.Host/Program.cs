using Polyclinic.Domain.Models;
using Polyclinic.Domain.Interfaces;
using Polyclinic.Application.Services;
using Polyclinic.Application.Interfaces;
using Polyclinic.Application.MappingProfiles;
using Polyclinic.Infrastructure.Mongo.Repositories;
using Polyclinic.Infrastructure.Mongo.Context;
using Polyclinic.Infrastructure.Mongo.Migrations;
using Polyclinic.ServiceDefaults;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(AppointmentMappingProfile));

// Register MongoDB client (Aspire will provide connection string via ConnectionStrings:mongodb)
var mongoConnectionString = builder.Configuration.GetConnectionString("mongodb");
var mongoClient = new MongoClient(mongoConnectionString);
builder.Services.AddSingleton<IMongoClient>(mongoClient);

// Register MongoDB context
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var db = client.GetDatabase("polyclinic");
    return new MongoDbContext(db);
});

// Register migrations (must be in order: create collections, indexes, then seed data)
builder.Services.AddSingleton<IMongoMigration, Migration_000_CreateCollections>();
builder.Services.AddSingleton<IMongoMigration, Migration_001_InitIndexes>();
builder.Services.AddSingleton<IMongoMigration, Migration_002_SeedData>();

// Register MigrationRunner
builder.Services.AddSingleton(sp =>
{
    var ctx = sp.GetRequiredService<MongoDbContext>();
    var migrations = sp.GetServices<IMongoMigration>();
    return new MigrationRunner(ctx, migrations);
});

// Register DataSeeder (optional backup seeder, migrations will seed data)
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

builder.Services.AddScoped<IRepository<Doctor, int>, DoctorMongoRepository>();
builder.Services.AddScoped<IRepository<Patient, int>, PatientMongoRepository>();
builder.Services.AddScoped<IRepository<Appointment, int>, AppointmentMongoRepository>();

var app = builder.Build();

// Run migrations (migrations will create indexes and seed initial data)
using (var scope = app.Services.CreateScope())
{
    var migrationRunner = scope.ServiceProvider.GetRequiredService<MigrationRunner>();
    await migrationRunner.RunAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultEndpoints();
app.MapControllers();
app.Run();
