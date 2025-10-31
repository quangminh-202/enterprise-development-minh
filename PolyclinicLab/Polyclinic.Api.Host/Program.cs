using Polyclinic.Infrastructure.InMemory;
using Polyclinic.Domain.Models;
using Polyclinic.Domain.Interfaces;
using Polyclinic.Application.Services;
using Polyclinic.Application.Interfaces;
using Polyclinic.Application.MappingProfiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(AppointmentMappingProfile));

// Register repositories
builder.Services.AddSingleton<IRepository<Doctor, int>, DoctorInMemoryRepository>();
builder.Services.AddSingleton<IRepository<Patient, int>, PatientInMemoryRepository>();
builder.Services.AddSingleton<IRepository<Appointment, int>, AppointmentInMemoryRepository>();

// Register services with their interfaces
builder.Services.AddSingleton<IAppointmentService, AppointmentService>();
builder.Services.AddSingleton<IDoctorService, DoctorService>();
builder.Services.AddSingleton<IPatientService, PatientService>();
builder.Services.AddSingleton<IAnalyticsService, AnalyticsService>();

// Register DataSeeder as singleton
builder.Services.AddSingleton<DataSeeder>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    seeder.Seed();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
