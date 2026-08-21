using RoomRental.API.DTOs;

namespace RoomRental.API.Services;

public interface IImageService
{
    Task<RoomResponse> AddImagesToRoom(Guid id, IFormFileCollection files);
    Task UpdateImagesOrder(Guid id, List<UpdateImageOrderRequest> request);
    Task DeleteImagesFromRoom(Guid roomId, Guid id);
}