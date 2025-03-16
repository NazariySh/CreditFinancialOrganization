using AutoMapper;
using CreditFinancialOrganization.Application.DTOs.Users;
using CreditFinancialOrganization.Domain.Entities.Users;

namespace CreditFinancialOrganization.Application.Profiles;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<AddressDto, Address>().ReverseMap();
    }
}