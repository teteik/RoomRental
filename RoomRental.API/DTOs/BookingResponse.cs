using RoomRental.Domain.Enums;

namespace RoomRental.API.DTOs;

public class BookingResponse
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal Price { get; set; }
    public BookingStatus Status { get; set; }
}