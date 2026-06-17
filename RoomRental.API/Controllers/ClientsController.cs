using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.API.DTOs;
using RoomRental.Domain.Entities;
using RoomRental.Infrastructure.Data;

namespace RoomRental.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientResponse>>> Get()
    {
        var clients = await _context.Clients.ToListAsync();
        var response = clients.Select(c => new ClientResponse
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber
            }
        );
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ClientResponse>> Post([FromBody] CreateClientRequest request)
    {
        try
        {
            var client = new Client(Guid.NewGuid(), request.FullName, request.Email, request.PhoneNumber);
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return StatusCode(201, new ClientResponse
            {
                Id = client.Id,
                FullName = client.FullName,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber
            });
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}")]

    public async Task<ActionResult<ClientResponse>> Put(Guid id, [FromBody] UpdateClientRequest request)
    {
        try
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound();
            
            client.UpdateFullName(request.FullName);
            client.UpdateEmail(request.Email);
            client.UpdatePhone(request.PhoneNumber);
            
            await _context.SaveChangesAsync();
            
            return Ok(new ClientResponse
            {
                Id = client.Id,
                FullName = client.FullName,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber
            });
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            return NotFound();
        
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}