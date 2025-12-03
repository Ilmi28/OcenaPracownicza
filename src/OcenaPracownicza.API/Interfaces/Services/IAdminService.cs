using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;

namespace OcenaPracownicza.API.Interfaces.Services
{
    public interface IAdminService
    {
        Task<AdminListResponse> GetAll();
        Task<AdminResponse> GetById(Guid id);
        Task<AdminResponse> Add(CreateAdminRequest request);
        Task<AdminResponse> Update(Guid id, UpdateAdminRequest request);
        Task<AdminResponse> Delete(Guid id);
    }
}