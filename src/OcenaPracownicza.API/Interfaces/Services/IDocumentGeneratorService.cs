using OcenaPracownicza.API.Entities;

namespace Ocenapracownicza.API.Services
{
    public interface IDocumentGeneratorService
    {
        byte[] GenerateReport(Employee employee);
    }
}
