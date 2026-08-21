using Microsoft.AspNetCore.Mvc;
using RoomRental.API.DTOs;
using RoomRental.API.Services;

namespace RoomRental.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientResponse>>> Get()
    {
        var response = await _clientService.GetClientsAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientResponse>> Get(Guid id)
    {
        try
        {
            var response = await _clientService.GetClientByIdAsync(id);
            return Ok(response);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ClientResponse>> Post([FromBody] CreateClientRequest request)
    {
        try
        {
            var client = await _clientService.CreateClientAsync(request);
            return CreatedAtAction(nameof(Get), new { id = client.Id }, client);
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
            var client = await _clientService.UpdateClientAsync(id, request);
            return Ok(client);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _clientService.DeleteClientAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return Conflict(e.Message);
        }
    }
}