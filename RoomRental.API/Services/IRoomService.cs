using RoomRental.API.DTOs;

namespace RoomRental.API.Services;

public interface IRoomService
{
    Task<PagedResult<RoomResponse>> GetRooms(string? search, int? minCapacity, decimal? maxPrice, int pageNumber, int pageSize);
    Task<RoomResponse> GetRoomById(Guid id);
    Task<RoomResponse> CreateRoom(CreateRoomRequest request);
    Task<RoomResponse> UpdateRoom(Guid id, UpdateRoomRequest request);
    Task DeleteRoom(Guid id);
    Task<IEnumerable<BookedSlotResponse>> GetBookedSlots(Guid id, DateTime date);
}