using FluxifyAPI.DTOs.CustomerAddress;
using FluxifyAPI.Models;

namespace FluxifyAPI.Mapper
{
    public static class CustomerAddressMapper
    {
        public static CustomerAddressDto ToDto(this CustomerAddress model)
        {
            if (model == null) return null!;
            return new CustomerAddressDto
            {
                Id = model.Id,
                CustomerId = model.CustomerId,
                ReceiverName = model.ReceiverName,
                Phone = model.Phone,
                Country = model.Country,
                Province = model.Province,
                District = model.District,
                Ward = model.Ward,
                StreetAddress = model.StreetAddress,
                IsDefault = model.IsDefault,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt
            };
        }

        public static CustomerAddress ToEntity(this CreateCustomerAddressDto dto)
        {
            if (dto == null) return null!;
            return new CustomerAddress
            {
                CustomerId = dto.CustomerId,
                ReceiverName = dto.ReceiverName,
                Phone = dto.Phone,
                Country = dto.Country,
                Province = dto.Province,
                District = dto.District,
                Ward = dto.Ward,
                StreetAddress = dto.StreetAddress,
                IsDefault = dto.IsDefault,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(this CustomerAddress model, UpdateCustomerAddressDto dto)
        {
            if (dto == null) return;
            model.ReceiverName = dto.ReceiverName;
            model.Phone = dto.Phone;
            model.Country = dto.Country;
            model.Province = dto.Province;
            model.District = dto.District;
            model.Ward = dto.Ward;
            model.StreetAddress = dto.StreetAddress;
            model.IsDefault = dto.IsDefault;
            model.UpdatedAt = DateTime.UtcNow;
        }
    }
}
