namespace LaundrySystem.DAL.Repos.Interfaces
{
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.Domain.Model.Entities;

    public interface IBookingRepo : IBaseRepo<Booking>
    {
        Task<bool> HasActiveBookingAsync(Guid userId);
    }
}