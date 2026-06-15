namespace RoomRental.Domain.Entities;

public class Room
{   
    public const int MaxNameLength = 100;
    public const int MaxDescriptionLength = 1000;
    public Guid Id { get; init; }
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }
    public decimal PricePerHour { get; private set; }
    public string? Description { get; private set; } 
    public ICollection<Booking> Bookings { get; private set; } = new List<Booking>();
    
    public Room(Guid id, string name, int capacity, decimal pricePerHour, string? description = null)
    {
        Id = id;
        UpdateName(name);
        SetCapacity(capacity);
        SetPrice(pricePerHour);
        if (description != null) 
            UpdateDescription(description);
    }

    public void SetPrice(decimal newPrice) => PricePerHour = newPrice < 0 ? throw new ArgumentOutOfRangeException() : newPrice;
    public void SetCapacity(int newCapacity) => Capacity = newCapacity <= 0 ? throw new ArgumentOutOfRangeException() : newCapacity;

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace", nameof(name));
        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Name cannot be longer than {MaxNameLength} characters", nameof(name));
        Name = name.Trim();
    }

    public void UpdateDescription(string description)
    {
        if (description != null && string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be only whitespace", nameof(description));
        if (description?.Length > MaxDescriptionLength)
            throw new ArgumentException($"Description cannot be longer than {MaxDescriptionLength} characters", nameof(description));
        Description = description?.Trim();
    }
}