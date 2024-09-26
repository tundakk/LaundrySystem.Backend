namespace LaundrySystem.DAL.Repos
{
    using LaundrySystem.DAL.DataModel;
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Entities;

    public class TimeslotRepo : BaseRepo<Timeslot>, ITimeslotRepo
    {
        public TimeslotRepo(DataContext dataContext) : base(dataContext)
        {
        }
    }
}