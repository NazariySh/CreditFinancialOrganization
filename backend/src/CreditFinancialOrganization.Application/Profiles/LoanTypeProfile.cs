using AutoMapper;
using CreditFinancialOrganization.Application.DTOs.Loans;
using CreditFinancialOrganization.Domain.Entities.Loans;

namespace CreditFinancialOrganization.Application.Profiles;

public class LoanTypeProfile : Profile
{
    public LoanTypeProfile()
    {
        CreateMap<LoanType, LoanTypeDto>().ReverseMap();
    }
}