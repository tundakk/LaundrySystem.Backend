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

    public class LostAndFoundService : BaseService<LostAndFoundModel, LostAndFound, ILostAndFoundRepo>, ILostAndFoundService
    {
        public LostAndFoundService(ILostAndFoundRepo lostandfoundRepo, ILogger<LostAndFoundService> logger)
            : base(lostandfoundRepo, logger)
        {
        }

        // Implement any additional methods specific to LostAndFound here
    }
}
