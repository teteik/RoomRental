using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoomRental.Domain.Entities;

namespace RoomRental.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Room>().Property(x => x.Name).IsRequired().HasMaxLength(Room.MaxNameLength);
        modelBuilder.Entity<Room>().Property(x => x.Capacity).IsRequired();
        modelBuilder.Entity<Room>().Property(x => x.PricePerHour).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Room>().Property(x => x.Description).HasMaxLength(Room.MaxDescriptionLength);
        
        modelBuilder.Entity<Client>().Property(x => x.FullName).IsRequired().HasMaxLength(Client.MaxFullNameLength);
        modelBuilder.Entity<Client>().Property(x => x.PhoneNumber).HasMaxLength(Client.MaxPhoneNumberLength);
        modelBuilder.Entity<Client>().Property(x => x.Email).HasMaxLength(Client.MaxEmailLength);
        
        modelBuilder.Entity<Booking>().Property(x => x.ClientId).IsRequired();
        modelBuilder.Entity<Booking>().Property(x => x.RoomId).IsRequired();
        modelBuilder.Entity<Booking>().Property(x => x.Price).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Booking>().HasIndex(b => b.RoomId);
        modelBuilder.Entity<Booking>().HasIndex(b => b.ClientId);
        modelBuilder.Entity<Booking>().HasOne<Room>().WithMany(r => r.Bookings).HasForeignKey(b => b.RoomId);
        modelBuilder.Entity<Booking>().HasOne<Client>().WithMany(c => c.Bookings).HasForeignKey(b => b.ClientId);
    }
}