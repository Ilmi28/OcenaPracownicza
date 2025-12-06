using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;

namespace OcenaPracownicza.API.Interfaces.Services;

public interface IManagerService
{
    Task<ManagerResponse> GetById(Guid id);
    Task<ManagerListResponse> GetAll();
    Task<ManagerResponse> Add(CreateManagerRequest request);
    Task<ManagerResponse> Update(Guid id, UpdateManagerRequest request);
    Task<ManagerResponse> Delete(Guid id);
}
