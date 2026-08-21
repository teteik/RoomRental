using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomRental.API.DTOs;
using RoomRental.API.Services;

namespace RoomRental.API.Controllers;

[ApiController] 
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly IImageService _imageService;

    public RoomsController(IRoomService roomService, IImageService imageService)
    {
        _roomService = roomService;
        _imageService = imageService;
    }
    
    [HttpGet]
    public async Task<ActionResult<PagedResult<RoomResponse>>> Get(string? search = null, int? minCapacity = null, decimal? maxPrice = null, int pageNumber = 1, int pageSize = 9)
    {
        var pagedRooms = await _roomService.GetRoomsAsync(search, minCapacity, maxPrice, pageNumber, pageSize);
        return Ok(pagedRooms);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomResponse>> Get(Guid id)
    {
        try
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            return Ok(room);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<RoomResponse>> Post([FromBody] CreateRoomRequest request)
    {
        try
        {
            var room = await _roomService.CreateRoomAsync(request);
            return CreatedAtAction(nameof(Get), new { id = room.Id }, room);
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
            var room = await _roomService.UpdateRoomAsync(id, request);
            return Ok(room);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
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
        try
        {
            await _roomService.DeleteRoomAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return Conflict(e.Message);
        }
    }

    [HttpGet("{id}/schedule")]
    public async Task<ActionResult<IEnumerable<BookedSlotResponse>>> GetSchedule(Guid id, [FromQuery] DateTime date)
    {
        try
        {
            var bookedSlots = await _roomService.GetBookedSlotsAsync(id, date);
            return Ok(bookedSlots);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/images")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<RoomResponse>> PostImages(Guid id, [FromForm] IFormFileCollection files) 
    {
        try
        {
            var images = await _imageService.AddImagesToRoomAsync(id,  files);
            return Ok(images);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}/images/{imageId}")] 
    public async Task<ActionResult> DeleteImages(Guid id, Guid imageId)
    {
        try
        {
            await _imageService.DeleteImagesFromRoomAsync(id, imageId);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/images/order")]
    public async Task<ActionResult> UpdateImagesOrder(Guid id, [FromBody] List<UpdateImageOrderRequest> request)
    {
        try
        {
            await _imageService.UpdateImagesOrderAsync(id, request);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        
    }
}