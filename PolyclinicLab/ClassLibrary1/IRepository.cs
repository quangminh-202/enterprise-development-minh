namespace Polyclinic.Infrastructure.InMemory;

public interface IRepository<T, TId>
{
    void Create(T entity);
    T? Read(TId id);
    List<T> ReadAll();
    void Update(T entity);
    void Delete(TId id);
}
