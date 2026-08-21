using Microsoft.EntityFrameworkCore;
using RoomRental.API.DTOs;
using RoomRental.Domain.Entities;
using RoomRental.Infrastructure.Data;

namespace RoomRental.API.Services;

public class ClientService : IClientService
{
    private readonly AppDbContext _context;
    
    public ClientService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<ClientResponse>> GetClientsAsync()
    {
        var clients = await _context.Clients.ToListAsync();
        var response = clients.Select(MapToResponse);

        return response;
    }

    public async Task<ClientResponse> GetClientByIdAsync(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            throw new KeyNotFoundException($"Client with id {id} not found");

        return MapToResponse(client);
    }

    public async Task<ClientResponse> CreateClientAsync(CreateClientRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var client = new Client(Guid.NewGuid(), request.FullName, request.Email, request.PhoneNumber);
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return MapToResponse(client);
    }

    public async Task<ClientResponse> UpdateClientAsync(Guid id, UpdateClientRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            throw new KeyNotFoundException($"Client with id {id} not found");
        
        client.UpdateFullName(request.FullName);
        client.UpdateEmail(request.Email);
        client.UpdatePhone(request.PhoneNumber);
        
        await _context.SaveChangesAsync();

        return MapToResponse(client);
    }

    public async Task DeleteClientAsync(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            throw new KeyNotFoundException($"Client with id {id} not found");
        
        if (await _context.Bookings.AnyAsync(b => b.ClientId == id))
            throw new InvalidOperationException("Cannot delete client because they have booking history");
        
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
    }

    private ClientResponse MapToResponse(Client client)
    {
        return new ClientResponse
        {
            Id = client.Id,
            FullName = client.FullName,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber
        };
    }
}