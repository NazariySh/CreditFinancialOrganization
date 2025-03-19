using AutoMapper;
using CreditFinancialOrganization.Application.DTOs.Users;
using CreditFinancialOrganization.Domain.Entities.Users;

namespace CreditFinancialOrganization.Application.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RegisterDto, User>()
            .ForMember(
                dest => dest.UserName,
                opt => opt.MapFrom(src => src.Email));

        CreateMap<User, UserDto>();

        CreateMap<UserDto, User>();
    }
}