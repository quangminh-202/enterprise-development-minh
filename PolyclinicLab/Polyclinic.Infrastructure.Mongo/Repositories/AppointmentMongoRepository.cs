using MongoDB.Driver;
using Polyclinic.Domain.Interfaces;
using Polyclinic.Domain.Models;
using Polyclinic.Infrastructure.Mongo.Context;

namespace Polyclinic.Infrastructure.Mongo.Repositories;

/// <summary>
/// MongoDB repository implementation for managing appointments.
/// Provides basic CRUD operations.
/// </summary>
public class AppointmentMongoRepository(MongoDbContext ctx) : IRepository<Appointment, int>
{
    private readonly IMongoCollection<Appointment> _col = ctx.Appointments;

    public Appointment Create(Appointment entity)
    {
        if (entity.Id == 0)
        {
            var last = _col.Find(FilterDefinition<Appointment>.Empty)
                           .SortByDescending(a => a.Id)
                           .Limit(1)
                           .FirstOrDefault();
            entity.Id = last == null ? 1 : last.Id + 1;
        }

        _col.InsertOne(entity);
        return entity;
    }

    public Appointment? Read(int id) =>
        _col.Find(a => a.Id == id).FirstOrDefault();

    public List<Appointment> ReadAll() =>
        _col.Find(FilterDefinition<Appointment>.Empty).ToList();

    public Appointment Update(Appointment entity)
    {
        _col.ReplaceOne(a => a.Id == entity.Id, entity, new ReplaceOptions { IsUpsert = false });
        return entity;
    }

    public bool Delete(int id) =>
        _col.DeleteOne(a => a.Id == id).DeletedCount > 0;
}
