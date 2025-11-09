using MongoDB.Driver;
using Polyclinic.Domain.Interfaces;
using Polyclinic.Domain.Models;
using Polyclinic.Infrastructure.Mongo.Context;

namespace Polyclinic.Infrastructure.Mongo.Repositories;

/// <summary>
/// MongoDB repository implementation for managing patients.
/// Provides basic CRUD operations for Patient entities stored in MongoDB.
/// </summary>
public class PatientMongoRepository(MongoDbContext ctx) : IRepository<Patient, int>
{
    private readonly IMongoCollection<Patient> _col = ctx.Patients;

    public Patient Create(Patient entity)
    {
        if (entity.Id == 0)
        {
            var last = _col.Find(FilterDefinition<Patient>.Empty)
                           .SortByDescending(p => p.Id)
                           .FirstOrDefault();
            entity.Id = (last?.Id ?? 0) + 1;
        }

        _col.InsertOne(entity);
        return entity;
    }

    public Patient? Read(int id) =>
        _col.Find(d => d.Id == id).FirstOrDefault();

    public List<Patient> ReadAll() =>
        _col.Find(FilterDefinition<Patient>.Empty).ToList();

    public Patient Update(Patient entity)
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
