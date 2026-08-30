using RoomRental.API.DTOs;
using RoomRental.Domain.Entities;

namespace RoomRental.API.Services;

public interface IBookingService
{
    Task<IEnumerable<BookingResponse>> GetBookingsAsync(Guid? clientId);
    Task<BookingResponse> GetBookingByIdAsync(Guid id);
    Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request);
    Task<BookingResponse> CancelBookingAsync(Guid id);
    Task<BookingResponse> ConfirmBookingAsync(Guid id);
}