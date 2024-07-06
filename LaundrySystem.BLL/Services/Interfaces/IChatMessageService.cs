using LaundrySystem.BLL.Infrastructure.Interfaces;

namespace LaundrySystem.BLL.Services.Interfaces
{
    using LaundrySystem.Domain.Model.Models;
    using LaundrySystem.Domain.Model.Responses;
    using System.Collections.Generic;

    public interface IChatMessageService : IBaseService<ChatMessageModel>
    {
        // Add any additional methods specific to ChatMessage if needed
    }
}
