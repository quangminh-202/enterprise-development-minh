namespace Polyclinic.Application.Interfaces;

/// <summary>
/// Generic interface for application services providing basic CRUD operations.
/// This interface abstracts common data access patterns and allows for
/// different implementations (in-memory, database, etc.).
/// </summary>
/// <typeparam name="TDto">The DTO type for data transfer (read operations)</typeparam>
/// <typeparam name="TCreateUpdateDto">The DTO type for create/update operations</typeparam>
/// <typeparam name="TKey">The type of the entity's primary key</typeparam>
public interface IApplicationService<TDto, TCreateUpdateDto, TKey> where TKey : struct
{
    /// <summary>
    /// Retrieves all entities as DTOs.
    /// </summary>
    /// <returns>List of all entities as DTOs</returns>
    public List<TDto> GetAll();

    /// <summary>
    /// Retrieves a specific entity by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the entity</param>
    /// <returns>The entity as DTO, or null if not found</returns>
    public TDto? Get(TKey id);

    /// <summary>
    /// Creates a new entity from the provided DTO and returns the created entity as DTO.
    /// </summary>
    /// <param name="dto">The DTO containing entity data for creation</param>
    /// <returns>The created entity as DTO</returns>
    public TDto Create(TCreateUpdateDto dto);

    /// <summary>
    /// Updates an existing entity with data from the provided DTO and returns the updated entity as DTO.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to update</param>
    /// <param name="dto">The DTO containing updated entity data</param>
    /// <returns>The updated entity as DTO</returns>
    public TDto Update(TKey id, TCreateUpdateDto dto);

    /// <summary>
    /// Deletes an entity by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete</param>
    /// <returns>True if deletion was successful, false if entity was not found</returns>
    public bool Delete(TKey id);
}