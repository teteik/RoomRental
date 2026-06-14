namespace RoomRental.Domain.Entities;

public class Room
{
    public Guid Id { get; init; }
    public string Name { get; private set; } 
    public int Capacity { get; private set; }
    public decimal PricePerHour { get; private set; }
    public string? Description { get; private set; } = String.Empty;
    
    public Room(Guid id, string name, int capacity, decimal pricePerHour)
    {
        Id = id;
        Name = name;
        SetCapacity(capacity);
        SetPrice(pricePerHour);
    }

    public void SetPrice(decimal newPrice) => PricePerHour = newPrice < 0 ? throw new ArgumentOutOfRangeException() : newPrice;
    public void SetCapacity(int newCapacity) => Capacity = newCapacity <= 0 ? throw new ArgumentOutOfRangeException() : newCapacity;
}