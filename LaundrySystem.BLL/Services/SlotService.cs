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

    public class SlotService : BaseService<SlotModel, Slot, ISlotRepo>, ISlotService
    {
        public SlotService(ISlotRepo slotRepo, ILogger<SlotService> logger)
            : base(slotRepo, logger)
        {
        }

        // Implement any additional methods specific to Slot here
    }
}
