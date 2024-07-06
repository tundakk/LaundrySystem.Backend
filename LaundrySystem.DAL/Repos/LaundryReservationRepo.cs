namespace LaundrySystem.DAL.Repos
{
    using LaundrySystem.DAL.DataModel;
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.DAL.Repos.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class LaundryReservationRepo : BaseRepo<LaundryReservation>, ILaundryReservationRepo
    {
        public LaundryReservationRepo(DataContext dataContext) : base(dataContext)
        {
        }

        public IEnumerable<LaundryReservation> GetByHouseholdId(int householdId)
        {
            return dataContext.LaundryReservations.Where(lr => lr.HouseholdId == householdId).ToList();
        }

        public IEnumerable<LaundryReservation> GetBySlotId(int slotId)
        {
            return dataContext.LaundryReservations.Where(lr => lr.SlotId == slotId).ToList();
        }
    }
}