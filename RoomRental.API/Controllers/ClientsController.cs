using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.API.DTOs;
using RoomRental.API.Services;
using RoomRental.Domain.Entities;
using RoomRental.Infrastructure.Data;

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
        var response = await _clientService.GetClients();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientResponse>> Get(Guid id)
    {
        try
        {
            var response = await _clientService.GetClientById(id);
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
            var client = await _clientService.CreateClient(request);
            return StatusCode(201, client);
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
            var client = await _clientService.UpdateClient(id, request);
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
            await _clientService.DeleteClient(id);
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