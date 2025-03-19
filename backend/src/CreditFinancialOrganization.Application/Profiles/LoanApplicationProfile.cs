using AutoMapper;
using CreditFinancialOrganization.Application.DTOs.Loans;
using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Enums;
using CreditFinancialOrganization.Domain.Models;

namespace CreditFinancialOrganization.Application.Profiles;

public class LoanApplicationProfile : Profile
{
    public LoanApplicationProfile()
    {
        CreateMap<LoanApplicationCreateDto, Loan>()
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(_ => LoanStatus.Pending))
            .ForMember(
                dest => dest.EndDate,
                opt => opt.MapFrom(src => DateTime.UtcNow.AddMonths(src.LoanTermInMonths)));

        CreateMap<LoanApplication, LoanApplicationDto>();

        CreateMap<PagedList<LoanApplication>, PagedList<LoanApplicationDto>>();
    }
}