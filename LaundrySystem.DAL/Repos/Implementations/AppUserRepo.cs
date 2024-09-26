namespace LaundrySystem.DAL.Repos
{
    using LaundrySystem.DAL.DataModel;
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Entities;

    public class AppUserRepo : BaseRepo<AppUser>, IAppUserRepo
    {
        public AppUserRepo(DataContext dataContext) : base(dataContext)
        {
        }
    }
}