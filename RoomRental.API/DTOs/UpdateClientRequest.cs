namespace RoomRental.API.DTOs;

public class UpdateClientRequest
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
}