namespace LaundrySystem.DAL.Repos.Interfaces
{
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Base;

    public interface IChatMessageRepo : IBaseRepo<ChatMessage>
    {
        // Add any additional methods specific to ChatMessage if needed
        IEnumerable<ChatMessage> GetByHouseholdId(int householdId);
    }
}