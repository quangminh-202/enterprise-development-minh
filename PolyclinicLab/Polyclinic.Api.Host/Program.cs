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
using Polyclinic.Infrastructure.Nats;
using Polyclinic.Validator.Nats;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(AppointmentMappingProfile));

// Add CORS for Client WASM
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// MongoDB configuration (without Aspire client to avoid version conflict)
var mongoConnectionString = builder.Configuration.GetConnectionString("polyclinic") 
    ?? "mongodb://localhost:27017";

builder.Services.AddDbContext<PolyclinicDbContext>(options =>
    options.UseMongoDB(mongoConnectionString, "polyclinic"));

builder.Services.AddSingleton<IMongoClient>(_ => 
    new MongoClient(mongoConnectionString));

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

// Register NATS services
builder.AddNatsClient("polyclinic-nats");
builder.Services.AddHostedService<PolyclinicNatsConsumer>();
builder.Services.AddHostedService<AppointmentValidatorService>();

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

app.UseCors();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapDefaultEndpoints();
app.MapControllers();
app.Run();
