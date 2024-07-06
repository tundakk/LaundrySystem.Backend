namespace LaundrySystem.DAL.Repos
{
    using LaundrySystem.DAL.DataModel;
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.DAL.Repos.Interfaces;

    public class SlotRepo : BaseRepo<Slot>, ISlotRepo
    {
        public SlotRepo(DataContext dataContext) : base(dataContext)
        {
        }
    }
}