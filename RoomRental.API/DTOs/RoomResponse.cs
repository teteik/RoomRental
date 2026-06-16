namespace RoomRental.API.DTOs;

public class RoomResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerHour { get; set; }
    public string? Description { get; set; }
}