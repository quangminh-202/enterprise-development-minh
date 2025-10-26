namespace Polyclinic.Domain.Interfaces;

/// <summary>
/// Generic repository interface defining basic CRUD operations 
/// for working with entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type of the entity.</typeparam>
/// <typeparam name="TId">Type of the entity identifier.</typeparam>
public interface IRepository<T, TId>
{
    /// <summary>Creates a new entity and returns the created entity with its assigned identifier.</summary>
    T Create(T entity);

    /// <summary>Finds an entity by its identifier.</summary>
    T? Read(TId id);

    /// <summary>Returns all entities.</summary>
    List<T> ReadAll();

    /// <summary>Updates an existing entity and returns the updated entity.</summary>
    T Update(T entity);

    /// <summary>Deletes an entity by its identifier. Returns true if deletion was successful, false if entity was not found.</summary>
    bool Delete(TId id);
}
