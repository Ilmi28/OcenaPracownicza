using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OcenaPracownicza.IntegrationTests.WebApplicationFactories
{
    public class ExampleWebApplicationFactory : BaseWebApplicationFactory
    {
        public ExampleWebApplicationFactory() : base("ExampleTestDb")
        {
        }
    }
}
