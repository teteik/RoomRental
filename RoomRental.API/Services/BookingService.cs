using Microsoft.EntityFrameworkCore;
using RoomRental.API.DTOs;
using RoomRental.Domain.Entities;
using RoomRental.Domain.Enums;
using RoomRental.Infrastructure.Data;

namespace RoomRental.API.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _context;
    
    public BookingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookingResponse>> GetBookingsAsync(Guid? clientId)
    {
        IQueryable<Booking> query = _context.Bookings
            .Include(b => b.Room);

        if (clientId != null)
            query = query.Where(b => b.ClientId == clientId.Value);

        var bookings = await query.ToListAsync();
        
        return bookings.Select(MapToResponse).ToList();
    }
    
    public async Task<BookingResponse> GetBookingByIdAsync(Guid id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);
        if(booking == null)
            throw new KeyNotFoundException("Booking not found");
        
        return MapToResponse(booking);
    }

    public async Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var room = await _context.Rooms.FindAsync(request.RoomId);
        if(room == null)
            throw new KeyNotFoundException("Room not found");
            
        var client = await _context.Clients.FindAsync(request.ClientId);
        if(client == null)
            throw new KeyNotFoundException("Client not found");
        
        var hasOverlap = await _context.Bookings
            .AnyAsync(b => b.RoomId == request.RoomId
                           && b.Status != BookingStatus.Cancelled
                           && b.StartTime < request.EndTime 
                           && b.EndTime > request.StartTime);
            
        if (hasOverlap)
            throw new InvalidOperationException("Room is already booked for this time period");
            
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
        
        var savedBooking = await _context.Bookings
            .Include(b => b.Room)
            .FirstAsync(b => b.Id == booking.Id);

        return MapToResponse(savedBooking);
    }

    public async Task<BookingResponse> CancelBookingAsync(Guid id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (booking == null)
            throw new KeyNotFoundException("Booking not found");
    
        booking.Cancel();
        await _context.SaveChangesAsync();
        
        return MapToResponse(booking);
    }

    public async Task<BookingResponse> ConfirmBookingAsync(Guid id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (booking == null)
            throw new KeyNotFoundException("Booking not found");
    
        booking.Confirm();
        await _context.SaveChangesAsync();
        
        return MapToResponse(booking);
    }
    
    private BookingResponse MapToResponse(Booking booking)
    {
        return new BookingResponse
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            RoomName = booking.Room?.Name ?? "Неизвестная комната",
            ClientId = booking.ClientId,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Price = booking.Price,
            Status = booking.Status
        };
    }
}