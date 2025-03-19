using AutoMapper;
using CreditFinancialOrganization.Application.DTOs.Payments;
using CreditFinancialOrganization.Domain.Entities.Payments;
using CreditFinancialOrganization.Domain.Enums;
using CreditFinancialOrganization.Domain.Models;

namespace CreditFinancialOrganization.Application.Profiles;

public class PaymentProfile : Profile
{
    public PaymentProfile()
    {
        CreateMap<Payment, PaymentDto>();

        CreateMap<PaymentDto, Payment>()
            .ForMember(
                dest => dest.Loan,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.LoanId,
                opt => opt.MapFrom(src => src.Loan.Id));


        CreateMap<PaymentCreateDto, Payment>()
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(_ => PaymentStatus.Completed))
            .ForMember(
                dest => dest.Date,
                opt => opt.MapFrom(src => DateTime.UtcNow));

        //CreateMap<Loan, LoanDto>();

        CreateMap<PagedList<Payment>, PagedList<PaymentDto>>();
    }
}