using AutoMapper;
using CreditFinancialOrganization.Application.DTOs.Loans;
using CreditFinancialOrganization.Application.Interfaces;
using FluentValidation;
using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Exceptions;
using CreditFinancialOrganization.Domain.Repositories;

namespace CreditFinancialOrganization.Application.Services;

public class LoanTypeService : ILoanTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<LoanTypeCreateDto> _createValidator;
    private readonly IValidator<LoanTypeUpdateDto> _updateValidator;

    public LoanTypeService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<LoanTypeCreateDto> createValidator,
        IValidator<LoanTypeUpdateDto> updateValidator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
        _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
    }

    public async Task CreateAsync(LoanTypeCreateDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken);

        if (await _unitOfWork.LoanTypes.AnyAsync(
                x => x.Name == dto.Name,
                cancellationToken))
        {
            throw new AlreadyExistsException($"Loan type with name {dto.Name} already exists");
        }

        var loanType = _mapper.Map<LoanType>(dto);

        _unitOfWork.LoanTypes.Add(loanType);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Guid id, LoanTypeUpdateDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentEmptyException.ThrowIfEmpty(id);
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.Id != id)
        {
            throw new ArgumentException("CustomerId does not match", nameof(id));
        }

        await _updateValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var loanType = await _unitOfWork.LoanTypes.GetSingleAsync(
            x => x.Id == id,
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException($"Loan type with id {id} not found");

        if (await _unitOfWork.LoanTypes.AnyAsync(
                x => x.Name == dto.Name && x.Id != dto.Id,
                cancellationToken))
        {
            throw new AlreadyExistsException($"Loan type with name {dto.Name} already exists");
        }

        _mapper.Map(dto, loanType);

        _unitOfWork.LoanTypes.Update(loanType);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ArgumentEmptyException.ThrowIfEmpty(id);

        var loanType = await _unitOfWork.LoanTypes.GetSingleAsync(
            x => x.Id == id,
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException($"Loan type with id {id} not found");

        _unitOfWork.LoanTypes.Remove(loanType);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<LoanTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ArgumentEmptyException.ThrowIfEmpty(id);

        var loanType = await _unitOfWork.LoanTypes.GetSingleAsync(
            x => x.Id == id,
            cancellationToken: cancellationToken);

        return _mapper.Map<LoanTypeDto>(loanType);
    }

    public async Task<IReadOnlyList<LoanTypeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var loanTypes = await _unitOfWork.LoanTypes.GetAllAsync(
            cancellationToken: cancellationToken);

        return _mapper.Map<IReadOnlyList<LoanTypeDto>>(loanTypes);
    }
}