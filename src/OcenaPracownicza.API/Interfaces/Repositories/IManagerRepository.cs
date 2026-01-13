using OcenaPracownicza.API.Entities;

namespace OcenaPracownicza.API.Interfaces.Repositories
{
    public interface IManagerRepository : IBaseRepository<Manager>
    {
        Task<Manager?> GetByUserId(string userId);
    }
}
