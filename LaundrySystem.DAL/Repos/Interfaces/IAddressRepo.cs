namespace LaundrySystem.DAL.Repos.Interfaces
{
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Base;

    public interface IAddressRepo : IBaseRepo<Address>
    {
        Address GetByEmail(string email);
    }
}