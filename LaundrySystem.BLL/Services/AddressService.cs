namespace LaundrySystem.BLL.Infrastructure.Services
{
    using LaundrySystem.BLL.Services.Interfaces;
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Models;
    using Microsoft.Extensions.Logging;

    public class AddressService : BaseService<AddressModel, Address, IAddressRepo>, IAddressService
    {
        public AddressService(IAddressRepo addressRepo, ILogger<AddressService> logger)
            : base(addressRepo, logger)
        {
        }

        // Implement any additional methods specific to Address here
    }
}