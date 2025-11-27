using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Dtos;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Services;

namespace OcenaPracownicza.API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public EmployeeProfileDto GetProfile(int employeeId)
        {
            var employee = _context.Employees
                .FirstOrDefault(e => e.Id == employeeId);

            if (employee == null) return null;

            return new EmployeeProfileDto
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Position = employee.Position,
                Unit = employee.Unit,
                AcademicTitle = employee.AcademicTitle
            };
        }
    }
}