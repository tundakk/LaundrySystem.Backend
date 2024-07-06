namespace LaundrySystem.DAL.Repos
{
    using LaundrySystem.DAL.DataModel;
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.DAL.Repos.Interfaces;

    public class ServiceMessageRepo : BaseRepo<ServiceMessage>, IServiceMessageRepo
    {
        public ServiceMessageRepo(DataContext dataContext) : base(dataContext)
        {
        }
    }
}