using RoomRental.Domain.Enums;

namespace RoomRental.Domain.Entities;

public class Booking
{
    public const int MinHoursBeforeCancellation = 72;
    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public Guid ClientId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public decimal Price { get; private set; }
    public BookingStatus Status { get; private set; } = BookingStatus.Pending;
    
    protected Booking() { }
    
    public static Booking Create(Guid id, Guid roomId, Guid clientId, DateTime startTime, DateTime endTime, decimal price)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time");
        if (startTime < DateTime.UtcNow)
            throw new ArgumentException("Start time must be after now");
        if (startTime.Minute != 0 || startTime.Second != 0 || startTime.Millisecond != 0)
            throw new ArgumentException("Start must be exactly on the hour");
        if (endTime.Minute != 0 || endTime.Second != 0 || endTime.Millisecond != 0)
            throw new ArgumentException("End must be exactly on the hour");

        var interval = endTime - startTime;
        if (interval.TotalHours < 1)
            throw new ArgumentException("Minimum rental time is 1 hour");
        
        return new Booking
        {
            Id = id,
            RoomId = roomId,
            ClientId = clientId,
            Price = price >= 0 ? price : throw new ArgumentException("Price must be positive"),
            StartTime = startTime,
            EndTime = endTime
        };
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
        if (StartTime < DateTime.UtcNow)
            throw new InvalidOperationException("Cannot cancel a booking that has already started");
        var timeUntilStart = StartTime - DateTime.UtcNow;
        if (timeUntilStart.TotalHours < MinHoursBeforeCancellation)
            throw new InvalidOperationException($"Cancellation is possible no later than {MinHoursBeforeCancellation} hours before the start of the rental");
        
        Status = BookingStatus.Cancelled;
    }
}