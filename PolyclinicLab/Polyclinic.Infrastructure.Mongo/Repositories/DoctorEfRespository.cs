using Polyclinic.Domain.Interfaces;
using Polyclinic.Domain.Models;

namespace Polyclinic.Infrastructure.Mongo.Repositories;

/// <summary>
/// EF Core repository for managing doctors in MongoDB.
/// </summary>
public class DoctorEfRepository(PolyclinicDbContext ctx) : IRepository<Doctor, int>
{
    public Doctor Create(Doctor entity)
    {
        if (entity.Id == 0)
        {
            var lastId = ctx.Doctors
                .OrderByDescending(d => d.Id)
                .Select(d => d.Id)
                .FirstOrDefault();

            entity.Id = lastId + 1;
        }

        ctx.Doctors.Add(entity);
        ctx.SaveChanges();
        return entity;
    }

    public Doctor? Read(int id) =>
        ctx.Doctors.FirstOrDefault(d => d.Id == id);

    public List<Doctor> ReadAll() =>
        ctx.Doctors.ToList();

    public Doctor Update(Doctor entity)
    {
        ctx.Doctors.Update(entity);
        ctx.SaveChanges();
        return entity;
    }

    public bool Delete(int id)
    {
        var entity = ctx.Doctors.FirstOrDefault(d => d.Id == id);
        if (entity == null)
            return false;

        ctx.Doctors.Remove(entity);
        ctx.SaveChanges();
        return true;
    }
}