using Microsoft.AspNetCore.Identity;

namespace RoomRental.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public Guid? ClientId { get; set; }
    public Client? Client { get; set; }
}