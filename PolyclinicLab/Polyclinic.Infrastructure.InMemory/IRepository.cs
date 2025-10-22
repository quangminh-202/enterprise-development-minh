namespace Polyclinic.Infrastructure.InMemory;

/// <summary>
/// Generic repository interface defining basic CRUD operations 
/// for working with entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type of the entity.</typeparam>
/// <typeparam name="TId">Type of the entity identifier.</typeparam>
public interface IRepository<T, TId>
{
    /// <summary>Creates a new entity.</summary>
    void Create(T entity);

    /// <summary>Finds an entity by its identifier.</summary>
    T? Read(TId id);

    /// <summary>Returns all entities.</summary>
    List<T> ReadAll();

    /// <summary>Updates an existing entity.</summary>
    void Update(T entity);

    /// <summary>Deletes an entity by its identifier.</summary>
    void Delete(TId id);
}
