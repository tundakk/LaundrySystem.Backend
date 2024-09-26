namespace LaundrySystem.DAL.Repos.Implementations
{
    using LaundrySystem.DAL.DataModel;
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Entities;

    public class ServiceMessageRepo : BaseRepo<ServiceMessage>, IServiceMessageRepo
    {
        public ServiceMessageRepo(DataContext dataContext) : base(dataContext)
        {
        }
    }
}