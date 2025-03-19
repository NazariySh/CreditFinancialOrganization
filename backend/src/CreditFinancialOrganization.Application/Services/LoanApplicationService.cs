using System.Globalization;
using AutoMapper;
using CreditFinancialOrganization.Application.DTOs.Loans;
using CreditFinancialOrganization.Application.Interfaces;
using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Enums;
using CreditFinancialOrganization.Domain.Exceptions;
using CreditFinancialOrganization.Domain.Models;
using CreditFinancialOrganization.Domain.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CreditFinancialOrganization.Application.Services;

public class LoanApplicationService : ILoanApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<LoanApplicationCreateDto> _validator;

    public LoanApplicationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<LoanApplicationCreateDto> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task CreateAsync(
        LoanApplicationCreateDto dto,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ArgumentEmptyException.ThrowIfEmpty(userId);

        await _validator.ValidateAndThrowAsync(dto, cancellationToken);

        var loan = _mapper.Map<Loan>(dto);
        loan.CustomerId = userId;

        _unitOfWork.Loans.Add(loan);

        var application = new LoanApplication
        {
            Id = loan.Id,
            Date = DateTime.UtcNow,
            Status = ApplicationStatus.Pending
        };

        _unitOfWork.LoanApplications.Add(application);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateStatusAsync(
        Guid id,
        ApplicationStatus status,
        CancellationToken cancellationToken = default)
    {
        ArgumentEmptyException.ThrowIfEmpty(id);
        ArgumentNullException.ThrowIfNull(status);

        _unitOfWork.LoanApplications.UpdateStatus(id, status);

        var loanStatus = status switch
        {
            ApplicationStatus.Approved => LoanStatus.Active,
            ApplicationStatus.Rejected => LoanStatus.Rejected,
            ApplicationStatus.Pending => LoanStatus.Pending,
            _ => throw new InvalidOperationException(
                string.Format(CultureInfo.InvariantCulture, "Invalid application status: {0}", status))
        };

        _unitOfWork.Loans.UpdateStatus(id, loanStatus);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedList<LoanApplicationDto>> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0)
        {
            throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));
        }

        if (pageSize <= 0)
        {
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));
        }

        var loanApplications = await _unitOfWork.LoanApplications.GetAllAsync(
            pageNumber,
            pageSize,
            x => x.Status == ApplicationStatus.Pending,
            x => x
                .Include(l => l.Employee)
                .Include(l => l.Loan)
                    .ThenInclude(loan => loan.LoanType)
                .Include(l => l.Loan)
                    .ThenInclude(loan => loan.Customer),
            cancellationToken: cancellationToken);

        return _mapper.Map<PagedList<LoanApplicationDto>>(loanApplications);
    }
}