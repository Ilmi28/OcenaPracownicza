using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;

namespace OcenaPracownicza.API.Interfaces.Services;

public interface IEmployeeService
{
    Task<EmployeeResponse> GetById(Guid id);
    Task<EmployeeListResponse> GetAll();
    Task<EmployeeResponse> Add(CreateEmployeeRequest request);
    Task<EmployeeResponse> Update(Guid id, UpdateEmployeeRequest request);
    Task<EmployeeResponse> Delete(Guid id);
    Task<EmployeeResponse> GetCurrent();
}
