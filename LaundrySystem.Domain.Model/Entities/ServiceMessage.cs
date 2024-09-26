namespace LaundrySystem.Domain.Model.Entities
{
    public class ServiceMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Message { get; set; }

        //public DateTime CreatedAt { get; private set; } = DateTime.UtcNow; shoukd be private set but i do this to seed data

        //public bool IsRead { get; private set; } shoukd be private set but i do this to seed data

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; }

        // Business logic methods
        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}