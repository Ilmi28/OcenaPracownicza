using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;

namespace OcenaPracownicza.API.Interfaces.Services
{
    public interface IAdminService
    {
        Task<List<AdminResponse>> GetAll();
        Task<AdminResponse> GetById(string id);
        Task<AdminResponse> Add(CreateAdminRequest request);
        Task<AdminResponse> Update(string id, UpdateAdminRequest request);
        Task<AdminResponse> Delete(string id);
    }
}