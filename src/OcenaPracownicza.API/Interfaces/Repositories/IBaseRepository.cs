namespace OcenaPracownicza.API.Interfaces.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<TEntity> Create(TEntity entity);
    Task<TEntity?> GetById(int id);
    Task<IEnumerable<TEntity>> GetAll();
    Task<TEntity> Update(TEntity entity);
    Task Delete(int id);
    Task<bool> Exists(int id);
    Task<int> Count();
}