using Polyclinic.Domain.Interfaces;
using Polyclinic.Domain.Models;

namespace Polyclinic.Infrastructure.Mongo.Repositories;

/// <summary>
/// EF Core repository for managing patients in MongoDB.
/// </summary>
public class PatientEfRepository(PolyclinicDbContext ctx) : IRepository<Patient, int>
{
    public Patient Create(Patient entity)
    {
        if (entity.Id == 0)
        {
            var lastId = ctx.Patients
                .OrderByDescending(p => p.Id)
                .Select(p => p.Id)
                .FirstOrDefault();

            entity.Id = lastId + 1;
        }

        ctx.Add(entity);
        ctx.SaveChanges();
        return entity;
    }

    public Patient? Read(int id) =>
        ctx.Patients.FirstOrDefault(p => p.Id == id);

    public List<Patient> ReadAll() =>[.. ctx.Patients];

    public Patient Update(Patient entity)
    {
        ctx.Update(entity);
        ctx.SaveChanges();
        return entity;
    }

    public bool Delete(int id)
    {
        var existing = ctx.Patients.FirstOrDefault(p => p.Id == id);
        if (existing is null)
            return false;

        ctx.Remove(existing);
        ctx.SaveChanges();
        return true;
    }
}