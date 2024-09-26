namespace LaundrySystem.BLL.Infrastructure.Services.Implementations
{
    using LaundrySystem.BLL.Infrastructure.Interfaces;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Entities;
    using LaundrySystem.Domain.Model.Models;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service for managing timeslots.
    /// </summary>
    public class TimeslotService : BaseService<TimeslotModel, Timeslot, ITimeslotRepo>, ITimeslotService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeslotService"/> class.
        /// </summary>
        /// <param name="timeslotRepo">The timeslot repository.</param>
        /// <param name="logger">The logger instance.</param>
        public TimeslotService(ITimeslotRepo timeslotRepo, ILogger<TimeslotService> logger)
            : base(timeslotRepo, logger)
        {
        }

        // Implement any additional methods specific to Timeslot here
    }
}