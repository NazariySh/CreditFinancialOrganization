using AutoMapper;
using CreditFinancialOrganization.Application.DTOs.Payments;
using CreditFinancialOrganization.Application.Interfaces;
using CreditFinancialOrganization.Domain.Entities.Payments;
using CreditFinancialOrganization.Domain.Exceptions;
using CreditFinancialOrganization.Domain.Models;
using CreditFinancialOrganization.Domain.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CreditFinancialOrganization.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<PaymentCreateDto> _validator;

    public PaymentService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<PaymentCreateDto> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task CreateAsync(PaymentCreateDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        await _validator.ValidateAndThrowAsync(dto, cancellationToken);

        var payment = _mapper.Map<Payment>(dto);

        _unitOfWork.Payments.Add(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ArgumentEmptyException.ThrowIfEmpty(id);

        var payment = await _unitOfWork.Payments.GetSingleAsync(
            x => x.Id == id,
            cancellationToken: cancellationToken)
             ?? throw new NotFoundException($"Payment with id {id} not found");

        _unitOfWork.Payments.Remove(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<PaymentDto?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        ArgumentEmptyException.ThrowIfEmpty(id);
        ArgumentEmptyException.ThrowIfEmpty(userId);

        var payment = await _unitOfWork.Payments.GetAsync(
            p => p.Id == id && p.Loan.CustomerId == userId,
            x =>x.Include(p => p.Loan),
            cancellationToken);

        return _mapper.Map<PaymentDto>(payment);
    }

    public async Task<PagedList<PaymentDto>> GetAllAsync(
        Guid loanId,
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentEmptyException.ThrowIfEmpty(loanId);
        ArgumentEmptyException.ThrowIfEmpty(userId);

        if (pageNumber <= 0)
        {
            throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));
        }

        if (pageSize <= 0)
        {
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));
        }

        var payments = await _unitOfWork.Payments.GetAllAsync(
            pageNumber,
            pageSize,
            p => p.LoanId == loanId && p.Loan.CustomerId == userId,
            cancellationToken: cancellationToken);

        return _mapper.Map<PagedList<PaymentDto>>(payments);
    }
}