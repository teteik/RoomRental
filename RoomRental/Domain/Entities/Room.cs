namespace RoomRental.Domain.Entities;

public class Room
{
    public Guid Id { get; init; }
    public string Name { get; private set; } 
    public int Capacity { get; private set; }
    public decimal PricePerHour { get; private set; }
    public string? Description { get; private set; } = string.Empty;
    
    public Room(Guid id, string name, int capacity, decimal pricePerHour)
    {
        Id = id;
        UpdateName(name);
        SetCapacity(capacity);
        SetPrice(pricePerHour);
    }

    public void SetPrice(decimal newPrice) => PricePerHour = newPrice < 0 ? throw new ArgumentOutOfRangeException() : newPrice;
    public void SetCapacity(int newCapacity) => Capacity = newCapacity <= 0 ? throw new ArgumentOutOfRangeException() : newCapacity;

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace", nameof(name));
        if (name.Length > 100)
            throw new ArgumentException("Name cannot be longer than 100 characters", nameof(name));
        Name = name.Trim();
    }

    public void UpdateDescription(string description)
    {
        if (description != null && string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be only whitespace", nameof(description));
        if (description?.Length > 1000)
            throw new ArgumentException("Description cannot be longer than 1000 characters", nameof(description));
        Description = description?.Trim();
    }
}