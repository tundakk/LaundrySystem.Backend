using LaundrySystem.BLL.Infrastructure.Interfaces;

namespace LaundrySystem.BLL.Services.Interfaces
{
    using LaundrySystem.Domain.Model.Models;
    using LaundrySystem.Domain.Model.Responses;
    using System.Collections.Generic;

    public interface ILostAndFoundService : IBaseService<LostAndFoundModel>
    {
        // Add any additional methods specific to LostAndFound if needed
    }
}
