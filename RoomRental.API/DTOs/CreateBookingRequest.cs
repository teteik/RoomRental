namespace RoomRental.API.DTOs;

public class CreateBookingRequest
{
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}