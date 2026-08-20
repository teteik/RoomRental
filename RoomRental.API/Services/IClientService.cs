using RoomRental.API.DTOs;

namespace RoomRental.API.Services;

public interface IClientService
{
    Task<IEnumerable<ClientResponse>> GetClients();
    Task<ClientResponse> GetClientById(Guid id);
    Task<ClientResponse> CreateClient(CreateClientRequest request);
    Task<ClientResponse> UpdateClient(Guid id, UpdateClientRequest request);
    Task DeleteClient(Guid id);
}