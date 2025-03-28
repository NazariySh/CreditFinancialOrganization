using AutoMapper;
using CreditFinancialOrganization.Application.DTOs.Loans;
using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Models;

namespace CreditFinancialOrganization.Application.Profiles;

public class LoanProfile : Profile
{
    public LoanProfile()
    {
        CreateMap<Loan, LoanDto>();

        CreateMap<LoanDto, Loan>()
            .ForMember(
                dest => dest.Customer,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.CustomerId,
                opt => opt.MapFrom(src => src.Customer.Id))
            .ForMember(
                dest => dest.LoanType,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.LoanTypeId,
                opt => opt.MapFrom(src => src.LoanType.Id));

        CreateMap<PagedList<Loan>, PagedList<LoanDto>>();
    }
}