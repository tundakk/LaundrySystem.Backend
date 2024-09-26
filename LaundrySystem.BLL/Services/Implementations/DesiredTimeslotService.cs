namespace LaundrySystem.BLL.Infrastructure.Services.Implementations
{
    using LaundrySystem.BLL.Infrastructure.Interfaces;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Entities;
    using LaundrySystem.Domain.Model.Models;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service for managing desired timeslots.
    /// </summary>
    public class DesiredTimeslotService : BaseService<DesiredTimeslotModel, DesiredTimeslot, IDesiredTimeslotRepo>, IDesiredTimeslotService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DesiredTimeslotService"/> class.
        /// </summary>
        /// <param name="desiredTimeslotRepo">The desired timeslot repository.</param>
        /// <param name="logger">The logger instance.</param>
        public DesiredTimeslotService(IDesiredTimeslotRepo desiredTimeslotRepo, ILogger<DesiredTimeslotService> logger)
            : base(desiredTimeslotRepo, logger)
        {
        }
    }
}