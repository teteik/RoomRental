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
        UpdateFullName(fullName);
        UpdateEmail(email);
        UpdatePhone(phoneNumber);
    }

    public void UpdateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Name cannot be null or whitespace",nameof(fullName));
        if (fullName.Length > 100)
            throw new ArgumentException("Name cannot be longer than 100 characters", nameof(fullName));
        FullName = fullName.Trim();
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or whitespace", nameof(email));
        if (email.Length > 200)
            throw new ArgumentException("Email cannot be longer than 200 characters", nameof(email));
        if (!email.Contains('@') || !email.Contains('.'))
            throw new ArgumentException("Email is not a valid email address", nameof(email));
        Email = email.Trim().ToLower();
    }

    public void UpdatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone cannot be empty", nameof(phone));
    
        var cleanPhone = new string(phone.Where(char.IsDigit).ToArray());
        if (cleanPhone.Length < 10)
            throw new ArgumentException("Phone must contain at least 10 digits", nameof(phone));
    
        PhoneNumber = phone.Trim();
    }
}