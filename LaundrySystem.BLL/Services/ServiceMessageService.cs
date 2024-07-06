namespace LaundrySystem.BLL.Infrastructure.Services
{
    using LaundrySystem.BLL.Services.Interfaces;
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Models;
    using LaundrySystem.Domain.Model.Responses;
    using Mapster;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;

    public class ServiceMessageService : BaseService<ServiceMessageModel, ServiceMessage, IServiceMessageRepo>, IServiceMessageService
    {
        public ServiceMessageService(IServiceMessageRepo servicemessageRepo, ILogger<ServiceMessageService> logger)
            : base(servicemessageRepo, logger)
        {
        }

        // Implement any additional methods specific to ServiceMessage here
    }
}
