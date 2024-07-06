namespace LaundrySystem.DAL.Repos.Interfaces
{
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Base;

    public interface ILaundryReservationRepo : IBaseRepo<LaundryReservation>
    {
        // Add any additional methods specific to LaundryReservation if needed
        IEnumerable<LaundryReservation> GetByHouseholdId(int householdId);

        IEnumerable<LaundryReservation> GetBySlotId(int slotId);
    }
}