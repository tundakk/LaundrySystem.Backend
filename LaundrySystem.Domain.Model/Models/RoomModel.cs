namespace LaundrySystem.Domain.Model.Models
{
    // Models/LoginViewModel.cs
    public class RoomModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public bool IsAvailable { get; set; }
        public int MaxCapacity { get; set; }
    }
}