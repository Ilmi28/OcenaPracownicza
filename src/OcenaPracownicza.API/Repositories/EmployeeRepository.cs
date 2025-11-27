using OcenaPracownicza.API.Data; // lub gdzie masz ApplicationDbContext
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OcenaPracownicza.API.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAll()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee> GetById(int id)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task Add(Employee entity)
        {
            _context.Employees.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Employee entity)
        {
            _context.Employees.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var e = await GetById(id);
            if (e != null)
            {
                _context.Employees.Remove(e);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Employees.AnyAsync(e => e.Id == id);
        }
    }
}
