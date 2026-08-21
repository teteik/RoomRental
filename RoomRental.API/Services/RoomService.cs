using Microsoft.EntityFrameworkCore;
using RoomRental.API.DTOs;
using RoomRental.Domain.Entities;
using RoomRental.Domain.Enums;
using RoomRental.Infrastructure.Data;

namespace RoomRental.API.Services;

public class RoomService : IRoomService
{
    private readonly AppDbContext _context;

    public RoomService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<RoomResponse>> GetRooms(string? search, int? minCapacity, decimal? maxPrice, int pageNumber, int pageSize)
    {
        var query = _context.Rooms.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowerSearch = search.ToLower();
            query = query.Where(r => r.Name.ToLower().Contains(lowerSearch));
        }

        if (minCapacity != null)
            query = query.Where(r => r.Capacity >= minCapacity.Value);

        if (maxPrice != null)
            query = query.Where(r => r.PricePerHour <= maxPrice.Value);


        var totalCount = await query.CountAsync();
        
        var pagedRooms = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(r => r.Images)
            .ToListAsync();
        
        var responseItems = pagedRooms.Select(RoomMapper.ToResponse).ToList();

        return new PagedResult<RoomResponse> { TotalCount = totalCount, Items = responseItems };
    }
    
    public async Task<RoomResponse> GetRoomById(Guid id)
    {
        var room = await _context.Rooms
            .Include(r => r.Images) 
            .FirstOrDefaultAsync(r => r.Id == id);
            
        if (room == null)
            throw new KeyNotFoundException($"Room with id {id} not found");
        
        return  RoomMapper.ToResponse(room);
    }

    public async Task<RoomResponse> CreateRoom(CreateRoomRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var room = new Room(Guid.NewGuid(), request.Name, request.Capacity, request.PricePerHour, request.Description);
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        
        return RoomMapper.ToResponse(room);
    }

    public async Task<RoomResponse> UpdateRoom(Guid id, UpdateRoomRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var room = await _context.Rooms
            .Include(r => r.Images)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (room == null)
            throw new KeyNotFoundException($"Room with id {id} not found");

        room.UpdateName(request.Name);
        room.SetCapacity(request.Capacity);
        room.SetPrice(request.PricePerHour);
        if (request.Description != null)
            room.UpdateDescription(request.Description);

        await _context.SaveChangesAsync();
        
        return  RoomMapper.ToResponse(room);
    }

    public async Task DeleteRoom(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);
        
        if (room == null)
            throw new  KeyNotFoundException($"Room with id {id} not found");
        
        if (await _context.Bookings.AnyAsync(b => b.RoomId == id))
            throw new InvalidOperationException("Cannot delete room because it has booking history");

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<BookedSlotResponse>> GetBookedSlots(Guid id, DateTime date)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            throw new  KeyNotFoundException($"Room with id {id} not found");
        
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
        
        return bookedSlots;
    }
}