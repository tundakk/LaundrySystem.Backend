namespace LaundrySystem.Domain.Model.Dtos
{
    using LaundrySystem.Domain.Model.Models;
    using System.Collections.Generic;

    /// <summary>
    /// This is made to see proper structure of a dto
    /// </summary>
    public class HouseHoldDto
    {
        public int HouseholdId { get; set; }
        public string Name { get; set; }

        public List<AddressModel> Addresses { get; set; }
        public List<LaundryReservationModel> LaundryReservationModels { get; set; }
    }
}