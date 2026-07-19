namespace RoomRental.Domain.Entities;

public class RoomImage
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string ImageUrl { get; set; }
    public int Order { get; set; }

    public RoomImage(Guid id, Guid roomId, string imageUrl, int order)
    {
        if (string.IsNullOrEmpty(imageUrl))
            throw new ArgumentNullException("Image URL cannot be empty.");
        Id = id;
        RoomId = roomId;
        ImageUrl = imageUrl;
        Order = order;
    }
}