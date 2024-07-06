namespace LaundrySystem.DAL.Repos.Interfaces
{
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Base;

    public interface IHouseholdRepo : IBaseRepo<Household>
    {
        Household GetByName(string name);
    }
}