using MongoDB.Driver;
using Polyclinic.Domain.Interfaces;
using Polyclinic.Domain.Models;
using Polyclinic.Infrastructure.Mongo.Context;

namespace Polyclinic.Infrastructure.Mongo.Repositories;

/// <summary>
/// MongoDB repository implementation for managing doctors.
/// Provides basic CRUD operations for Doctor entities stored in MongoDB.
/// </summary>
public class DoctorMongoRepository(MongoDbContext ctx) : IRepository<Doctor, int>
{
    private readonly IMongoCollection<Doctor> _col = ctx.Doctors;

    public Doctor Create(Doctor entity)
    {
        if (entity.Id == 0)
        {
            var last = _col.Find(FilterDefinition<Doctor>.Empty)
                           .SortByDescending(d => d.Id)
                           .FirstOrDefault();
            entity.Id = (last?.Id ?? 0) + 1;
        }

        _col.InsertOne(entity);
        return entity;
    }

    public Doctor? Read(int id) =>
        _col.Find(d => d.Id == id).FirstOrDefault();

    public List<Doctor> ReadAll() =>
        _col.Find(FilterDefinition<Doctor>.Empty).ToList();

    public Doctor Update(Doctor entity)
    {
        _col.ReplaceOne(d => d.Id == entity.Id, entity, new ReplaceOptions { IsUpsert = false });
        return entity;
    }

    public bool Delete(int id)
    {
        var result = _col.DeleteOne(d => d.Id == id);
        return result.DeletedCount > 0;
    }
}
