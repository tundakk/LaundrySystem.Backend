namespace LaundrySystem.DAL.Repos
{
    using LaundrySystem.DAL.DataModel;
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Entities;

    public class LostAndFoundRepo : BaseRepo<LostAndFound>, ILostAndFoundRepo
    {
        public LostAndFoundRepo(DataContext dataContext) : base(dataContext)
        {
        }
    }
}