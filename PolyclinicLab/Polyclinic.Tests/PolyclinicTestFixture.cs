using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polyclinic.Infrastructure.Mongo;
using Polyclinic.Infrastructure.Mongo.Repositories;

namespace Polyclinic.Tests;

/// <summary>
/// Base test fixture providing in-memory database and repositories for tests.
/// </summary>
public class PolyclinicTestFixture : IDisposable
{
    protected readonly PolyclinicDbContext Context;
    protected readonly DoctorEfRepository DoctorRepo;
    protected readonly PatientEfRepository PatientRepo;
    protected readonly AppointmentEfRepository AppointmentRepo;
    private readonly ServiceProvider _serviceProvider;

    public PolyclinicTestFixture()
    {
        var services = new ServiceCollection();

        // Register EF Core In-Memory Database
        services.AddDbContext<PolyclinicDbContext>(options =>
        {
            options.UseInMemoryDatabase("PolyclinicTestDb_" + Guid.NewGuid());
            options.EnableSensitiveDataLogging();
        });

        // Register repositories
        services.AddScoped<DoctorEfRepository>();
        services.AddScoped<PatientEfRepository>();
        services.AddScoped<AppointmentEfRepository>();

        _serviceProvider = services.BuildServiceProvider();

        Context = _serviceProvider.GetRequiredService<PolyclinicDbContext>();
        DoctorRepo = _serviceProvider.GetRequiredService<DoctorEfRepository>();
        PatientRepo = _serviceProvider.GetRequiredService<PatientEfRepository>();
        AppointmentRepo = _serviceProvider.GetRequiredService<AppointmentEfRepository>();

        SeedTestData();
    }

    private void SeedTestData()
    {
        Context.Database.EnsureCreated();

        var fixture = new Polyclinic.Domain.Data.PolyclinicFixture();

        Context.Doctors.AddRange(fixture.Doctors);
        Context.Patients.AddRange(fixture.Patients);
        Context.Appointments.AddRange(fixture.Appointments);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context?.Dispose();
        _serviceProvider?.Dispose();
    }
}
