namespace LaundrySystem.Domain.Model.Entities
{
    public class Room
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string Location { get; set; }

        //public bool IsAvailable { get; private set; } shoukd be private set but i do this to seed data
        public bool IsAvailable { get; set; }

        public int MaxCapacity { get; set; }

        // Navigation property
        public ICollection<Timeslot> Timeslots { get; set; } = new List<Timeslot>();

        // Business logic methods
        public void MarkAsAvailable()
        {
            IsAvailable = true;
        }

        public void MarkAsUnavailable()
        {
            IsAvailable = false;
        }
    }
}