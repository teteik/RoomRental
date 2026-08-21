using RoomRental.API.DTOs;
using RoomRental.Domain.Entities;

namespace RoomRental.API.Services;

public class RoomMapper
{
    public static RoomResponse ToResponse(Room room)
    {
        return new RoomResponse
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            PricePerHour = room.PricePerHour,
            Description = room.Description,
            Images = room.Images
                .OrderBy(i => i.Order)
                .Select(i => new RoomImageResponse 
                { 
                    Id = i.Id,       
                    ImageUrl = i.ImageUrl 
                })
                .ToList()
        };
    }
}