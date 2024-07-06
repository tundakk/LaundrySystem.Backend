using LaundrySystem.DAL.Entities;
using LaundrySystem.Domain.Model.Models;
using Mapster;

/// <summary>
/// Mapping configuration class.
/// </summary>
public static class MappingConfig
{
    /// <summary>
    /// Register mappings.
    /// </summary>
    public static void RegisterMappings()
    {
        // Address mapping
        TypeAdapterConfig<Address, AddressModel>.NewConfig()
            .Map(dest => dest.HomeAddress, src => src.HomeAddress)
            .Map(dest => dest.PhoneNumber1, src => src.PhoneNumber1)
            .Map(dest => dest.PhoneNumber2, src => src.PhoneNumber2)
            .Map(dest => dest.EmailAddress, src => src.EmailAddress);

        TypeAdapterConfig<AddressModel, Address>.NewConfig()
            .Map(dest => dest.HomeAddress, src => src.HomeAddress)
            .Map(dest => dest.PhoneNumber1, src => src.PhoneNumber1)
            .Map(dest => dest.PhoneNumber2, src => src.PhoneNumber2)
            .Map(dest => dest.EmailAddress, src => src.EmailAddress);

        // ChatMessage mapping
        TypeAdapterConfig<ChatMessage, ChatMessageModel>.NewConfig()
            .Map(dest => dest.ChatMessageId, src => src.ChatMessageId)
            .Map(dest => dest.HouseholdId, src => src.HouseholdId)
            .Map(dest => dest.SenderId, src => src.SenderId)
            .Map(dest => dest.MessageText, src => src.MessageText)
            .Map(dest => dest.Timestamp, src => src.Timestamp)
            .Map(dest => dest.Household, src => src.Household);

        TypeAdapterConfig<ChatMessageModel, ChatMessage>.NewConfig()
            .Map(dest => dest.ChatMessageId, src => src.ChatMessageId)
            .Map(dest => dest.HouseholdId, src => src.HouseholdId)
            .Map(dest => dest.SenderId, src => src.SenderId)
            .Map(dest => dest.MessageText, src => src.MessageText)
            .Map(dest => dest.Timestamp, src => src.Timestamp)
            .Map(dest => dest.Household, src => src.Household);

        // LaundryReservation mapping
        TypeAdapterConfig<LaundryReservation, LaundryReservationModel>.NewConfig()
            .Map(dest => dest.ReservationId, src => src.ReservationId)
            .Map(dest => dest.HouseholdId, src => src.HouseholdId)
            .Map(dest => dest.SlotId, src => src.SlotId)
            .Map(dest => dest.ExpectedStart, src => src.ExpectedStart)
            .Map(dest => dest.Household, src => src.Household)
            .Map(dest => dest.Slot, src => src.Slot);

        TypeAdapterConfig<LaundryReservationModel, LaundryReservation>.NewConfig()
            .Map(dest => dest.ReservationId, src => src.ReservationId)
            .Map(dest => dest.HouseholdId, src => src.HouseholdId)
            .Map(dest => dest.SlotId, src => src.SlotId)
            .Map(dest => dest.ExpectedStart, src => src.ExpectedStart)
            .Map(dest => dest.Household, src => src.Household)
            .Map(dest => dest.Slot, src => src.Slot);

        // ServiceMessage mapping
        TypeAdapterConfig<ServiceMessage, ServiceMessageModel>.NewConfig()
            .Map(dest => dest.ServiceMessageId, src => src.ServiceMessageId)
            .Map(dest => dest.Message, src => src.Message)
            .Map(dest => dest.StartDate, src => src.StartDate)
            .Map(dest => dest.EndDate, src => src.EndDate);

        TypeAdapterConfig<ServiceMessageModel, ServiceMessage>.NewConfig()
            .Map(dest => dest.ServiceMessageId, src => src.ServiceMessageId)
            .Map(dest => dest.Message, src => src.Message)
            .Map(dest => dest.StartDate, src => src.StartDate)
            .Map(dest => dest.EndDate, src => src.EndDate);

        // Slot mapping
        TypeAdapterConfig<Slot, SlotModel>.NewConfig()
            .Map(dest => dest.SlotId, src => src.SlotId)
            .Map(dest => dest.AvailableDateTime, src => src.AvailableDateTime)
            .Map(dest => dest.Reserved, src => src.Reserved)
            .Map(dest => dest.NotifyHouseholdId, src => src.NotifyHouseholdId)
            .Map(dest => dest.NotifyHousehold, src => src.NotifyHousehold)
            .Map(dest => dest.LaundryReservations, src => src.LaundryReservations);

        TypeAdapterConfig<SlotModel, Slot>.NewConfig()
            .Map(dest => dest.SlotId, src => src.SlotId)
            .Map(dest => dest.AvailableDateTime, src => src.AvailableDateTime)
            .Map(dest => dest.Reserved, src => src.Reserved)
            .Map(dest => dest.NotifyHouseholdId, src => src.NotifyHouseholdId)
            .Map(dest => dest.NotifyHousehold, src => src.NotifyHousehold)
            .Map(dest => dest.LaundryReservations, src => src.LaundryReservations);

        // LostAndFound mapping
        TypeAdapterConfig<LostAndFound, LostAndFoundModel>.NewConfig()
            .Map(dest => dest.LostAndFoundId, src => src.LostAndFoundId)
            .Map(dest => dest.HouseholdId, src => src.HouseholdId)
            .Map(dest => dest.Image, src => src.Image)
            .Map(dest => dest.TextMessage, src => src.TextMessage)
            .Map(dest => dest.RegistrationDate, src => src.RegistrationDate)
            .Map(dest => dest.Household, src => src.Household);

        TypeAdapterConfig<LostAndFoundModel, LostAndFound>.NewConfig()
            .Map(dest => dest.LostAndFoundId, src => src.LostAndFoundId)
            .Map(dest => dest.HouseholdId, src => src.HouseholdId)
            .Map(dest => dest.Image, src => src.Image)
            .Map(dest => dest.TextMessage, src => src.TextMessage)
            .Map(dest => dest.RegistrationDate, src => src.RegistrationDate)
            .Map(dest => dest.Household, src => src.Household);

        // Household mapping
        TypeAdapterConfig<Household, HouseholdModel>.NewConfig()
            .Map(dest => dest.HouseholdId, src => src.HouseholdId)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Addresses, src => src.Addresses)
            .Map(dest => dest.Slots, src => src.Slots)
            .Map(dest => dest.LaundryReservations, src => src.LaundryReservations)
            .Map(dest => dest.ChatMessages, src => src.ChatMessages)
            .Map(dest => dest.LostAndFoundItems, src => src.LostAndFoundItems);

        TypeAdapterConfig<HouseholdModel, Household>.NewConfig()
            .Map(dest => dest.HouseholdId, src => src.HouseholdId)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Addresses, src => src.Addresses)
            .Map(dest => dest.Slots, src => src.Slots)
            .Map(dest => dest.LaundryReservations, src => src.LaundryReservations)
            .Map(dest => dest.ChatMessages, src => src.ChatMessages)
            .Map(dest => dest.LostAndFoundItems, src => src.LostAndFoundItems);
    }
}