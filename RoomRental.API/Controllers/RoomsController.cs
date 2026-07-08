using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.API.DTOs;
using RoomRental.Domain.Entities;
using RoomRental.Domain.Enums;
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

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomResponse>> Get(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            return NotFound();

        return Ok(new RoomResponse
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            PricePerHour = room.PricePerHour,
            Description = room.Description
        });
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

    [HttpPut("{id}")]
    public async Task<ActionResult<RoomResponse>> Put(Guid id,[FromBody] UpdateRoomRequest request)
    {
        try
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return NotFound();

            room.UpdateName(request.Name);
            room.SetCapacity(request.Capacity);
            room.SetPrice(request.PricePerHour);
            if (request.Description != null)
                room.UpdateDescription(request.Description);

            await _context.SaveChangesAsync();

            return Ok(new RoomResponse
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

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            return NotFound();
        
        if (await _context.Bookings.AnyAsync(b => b.RoomId == id))
            return Conflict("Cannot delete room because it has booking history");

        _context.Rooms.Remove(room);
        
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}/schedule")]
    public async Task<ActionResult<IEnumerable<BookedSlotResponse>>> GetSchedule(Guid id, [FromQuery] DateTime date)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            return NotFound("Room not found");
        
        date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        
        var startOfDay = date.Date;
        var endOfDay = date.Date.AddDays(1);

        var bookedSlots = await _context.Bookings
            .Where(b => b.RoomId == id
                        && b.Status != BookingStatus.Cancelled
                        && b.EndTime > startOfDay
                        && b.StartTime < endOfDay)
            .Select(b => new BookedSlotResponse
            {
                StartTime = b.StartTime,
                EndTime = b.EndTime,
            }).ToListAsync();
        
        return Ok(bookedSlots);
    }
}