using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;

namespace OcenaPracownicza.API.Interfaces.Services;

public interface IEmployeeService
{
    Task<EmployeeResponse> GetById(int id);
    Task<EmployeeListResponse> GetAll();
    Task<EmployeeResponse> Add(EmployeeRequest request);
    Task<EmployeeResponse> Update(int id, EmployeeRequest request);
    Task<EmployeeResponse> Delete(int id);
}
