using OcenaPracownicza.API.Entities;

namespace OcenaPracownicza.API.Interfaces.Services
{
    public interface IDocumentGeneratorService
    {
        byte[] GenerateReport(Employee employee);
    }
}
