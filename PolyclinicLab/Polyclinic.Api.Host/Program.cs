using Polyclinic.Infrastructure.InMemory;
using Polyclinic.Domain.Models;
using Polyclinic.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IRepository<Doctor, int>, DoctorInMemoryRepository>();
builder.Services.AddSingleton<IRepository<Patient, int>, PatientInMemoryRepository>();
builder.Services.AddSingleton<IRepository<Appointment, int>, AppointmentInMemoryRepository>();
builder.Services.AddSingleton<DoctorService>();
builder.Services.AddSingleton<PatientService>();
builder.Services.AddSingleton<AppointmentService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var doctorRepo = scope.ServiceProvider.GetRequiredService<IRepository<Doctor, int>>();
    var patientRepo = scope.ServiceProvider.GetRequiredService<IRepository<Patient, int>>();
    var appointmentRepo = scope.ServiceProvider.GetRequiredService<IRepository<Appointment, int>>();

    DataSeeder.Seed(doctorRepo, patientRepo, appointmentRepo);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
