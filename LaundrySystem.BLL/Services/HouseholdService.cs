namespace LaundrySystem.BLL.Infrastructure.Services
{
    using LaundrySystem.BLL.Services.Interfaces;
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Models;
    using Microsoft.Extensions.Logging;

    public class HouseholdService : BaseService<HouseholdModel, Household, IHouseholdRepo>, IHouseholdService
    {
        public HouseholdService(IHouseholdRepo householdRepo, ILogger<HouseholdService> logger)
            : base(householdRepo, logger)
        {
        }

        // Implement any additional methods specific to Household here
    }
}