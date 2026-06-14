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
    public BookingStatus Status { get; private set; } = BookingStatus.Pending;

    public Booking(Guid id, Guid roomId, Guid clientId, DateTime startTime, DateTime endTime, decimal price)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time");
        if (startTime < DateTime.Now)
            throw new ArgumentException("Start time must be after now");
        if (startTime.Minute != 0 || startTime.Second != 0 || startTime.Millisecond != 0)
            throw new ArgumentException("Start must be exactly on the hour");
        if (endTime.Minute != 0 || endTime.Second != 0 || endTime.Millisecond != 0)
            throw new ArgumentException("End must be exactly on the hour");

        var interval = endTime - startTime;
        if (interval.TotalHours < 1)
            throw new ArgumentException("Minimum rental time is 1 hour");

        StartTime = startTime;
        EndTime = endTime;

        Id = id;
        RoomId = roomId;
        ClientId = clientId;
        Price = price >= 0 ? price : throw new ArgumentException("Price must be positive");
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Booking status must be pending");
        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Booking has already been cancelled");
        if (Status == BookingStatus.Completed)
            throw new InvalidOperationException("You cannot cancel a completed booking");
        var timeUntilStart = StartTime - DateTime.Now;
        if (timeUntilStart.TotalHours < 72)
            throw new InvalidOperationException("Cancellation is possible no later than 3 days (72 hours) before the start of the rental");
        
        Status = BookingStatus.Cancelled;
    }
}