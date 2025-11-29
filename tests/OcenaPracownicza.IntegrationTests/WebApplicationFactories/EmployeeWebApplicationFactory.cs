using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OcenaPracownicza.IntegrationTests.WebApplicationFactories
{
    public class EmployeeWebApplicationFactory : BaseWebApplicationFactory
    {
        public EmployeeWebApplicationFactory() : base("EmployeeTestDb")
        {
        }
    }
}
