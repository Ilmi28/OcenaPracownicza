using OcenaPracownicza.API.Dtos;

namespace OcenaPracownicza.API.Interfaces.Services
{
    public interface IEmployeeService
    {
        EmployeeProfileDto GetProfile(int employeeId);
    }
}