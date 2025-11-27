using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OcenaPracownicza.API.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeResponse>> GetAll();
        Task<EmployeeResponse> GetById(int id);
        Task<EmployeeResponse> Create(CreateEmployeeRequest request);
        Task<EmployeeResponse> Update(int id, UpdateEmployeeRequest request);
        Task<DeleteEmployeeResponse> Delete(int id);
    }
}
