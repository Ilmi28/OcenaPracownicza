using OcenaPracownicza.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OcenaPracownicza.API.Interfaces.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAll();
        Task<Employee> GetById(int id);
        Task Add(Employee entity);
        Task Update(Employee entity);
        Task Delete(int id);
        Task<bool> Exists(int id);
    }
}
