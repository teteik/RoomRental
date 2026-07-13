using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.API.DTOs;
using RoomRental.Domain.Entities;
using RoomRental.Domain.Enums;
using RoomRental.Infrastructure.Data;

namespace RoomRental.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public BookingsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetBookings([FromQuery] Guid? clientId = null)
    {
        IQueryable<Booking> query = _context.Bookings;

        if (clientId.HasValue)
        {
            query = query.Where(b => b.ClientId == clientId.Value);
        }

        var bookings = await query.ToListAsync();
    
        var response = new List<BookingResponse>();
        foreach (var booking in bookings)
        {
            response.Add(await MapToResponse(booking));
        }
    
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingResponse>> GetBooking(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if(booking == null)
            return NotFound();
        
        return Ok(await MapToResponse(booking));
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> Post([FromBody] CreateBookingRequest request)
    {
        try
        {
            var room = await _context.Rooms.FindAsync(request.RoomId);
            if(room == null)
                return NotFound("Room not found");
            
            var client = await _context.Clients.FindAsync(request.ClientId);
            if(client == null)
                return NotFound("Client not found");
            var hasOverlap = await _context.Bookings
                .AnyAsync(b => b.RoomId == request.RoomId
                               && b.Status != BookingStatus.Cancelled
                               && b.StartTime < request.EndTime 
                               && b.EndTime > request.StartTime);
            
            if (hasOverlap)
                return Conflict("Room is already booked for this time period");
            
            var hours = (decimal) (request.EndTime - request.StartTime).TotalMinutes / 60m;
            var price = hours * room.PricePerHour;

            var booking = Booking.Create 
            (
                Guid.NewGuid(),
                request.RoomId,
                request.ClientId,
                request.StartTime,
                request.EndTime,
                price
            );
            
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return StatusCode(201, await MapToResponse(booking));
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<BookingResponse>> Cancel(Guid id)
    {
        return await ActionBooking(id, booking => booking.Cancel());
    }
    
    [HttpPost("{id}/confirm")]
    public async Task<ActionResult<BookingResponse>> Confirm(Guid id)
    {
        return await ActionBooking(id, booking => booking.Confirm());
    }

    private async Task<ActionResult<BookingResponse>> ActionBooking(Guid id, Action<Booking> action)
    {
        try
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();
        
            action(booking);
            await _context.SaveChangesAsync();
            return Ok(await MapToResponse(booking));
        }
        catch (InvalidOperationException e)
        {
            return Conflict(e.Message);
        }
    }

    private async Task<BookingResponse> MapToResponse(Booking booking)
    {
        var room = await _context.Rooms.FindAsync(booking.RoomId);
        return new BookingResponse
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            RoomName = room?.Name ?? "Неизвестная комната",
            ClientId = booking.ClientId,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Price = booking.Price,
            Status = booking.Status
        };
    }
}