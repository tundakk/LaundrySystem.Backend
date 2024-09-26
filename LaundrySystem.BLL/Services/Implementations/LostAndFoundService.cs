namespace LaundrySystem.BLL.Infrastructure.Services.Implementations
{
    using LaundrySystem.BLL.Infrastructure.Interfaces;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Entities;
    using LaundrySystem.Domain.Model.Models;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service class for handling Lost and Found operations.
    /// </summary>
    public class LostAndFoundService : BaseService<LostAndFoundModel, LostAndFound, ILostAndFoundRepo>, ILostAndFoundService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LostAndFoundService"/> class.
        /// </summary>
        /// <param name="lostAndFoundRepo">The repository for Lost and Found entities.</param>
        /// <param name="logger">The logger instance.</param>
        public LostAndFoundService(ILostAndFoundRepo lostAndFoundRepo, ILogger<LostAndFoundService> logger)
            : base(lostAndFoundRepo, logger)
        {
        }

        // Implement any additional methods specific to LostAndFound here
    }
}