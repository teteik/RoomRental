using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.API.DTOs;
using RoomRental.Domain.Entities;
using RoomRental.Domain.Enums;
using RoomRental.Infrastructure.Data;

namespace RoomRental.API.Controllers;

[ApiController] 
[Route("api/[controller]")]
public class RoomsController(AppDbContext context, IWebHostEnvironment env) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomResponse>>> Get()
    {
        var rooms = await context.Rooms.Include(r => r.Images).ToListAsync();
        var response = rooms.Select(MapToResponse).ToList();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomResponse>> Get(Guid id)
    {
        var room = await context.Rooms
            .Include(r => r.Images) 
            .FirstOrDefaultAsync(r => r.Id == id);
            
        if (room == null)
            return NotFound();

        return Ok(MapToResponse(room));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<RoomResponse>> Post([FromBody] CreateRoomRequest request)
    {
        try
        {
            var room = new Room(Guid.NewGuid(), request.Name, request.Capacity, request.PricePerHour, request.Description);
            context.Rooms.Add(room);
            await context.SaveChangesAsync();
            
            return StatusCode(201, MapToResponse(room));
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<RoomResponse>> Put(Guid id, [FromBody] UpdateRoomRequest request)
    {
        try
        {
            var room = await context.Rooms
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
                return NotFound();

            room.UpdateName(request.Name);
            room.SetCapacity(request.Capacity);
            room.SetPrice(request.PricePerHour);
            if (request.Description != null)
                room.UpdateDescription(request.Description);

            await context.SaveChangesAsync();

            return Ok(MapToResponse(room));
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var room = await context.Rooms.FindAsync(id);
        if (room == null)
            return NotFound();
        
        if (await context.Bookings.AnyAsync(b => b.RoomId == id))
            return Conflict("Cannot delete room because it has booking history");

        context.Rooms.Remove(room);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}/schedule")]
    public async Task<ActionResult<IEnumerable<BookedSlotResponse>>> GetSchedule(Guid id, [FromQuery] DateTime date)
    {
        var room = await context.Rooms.FindAsync(id);
        if (room == null)
            return NotFound("Room not found");
        
        date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        
        var startOfDay = date.Date;
        var endOfDay = date.Date.AddDays(1);

        var bookedSlots = await context.Bookings
            .Where(b => b.RoomId == id
                        && b.Status != BookingStatus.Cancelled
                        && b.EndTime > startOfDay
                        && b.StartTime < endOfDay)
            .Select(b => new BookedSlotResponse
            {
                StartTime = b.StartTime,
                EndTime = b.EndTime,
            }).ToListAsync();
        
        return Ok(bookedSlots);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/images")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<RoomResponse>> PostImages(Guid id, IFormFile file) 
    {
        if (file.Length == 0)
            return BadRequest("File is empty");
            
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(extension))
            return BadRequest("File is not a valid image format");
        
        var room = await context.Rooms
            .Include(r => r.Images) 
            .FirstOrDefaultAsync(r => r.Id == id);
            
        if (room == null)
            return NotFound("Room not found");
        
        var uniqueFileName = Guid.NewGuid() + extension;
        
        var imagesFolder = Path.Combine(env.WebRootPath, "images");
        
        if (!Directory.Exists(imagesFolder))
            Directory.CreateDirectory(imagesFolder);
        
        var filePath = Path.Combine(imagesFolder, uniqueFileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        
        var currentCount = room.Images.Count; 
        
        var roomImage = new RoomImage(Guid.NewGuid(), id, $"/images/{uniqueFileName}", currentCount);
        context.RoomImages.Add(roomImage);
        await context.SaveChangesAsync();

        return Ok(MapToResponse(room));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}/images/{imageId}")] 
    public async Task<ActionResult> DeleteImages(Guid id, Guid imageId)
    {
        var image = await context.RoomImages.FindAsync(imageId);
        if (image == null)
            return NotFound("Image not found");
        
        if (image.RoomId != id)
            return BadRequest("Image does not belong to this room");
        
        var filePath = Path.Combine(env.WebRootPath, image.ImageUrl.TrimStart('/'));
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
        
        context.RoomImages.Remove(image);
        await context.SaveChangesAsync();
        
        return NoContent();
    }
    
    private RoomResponse MapToResponse(Room room)
    {
        return new RoomResponse
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            PricePerHour = room.PricePerHour,
            Description = room.Description,
            Images = room.Images
                .OrderBy(i => i.Order)
                .Select(i => new RoomImageResponse 
                { 
                    Id = i.Id,       
                    ImageUrl = i.ImageUrl 
                })
                .ToList()
        };
    }
}