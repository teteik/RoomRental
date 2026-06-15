namespace RoomRental.Domain.Entities;

public class Client
{
    public const int MaxFullNameLength = 100;
    public const int MaxPhoneNumberLength = 20;
    public const int MinPhoneNumberLength = 7;
    public const int MaxEmailLength = 200;
    public Guid Id { get; init; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public ICollection<Booking> Bookings { get; private set; } = new List<Booking>();
    
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
        if (fullName.Length > MaxFullNameLength)
            throw new ArgumentException($"Name cannot be longer than {MaxFullNameLength} characters", nameof(fullName));
        FullName = fullName.Trim();
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or whitespace", nameof(email));
        if (email.Length > MaxEmailLength)
            throw new ArgumentException($"Email cannot be longer than {MaxEmailLength} characters", nameof(email));
        if (!email.Contains('@') || !email.Contains('.'))
            throw new ArgumentException("Email is not a valid email address", nameof(email));
        Email = email.Trim().ToLower();
    }

    public void UpdatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone cannot be empty", nameof(phone));
    
        var cleanPhone = new string(phone.Where(char.IsDigit).ToArray());
        if (cleanPhone.Length < MinPhoneNumberLength)
            throw new  ArgumentException($"Phone cannot be less than {MinPhoneNumberLength}", nameof(phone));
        if (cleanPhone.Length > MaxPhoneNumberLength)
            throw new ArgumentException($"Phone must contain no more than {MaxPhoneNumberLength} digits", nameof(phone));
    
        PhoneNumber = phone.Trim();
    }
}