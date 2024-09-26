namespace LaundrySystem.BLL.Infrastructure.Services.Implementations
{
    using LaundrySystem.BLL.Infrastructure.Interfaces;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Models;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service for managing application users.
    /// </summary>
    public class AppUserService : BaseService<AppUserModel, AppUser, IAppUserRepo>, IAppUserService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppUserService"/> class.
        /// </summary>
        /// <param name="appUserRepo">The application user repository.</param>
        /// <param name="logger">The logger instance.</param>
        public AppUserService(IAppUserRepo appUserRepo, ILogger<AppUserService> logger)
            : base(appUserRepo, logger)
        {
        }

        // Implement any additional methods specific to AppUser here
    }
}