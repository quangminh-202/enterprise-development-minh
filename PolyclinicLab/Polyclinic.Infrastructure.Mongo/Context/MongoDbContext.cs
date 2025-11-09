using MongoDB.Driver;
using Polyclinic.Domain.Models;

namespace Polyclinic.Infrastructure.Mongo.Context;

/// <summary>
/// MongoDB database context that provides access to collections for Doctors, Patients, and Appointments.
/// This class encapsulates the MongoDB database connection and provides typed collection accessors.
/// </summary>
public class MongoDbContext(IMongoDatabase db)
{
    private readonly IMongoDatabase _db = db;

    public IMongoDatabase Database => _db;

    public IMongoCollection<Doctor> Doctors => _db.GetCollection<Doctor>("Doctors");
    public IMongoCollection<Patient> Patients => _db.GetCollection<Patient>("Patients");
    public IMongoCollection<Appointment> Appointments => _db.GetCollection<Appointment>("Appointments");
}
