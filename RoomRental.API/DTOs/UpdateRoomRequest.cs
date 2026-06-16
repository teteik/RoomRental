namespace RoomRental.API.DTOs;

public class UpdateRoomRequest
{
    public required string Name { get; set; } 
    public int Capacity { get; set; }
    public decimal PricePerHour { get; set; }
    public string? Description { get; set; }
}