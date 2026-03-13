using OcenaPracownicza.API.Entities;

namespace OcenaPracownicza.API.Interfaces.Repositories;

public interface IAdminRepository : IBaseRepository<Admin>
{
    Task<Admin?> GetByUserId(string userId);
}
