using RoomRental.API.DTOs;

namespace RoomRental.API.Services;

public interface IImageService
{
    Task<RoomResponse> AddImagesToRoomAsync(Guid id, IFormFileCollection files);
    Task UpdateImagesOrderAsync(Guid id, List<UpdateImageOrderRequest> request);
    Task DeleteImagesFromRoomAsync(Guid roomId, Guid id);
}