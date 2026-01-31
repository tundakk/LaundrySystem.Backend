using LaundrySystem.BLL.Infrastructure.Interfaces;
using LaundrySystem.BLL.Notifications;
using LaundrySystem.DAL.DataModel;
using LaundrySystem.DAL.Repos.Interfaces;
using LaundrySystem.Domain.Model.Entities;
using LaundrySystem.Domain.Model.Enums;
using LaundrySystem.Domain.Model.Models;
using LaundrySystem.Domain.Model.Responses;
using Mapster;
using Microsoft.Extensions.Logging;

namespace LaundrySystem.BLL.Infrastructure.Services.Implementations
{
    /// <summary>
    /// Service for handling bookings.
    /// </summary>
    public class BookingService : BaseService<BookingModel, Booking, IBookingRepo>, IBookingService
    {
        private readonly ITimeslotRepo _timeslotRepo;
        private readonly IAppUserRepo _appUserRepo;
        private readonly DataContext _dataContext;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingService"/> class.
        /// </summary>
        /// <param name="bookingRepo">The booking repository.</param>
        /// <param name="timeslotRepo">The timeslot repository.</param>
        /// <param name="appUserRepo">The app user repository.</param>
        /// <param name="dataContext">The data context for transaction handling.</param>
        /// <param name="notificationService">The notification service.</param>
        /// <param name="logger">The logger instance.</param>
        public BookingService(
            IBookingRepo bookingRepo,
            ITimeslotRepo timeslotRepo,
            IAppUserRepo appUserRepo,
            DataContext dataContext,
            INotificationService notificationService,
            ILogger<BookingService> logger)
            : base(bookingRepo, logger)
        {
            _timeslotRepo = timeslotRepo;
            _appUserRepo = appUserRepo;
            _dataContext = dataContext;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Creates a new booking.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="timeslotId">The timeslot ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A service response containing the created booking model.</returns>
        public async Task<ServiceResponse<BookingModel>> CreateBookingAsync(Guid userId, Guid timeslotId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if user exists
                var user = await _appUserRepo.GetByIdAsync(userId, cancellationToken);
                if (user == null)
                {
                    return new ServiceResponse<BookingModel>
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                // Check for active booking
                if (await Repository.HasActiveBookingAsync(userId, cancellationToken))
                {
                    return new ServiceResponse<BookingModel>
                    {
                        Success = false,
                        Message = "User already has an active booking."
                    };
                }

                // Check timeslot availability
                var timeslot = await _timeslotRepo.GetByIdAsync(timeslotId, cancellationToken);
                if (timeslot == null || !timeslot.IsAvailable)
                {
                    return new ServiceResponse<BookingModel>
                    {
                        Success = false,
                        Message = "Timeslot is not available."
                    };
                }

                // Use transaction to ensure both operations succeed or fail together
                await using var transaction = await _dataContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    // Create booking
                    var booking = new Booking
                    {
                        UserId = userId,
                        TimeslotId = timeslotId,
                        Status = BookingStatus.Pending,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    // Insert booking
                    await Repository.InsertAsync(booking, cancellationToken);

                    // Update timeslot availability
                    timeslot.MarkAsUnavailable();
                    await _timeslotRepo.UpdateAsync(timeslot, cancellationToken);

                    await transaction.CommitAsync(cancellationToken);

                    return new ServiceResponse<BookingModel>
                    {
                        Success = true,
                        Data = booking.Adapt<BookingModel>(),
                        Message = "Booking created successfully."
                    };
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating booking for user {UserId}", userId);
                return new ServiceResponse<BookingModel>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<IEnumerable<object>>> GetUpcomingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var bookings = await Repository.GetUpcomingByUserIdAsync(userId, cancellationToken);
                var result = bookings.Select(b => (object)new
                {
                    b.Id,
                    b.Status,
                    b.CreatedAt,
                    StartTime = b.Timeslot.SlotTime.Start,
                    EndTime = b.Timeslot.SlotTime.End,
                    RoomName = b.Timeslot.Room.Name,
                    RoomLocation = b.Timeslot.Room.Location
                });

                return new ServiceResponse<IEnumerable<object>>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting bookings for user {UserId}", userId);
                return new ServiceResponse<IEnumerable<object>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Deletes a booking by ID.
        /// </summary>
        /// <param name="id">The booking ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A service response indicating the success or failure of the operation.</returns>
        public override async Task<ServiceResponse<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var booking = await Repository.GetByIdAsync(id, cancellationToken);
                if (booking == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Booking not found",
                        Data = false
                    };
                }

                // Use transaction to ensure both operations succeed or fail together
                await using var transaction = await _dataContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    // Store timeslot ID before deletion for notification
                    var timeslotId = booking.TimeslotId;

                    // Update timeslot to make it available again
                    var timeslot = await _timeslotRepo.GetByIdAsync(timeslotId, cancellationToken);
                    if (timeslot != null)
                    {
                        timeslot.MarkAsAvailable();
                        await _timeslotRepo.UpdateAsync(timeslot, cancellationToken);
                    }

                    await Repository.DeleteAsync(booking, cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    // Notify users who subscribed to this timeslot (fire and forget)
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _notificationService.NotifyTimeslotAvailableAsync(timeslotId, CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "Error sending timeslot availability notification for {TimeslotId}", timeslotId);
                        }
                    }, CancellationToken.None);

                    return new ServiceResponse<bool>
                    {
                        Success = true,
                        Message = "Booking deleted successfully.",
                        Data = true
                    };
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting booking");
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                };
            }
        }
    }
}
