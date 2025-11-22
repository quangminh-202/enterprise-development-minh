using Polyclinic.Domain.Models;
using Polyclinic.Domain.Interfaces;
using Polyclinic.Application.Services;
using Polyclinic.Application.Interfaces;
using Polyclinic.Application.MappingProfiles;
using Polyclinic.Infrastructure.Mongo.Repositories;
using Polyclinic.Infrastructure.Mongo.Migrations;
using Polyclinic.ServiceDefaults;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;
using Polyclinic.Infrastructure.Mongo;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(AppointmentMappingProfile));

// Register MongoDB Client - support both Aspire and standalone
builder.AddMongoDBClient("polyclinic");

// Register DbContext with EF Core
builder.Services.AddDbContext<PolyclinicDbContext>((serviceProvider, options) =>
{
    var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
    options.UseMongoDB(mongoClient, "polyclinic");
});

// Register Application Services
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

// Register Repositories
builder.Services.AddScoped<IRepository<Doctor, int>, DoctorEfRepository>();
builder.Services.AddScoped<IRepository<Patient, int>, PatientEfRepository>();
builder.Services.AddScoped<IRepository<Appointment, int>, AppointmentEfRepository>();

// Register Migrations
builder.Services.AddTransient<IMongoMigration, Migration_000_CreateCollections>();
builder.Services.AddTransient<IMongoMigration, Migration_001_InitIndexes>();
builder.Services.AddTransient<IMongoMigration, Migration_002_SeedData>();
builder.Services.AddScoped<MigrationRunner>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PolyclinicDbContext>();
    dbContext.Database.AutoTransactionBehavior = Microsoft.EntityFrameworkCore.AutoTransactionBehavior.Never;
    
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
