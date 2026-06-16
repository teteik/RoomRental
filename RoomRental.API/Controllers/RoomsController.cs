using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.API.DTOs;
using RoomRental.Domain.Entities;
using RoomRental.Infrastructure.Data;

namespace RoomRental.API.Controllers;

[ApiController] 
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public RoomsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomResponse>>> Get()
    {
        var rooms = await _context.Rooms.ToListAsync();
        var response = rooms.Select(r => new RoomResponse
        {
            Id = r.Id,
            Name = r.Name,
            Capacity = r.Capacity,
            PricePerHour = r.PricePerHour,
            Description = r.Description
        });
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<RoomResponse>> Post([FromBody] CreateRoomRequest request)
    {
        try
        {
            var room = new Room(Guid.NewGuid(), request.Name, request.Capacity, request.PricePerHour, request.Description);
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            
            return StatusCode(201, new RoomResponse
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                PricePerHour = room.PricePerHour,
                Description = room.Description
            });
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }
}