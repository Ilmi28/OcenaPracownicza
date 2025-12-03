using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;

namespace OcenaPracownicza.API.Interfaces.Services;

public interface IAdminService
{
    Task<AdminResponse> GetById(string id);
    Task<List<AdminResponse>> GetAll();
    Task<AdminResponse> Add(CreateAdminRequest request);
    Task<AdminResponse> Update(string id, UpdateAdminRequest request);
    Task Delete(string id);
}