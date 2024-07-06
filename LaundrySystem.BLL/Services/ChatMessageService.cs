namespace LaundrySystem.BLL.Infrastructure.Services
{
    using LaundrySystem.BLL.Services.Interfaces;
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Models;
    using Microsoft.Extensions.Logging;

    public class ChatMessageService : BaseService<ChatMessageModel, ChatMessage, IChatMessageRepo>, IChatMessageService
    {
        public ChatMessageService(IChatMessageRepo chatmessageRepo, ILogger<ChatMessageService> logger)
            : base(chatmessageRepo, logger)
        {
        }

        // Implement any additional methods specific to ChatMessage here
    }
}