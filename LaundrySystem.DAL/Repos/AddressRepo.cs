namespace LaundrySystem.DAL.Repos
{
    using LaundrySystem.DAL.DataModel;
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.DAL.Repos.Interfaces;
    using System.Linq;

    public class AddressRepo : BaseRepo<Address>, IAddressRepo
    {
        public AddressRepo(DataContext dataContext) : base(dataContext)
        {
        }

        public override Address Delete(Address entity)
        {
            return dataContext.Addresses.Remove(entity).Entity;
        }

        public Address GetByEmail(string email)
        {
            return dataContext.Addresses.FirstOrDefault(a => a.EmailAddress == email);
        }

        public override Address Insert(Address entity)
        {
            throw new NotImplementedException();
        }
    }
}