using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using Polyclinic.Domain.Models;

namespace Polyclinic.Infrastructure.Mongo;

/// <summary>
/// Database context for the Polyclinic system using MongoDB with Entity Framework Core.
/// </summary>
public class PolyclinicDbContext : DbContext
{
    public PolyclinicDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>().ToCollection("Doctors");
        modelBuilder.Entity<Patient>().ToCollection("Patients");
        
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToCollection("Appointments");
            entity.Ignore(a => a.Patient);
            entity.Ignore(a => a.Doctor);
        });
    }
}
