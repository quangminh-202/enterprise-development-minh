using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using Polyclinic.Domain.Models;

namespace Polyclinic.Infrastructure.Mongo;

/// <summary>
/// Database context for the Polyclinic system using MongoDB with Entity Framework Core.
/// </summary>
public class PolyclinicDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Doctor>().ToCollection("Doctors");
        modelBuilder.Entity<Patient>().ToCollection("Patients");
        modelBuilder.Entity<Appointment>().ToCollection("Appointments");
    }
}
