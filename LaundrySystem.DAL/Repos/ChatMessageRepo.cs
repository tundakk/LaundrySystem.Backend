namespace LaundrySystem.DAL.Repos
{
    using LaundrySystem.DAL.DataModel;
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.DAL.Repos.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class ChatMessageRepo : BaseRepo<ChatMessage>, IChatMessageRepo
    {
        public ChatMessageRepo(DataContext dataContext) : base(dataContext)
        {
        }

        public override IQueryable<ChatMessage> GetAll()
        {
            return dataContext.ChatMessages;
        }

        public override ChatMessage Insert(ChatMessage entity)
        {
            return dataContext.ChatMessages.Add(entity).Entity;
        }

        public override ChatMessage Update(ChatMessage entity)
        {
            dataContext.SetModified(entity);
            return entity;
        }

        public override ChatMessage Delete(ChatMessage entity)
        {
            return dataContext.ChatMessages.Remove(entity).Entity;
        }

        public override ChatMessage GetById(int id)
        {
            return dataContext.ChatMessages.Find(id);
        }

        public IEnumerable<ChatMessage> GetByHouseholdId(int householdId)
        {
            return dataContext.ChatMessages.Where(cm => cm.HouseholdId == householdId).ToList();
        }
    }
}