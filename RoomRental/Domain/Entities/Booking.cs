using RoomRental.Domain.Enums;

namespace RoomRental.Domain.Entities;

public class Booking
{
    public Guid Id { get; init; }
    public Guid RoomId { get; init; }
    public Guid ClientId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public decimal Price { get; init; }
    public BookingStatus Status { get; private set; } =  BookingStatus.Pending;

}