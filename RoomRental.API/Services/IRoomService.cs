using RoomRental.API.DTOs;

namespace RoomRental.API.Services;

public interface IRoomService
{
    Task<PagedResult<RoomResponse>> GetRoomsAsync(string? search, int? minCapacity, decimal? maxPrice, int pageNumber, int pageSize);
    Task<RoomResponse> GetRoomByIdAsync(Guid id);
    Task<RoomResponse> CreateRoomAsync(CreateRoomRequest request);
    Task<RoomResponse> UpdateRoomAsync(Guid id, UpdateRoomRequest request);
    Task DeleteRoomAsync(Guid id);
    Task<IEnumerable<BookedSlotResponse>> GetBookedSlotsAsync(Guid id, DateTime date);
}