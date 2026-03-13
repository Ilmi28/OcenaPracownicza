namespace OcenaPracownicza.API.Interfaces.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<TEntity> Create(TEntity entity);
    Task<TEntity?> GetById(Guid id);
    Task<IEnumerable<TEntity>> GetAll();
    Task<TEntity> Update(TEntity entity);
    Task Delete(Guid id);
    Task<bool> Exists(Guid id);
    Task<int> Count();
}