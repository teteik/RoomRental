using Microsoft.EntityFrameworkCore;
using RoomRental.API.DTOs;
using RoomRental.Domain.Entities;
using RoomRental.Infrastructure.Data;

namespace RoomRental.API.Services;

public class ImageService : IImageService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const string ImagesFolderName = "images";

    public ImageService(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    
    public async Task<RoomResponse> AddImagesToRoomAsync(Guid roomId, IFormFileCollection files)
    {
        ArgumentNullException.ThrowIfNull(files);
        if (files.Count == 0)
            throw new ArgumentException("No files provided", nameof(files));
            
        var room = await _context.Rooms
            .Include(r => r.Images) 
            .FirstOrDefaultAsync(r => r.Id == roomId)
            ?? throw new KeyNotFoundException($"Room with id {roomId} not found");

        int currentOrder = room.Images.Count;

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException($"File extension '{extension}' is not supported. Allowed: {string.Join(", ", AllowedExtensions)}");

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            
            var relativeUrl = await SaveFileToDiskAsync(file, uniqueFileName);
            
            var roomImage = new RoomImage(Guid.NewGuid(), roomId, relativeUrl, currentOrder);
            _context.RoomImages.Add(roomImage);
            currentOrder++;
        }

        await _context.SaveChangesAsync();

        return RoomMapper.ToResponse(room);
    }

    public async Task UpdateImagesOrderAsync(Guid roomId, List<UpdateImageOrderRequest> request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var room = await _context.Rooms
            .Include(r => r.Images)
            .FirstOrDefaultAsync(r => r.Id == roomId)
            ?? throw new KeyNotFoundException($"Room with id {roomId} not found");

        foreach (var item in request)
        {
            var image = room.Images.FirstOrDefault(i => i.Id == item.Id);
            if (image != null)
            {
                image.Order = item.Order;
            }
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteImagesFromRoomAsync(Guid roomId, Guid imageId)
    {
        var image = await _context.RoomImages.FindAsync(imageId)
            ?? throw new KeyNotFoundException($"Image with id {imageId} not found");
        
        if (image.RoomId != roomId)
            throw new ArgumentException("Image does not belong to this room", nameof(roomId));
        
        var filePath = Path.Combine(_env.WebRootPath, image.ImageUrl.TrimStart('/'));
        
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (IOException)
            {
            }
        }
        
        _context.RoomImages.Remove(image);
        await _context.SaveChangesAsync();
    }

    private async Task<string> SaveFileToDiskAsync(IFormFile file, string fileName)
    {
        var imagesFolder = Path.Combine(_env.WebRootPath, ImagesFolderName);

        if (!Directory.Exists(imagesFolder))
        {
            Directory.CreateDirectory(imagesFolder);
        }

        var filePath = Path.Combine(imagesFolder, fileName);
        
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/{ImagesFolderName}/{fileName}";
    }
}