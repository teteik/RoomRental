using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomRental.API.DTOs;
using RoomRental.API.Services;

namespace RoomRental.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetBookings([FromQuery] Guid? clientId = null)
    {
        var response = await _bookingService.GetBookingsAsync(clientId);
    
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingResponse>> GetBooking(Guid id)
    {
        try
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            
            return Ok(booking);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> Post([FromBody] CreateBookingRequest request)
    {
        try
        {
            var booking = await _bookingService.CreateBookingAsync(request);
            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return Conflict(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<BookingResponse>> Cancel(Guid id)
    {
        try
        {
            return await _bookingService.CancelBookingAsync(id);
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
    
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/confirm")]
    public async Task<ActionResult<BookingResponse>> Confirm(Guid id)
    {
        try
        {
            return await _bookingService.ConfirmBookingAsync(id);
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