namespace LaundrySystem.DAL.Repos
{
    using LaundrySystem.DAL.DataModel;
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.DAL.Repos.Interfaces;
    using System.Linq;

    public class HouseholdRepo : BaseRepo<Household>, IHouseholdRepo
    {
        public HouseholdRepo(DataContext dataContext) : base(dataContext)
        {
        }

        public Household GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("GetByName - name must not be null or empty.");
            }

            return dataContext.Households
                              .AsQueryable()
                              .FirstOrDefault(h => h.Name.ToLower() == name.ToLower());
        }
    }
}