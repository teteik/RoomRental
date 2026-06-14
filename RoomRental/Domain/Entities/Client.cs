namespace RoomRental.Domain.Entities;

public class Client
{
    public Guid Id { get; init; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; } 
    
    public Client(Guid id, string fullName, string email, string phoneNumber)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}