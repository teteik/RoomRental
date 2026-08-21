using RoomRental.API.DTOs;

namespace RoomRental.API.Services;

public interface IClientService
{
    Task<IEnumerable<ClientResponse>> GetClientsAsync();
    Task<ClientResponse> GetClientByIdAsync(Guid id);
    Task<ClientResponse> CreateClientAsync(CreateClientRequest request);
    Task<ClientResponse> UpdateClientAsync(Guid id, UpdateClientRequest request);
    Task DeleteClientAsync(Guid id);
}