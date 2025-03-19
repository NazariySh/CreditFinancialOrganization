using AutoMapper;
using CreditFinancialOrganization.Application.DTOs.Loans;
using System.Linq.Expressions;
using CreditFinancialOrganization.Application.Interfaces;
using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Exceptions;
using CreditFinancialOrganization.Domain.Models;
using CreditFinancialOrganization.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CreditFinancialOrganization.Application.Services;

public class LoanService : ILoanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LoanService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentEmptyException.ThrowIfEmpty(id);

        var loan = await _unitOfWork.Loans.GetSingleAsync(
            x => x.Id == id,
            include: x => x.Include(l => l.Application),
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException($"Loan with id {id} not found");

        if (loan.CustomerId != userId)
        {
            throw new ForbiddenException($"You do not have permission to delete the loan with id '{id}'.");
        }
        
        _unitOfWork.Loans.Remove(loan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<LoanDto?> GetByIdAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentEmptyException.ThrowIfEmpty(id);
        ArgumentEmptyException.ThrowIfEmpty(userId);

        var loan = await _unitOfWork.Loans.GetAsync(
            x => x.Id == id && x.CustomerId == userId,
            x => x
                .Include(l => l.Customer)
                .Include(l => l.LoanType)
                .Include(l => l.Application),
            cancellationToken: cancellationToken);

        return _mapper.Map<LoanDto>(loan);
    }

    public async Task<PagedList<LoanDto>> GetAllAsync(
        Guid userId,
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentEmptyException.ThrowIfEmpty(userId);

        if (pageNumber <= 0)
        {
            throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));
        }

        if (pageSize <= 0)
        {
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));
        }

        var loans = await _unitOfWork.Loans.GetAllAsync(
            pageNumber,
            pageSize,
            GetSearchFilter(search, userId),
            x => x
                .Include(l => l.Customer)
                .Include(l => l.LoanType),
            cancellationToken);

        return _mapper.Map<PagedList<LoanDto>>(loans);
    }

    private static Expression<Func<Loan, bool>> GetSearchFilter(string? searchTerm, Guid userId)
    {
        return x =>
            x.CustomerId == userId &&
            (string.IsNullOrEmpty(searchTerm) ||
             x.LoanType.Name.ToLower().Contains(searchTerm.ToLower()));
    }
}