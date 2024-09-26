namespace LaundrySystem.DAL.Repos
{
    using LaundrySystem.DAL.DataModel;
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Entities;

    public class RoomRepo : BaseRepo<Room>, IRoomRepo
    {
        public RoomRepo(DataContext dataContext) : base(dataContext)
        {
        }
    }
}