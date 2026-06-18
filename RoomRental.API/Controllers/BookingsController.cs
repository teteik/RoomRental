using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.API.DTOs;
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
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetBookings()
    {
        var bookings = await _context.Bookings.ToListAsync();
        var response = bookings.Select(b => new BookingResponse
        {
            Id = b.Id,
            RoomId = b.RoomId,
            ClientId = b.ClientId,
            StartTime = b.StartTime,
            EndTime = b.EndTime,
            Price = b.Price,
            Status = b.Status
        });
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingResponse>> GetBooking(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if(booking == null)
            return NotFound();
        
        return Ok(new BookingResponse
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            ClientId = booking.ClientId,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Price = booking.Price,
            Status = booking.Status
        });
    }
}