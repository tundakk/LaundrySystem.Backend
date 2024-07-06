namespace LaundrySystem.DAL.Repos
{
    using LaundrySystem.DAL.DataModel;
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.DAL.Repos.Interfaces;

    public class LostAndFoundRepo : BaseRepo<LostAndFound>, ILostAndFoundRepo
    {
        public LostAndFoundRepo(DataContext dataContext) : base(dataContext)
        {
        }
    }
}