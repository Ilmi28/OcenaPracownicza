using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;

namespace OcenaPracownicza.API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        // ------------------------------------------------------------
        // CREATE
        // ------------------------------------------------------------
        public async Task<EmployeeResponse> Create(CreateEmployeeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
                throw new ArgumentException("FirstName is required");

            var entity = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Position = request.Position,
                Unit = request.Unit,
                AcademicTitle = request.AcademicTitle,

                // Wartości domyślne — zgodnie z Employee.cs (required string)
                Period = "",
                FinalScore = "",
                AchievementsSummary = ""
            };

            await _repository.Add(entity);

            return new EmployeeResponse(entity);
        }

        // ------------------------------------------------------------
        // GET BY ID
        // ------------------------------------------------------------
        public async Task<EmployeeResponse> GetById(int id)
        {
            var employee = await _repository.GetById(id);

            if (employee == null)
                throw new Exception($"Employee with id {id} not found.");

            return new EmployeeResponse(employee);
        }

        // ------------------------------------------------------------
        // GET ALL
        // ------------------------------------------------------------
        public async Task<IEnumerable<EmployeeResponse>> GetAll()
        {
            var employees = await _repository.GetAll();

            return employees.Select(e => new EmployeeResponse(e));
        }

        // ------------------------------------------------------------
        // UPDATE
        // ------------------------------------------------------------
        public async Task<EmployeeResponse> Update(int id, UpdateEmployeeRequest request)
        {
            var employee = await _repository.GetById(id);

            if (employee == null)
                throw new Exception($"Employee with id {id} not found.");

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Position = request.Position;
            employee.Unit = request.Unit;
            employee.AcademicTitle = request.AcademicTitle;

            await _repository.Update(employee);

            return new EmployeeResponse(employee);
        }

        // ------------------------------------------------------------
        // DELETE
        // ------------------------------------------------------------
        public async Task<DeleteEmployeeResponse> Delete(int id)
        {
            var employee = await _repository.GetById(id);

            if (employee == null)
                throw new Exception($"Employee with id {id} not found.");

            await _repository.Delete(id);

            return new DeleteEmployeeResponse();
        }
    }
}
