namespace LaundrySystem.BLL.Infrastructure.Services.Implementations
{
    using LaundrySystem.BLL.Infrastructure.Interfaces;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Entities;
    using LaundrySystem.Domain.Model.Models;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service for managing service messages.
    /// </summary>
    public class ServiceMessageService : BaseService<ServiceMessageModel, ServiceMessage, IServiceMessageRepo>, IServiceMessageService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMessageService"/> class.
        /// </summary>
        /// <param name="serviceMessageRepo">The service message repository.</param>
        /// <param name="logger">The logger instance.</param>
        public ServiceMessageService(IServiceMessageRepo serviceMessageRepo, ILogger<ServiceMessageService> logger)
            : base(serviceMessageRepo, logger)
        {
        }
    }
}