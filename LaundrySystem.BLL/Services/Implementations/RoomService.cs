namespace LaundrySystem.BLL.Infrastructure.Services.Implementations
{
    using LaundrySystem.BLL.Infrastructure.Interfaces;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Entities;
    using LaundrySystem.Domain.Model.Models;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service for managing rooms.
    /// </summary>
    public class RoomService : BaseService<RoomModel, Room, IRoomRepo>, IRoomService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomService"/> class.
        /// </summary>
        /// <param name="roomRepo">The room repository.</param>
        /// <param name="logger">The logger instance.</param>
        public RoomService(IRoomRepo roomRepo, ILogger<RoomService> logger)
            : base(roomRepo, logger)
        {
        }

        // Implement any additional methods specific to Room here
    }
}