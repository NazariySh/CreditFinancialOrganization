using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using CreditFinancialOrganization.Application.DTOs.Loans;
using CreditFinancialOrganization.Application.Services;
using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Exceptions;
using CreditFinancialOrganization.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace CreditFinancialOrganization.UnitTests.Services;

public class LoanTypeServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<LoanTypeCreateDto>> _createValidatorMock;
    private readonly Mock<IValidator<LoanTypeUpdateDto>> _updateValidatorMock;
    private readonly LoanTypeService _loanTypeService;

    public LoanTypeServiceTests()
    {
        _fixture = new Fixture();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<LoanTypeCreateDto>>();
        _updateValidatorMock = new Mock<IValidator<LoanTypeUpdateDto>>();
        _loanTypeService = new LoanTypeService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_ThrowValidationException_WhenValidationFails()
    {
        var loanTypeDto = _fixture.Create<LoanTypeCreateDto>();

        SetupMockCreateValidatorThrowsValidationException();

        var act = async () => await _loanTypeService.CreateAsync(loanTypeDto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateAsync_Should_ThrowAlreadyExistsException_WhenLoanTypeAlreadyExists()
    {
        var loanTypeDto = _fixture.Create<LoanTypeCreateDto>();

        SetupMockRepositoryIsUnique(false);

        var act = async () => await _loanTypeService.CreateAsync(loanTypeDto);

        await act.Should().ThrowAsync<AlreadyExistsException>()
            .WithMessage($"Loan type with name {loanTypeDto.Name} already exists");
    }

    [Fact]
    public async Task CreateAsync_Should_NotThrowException_WhenValidRequest()
    {
        var loanType = _fixture.Create<LoanType>();
        var loanTypeCreate = _fixture.Build<LoanTypeCreateDto>()
            .With(p => p.Name, loanType.Name)
            .With(p => p.InterestRate, loanType.InterestRate)
            .With(p => p.Description, loanType.Description)
            .Create();

        SetupMockRepositoryIsUnique(true);
        SetupMockMapper(loanType);
        SetupMockRepositoryAdd(loanType);

        var act = async () => await _loanTypeService.CreateAsync(loanTypeCreate);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateAsync_Should_ThrowArgumentException_WhenIdDoesNotMatch()
    {
        var id = _fixture.Create<Guid>();
        var loanTypeDto = _fixture.Create<LoanTypeUpdateDto>();

        var act = async () => await _loanTypeService.UpdateAsync(id, loanTypeDto);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_Should_ThrowValidationException_WhenValidationFails()
    {
        var loanTypeDto = _fixture.Create<LoanTypeUpdateDto>();
        var id = loanTypeDto.Id;

        SetupMockUpdateValidatorThrowsValidationException();

        var act = async () => await _loanTypeService.UpdateAsync(id, loanTypeDto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateAsync_Should_ThrowNotFoundException_WhenLoanTypeDoesNotExists()
    {
        var loanTypeDto = _fixture.Create<LoanTypeUpdateDto>();
        var id = loanTypeDto.Id;

        SetupMockRepositoryGet(null);

        var act = async () => await _loanTypeService.UpdateAsync(id, loanTypeDto);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_Should_ThrowAlreadyExistsException_WhenLoanTypeAlreadyExists()
    {
        var loanType = _fixture.Create<LoanType>();
        var loanTypeUpdate = _fixture.Build<LoanTypeUpdateDto>()
            .With(p => p.Id, loanType.Id)
            .With(p => p.Name, loanType.Name)
            .With(p => p.InterestRate, loanType.InterestRate)
            .With(p => p.Description, loanType.Description)
            .Create();
        var loanTypeDto = GetLoanTypeDto(loanType);

        SetupMockRepositoryGet(loanType);
        SetupMockRepositoryIsUnique(false);

        var act = async () => await _loanTypeService.UpdateAsync(loanTypeDto.Id, loanTypeUpdate);

        await act.Should().ThrowAsync<AlreadyExistsException>()
            .WithMessage($"Loan type with name {loanTypeDto.Name} already exists");
    }

    [Fact]
    public async Task UpdateAsync_Should_NotThrowException_WhenValidRequest()
    {
        var loanType = _fixture.Create<LoanType>();
        var loanTypeUpdate = _fixture.Build<LoanTypeUpdateDto>()
            .With(p => p.Id, loanType.Id)
            .With(p => p.Name, loanType.Name)
            .With(p => p.InterestRate, loanType.InterestRate)
            .With(p => p.Description, loanType.Description)
            .Create();
        var loanTypeDto = GetLoanTypeDto(loanType);

        SetupMockRepositoryGet(loanType);
        SetupMockRepositoryIsUnique(true);
        SetupMockMapper(loanType);

        var act = async () => await _loanTypeService.UpdateAsync(loanTypeDto.Id, loanTypeUpdate);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteAsync_Should_ThrowNotFoundException_WhenLoanTypeDoesNotExists()
    {
        var id = _fixture.Create<Guid>();

        SetupMockRepositoryGet(null);

        var act = async () => await _loanTypeService.DeleteAsync(id);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_Should_NotThrowException_WhenValidRequest()
    {
        var loanType = _fixture.Create<LoanType>();

        SetupMockRepositoryGet(loanType);

        var act = async () => await _loanTypeService.DeleteAsync(loanType.Id);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task GetAsync_Should_ReturnNull_WhenLoanTypeDoesNotExists()
    {
        var id = _fixture.Create<Guid>();

        SetupMockRepositoryGet(null);

        var result = await _loanTypeService.GetByIdAsync(id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_Should_ReturnLoanType_WhenLoanTypeExists()
    {
        var loanType = _fixture.Create<LoanType>();
        var loanTypeDto = GetLoanTypeDto(loanType);

        SetupMockRepositoryGet(loanType);
        SetupMockMapper(loanTypeDto);

        var result = await _loanTypeService.GetByIdAsync(loanType.Id);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(loanTypeDto);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnEmptyList_WhenNoLoanTypesExists()
    {
        var loanTypes = new List<LoanType>();
        var loanTypesDto = new List<LoanTypeDto>();

        SetupMockRepositoryGetAll(loanTypes);
        SetupMockMapper(loanTypesDto);

        var result = await _loanTypeService.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnLoanTypes_WhenLoanTypesExists()
    {
        var loanTypes = _fixture.CreateMany<LoanType>().ToList();
        var loanTypesDto = loanTypes.Select(GetLoanTypeDto).ToList();

        SetupMockRepositoryGetAll(loanTypes);
        SetupMockMapper(loanTypesDto);

        var result = await _loanTypeService.GetAllAsync();

        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(loanTypesDto);
    }

    private void SetupMockCreateValidatorThrowsValidationException()
    {
        _createValidatorMock.Setup(p => p.ValidateAsync(
                It.Is<ValidationContext<LoanTypeCreateDto>>(context => context.ThrowOnFailures),
                CancellationToken.None))
            .Throws(new ValidationException("error"));
    }

    private void SetupMockUpdateValidatorThrowsValidationException()
    {
        _updateValidatorMock.Setup(p => p.ValidateAsync(
                It.Is<ValidationContext<LoanTypeUpdateDto>>(context => context.ThrowOnFailures),
                CancellationToken.None))
            .Throws(new ValidationException("error"));
    }

    private void SetupMockRepositoryAdd(LoanType loanType)
    {
        _unitOfWorkMock.Setup(u => u.LoanTypes.Add(
                It.IsAny<LoanType>()))
            .Returns(loanType);
    }

    private void SetupMockRepositoryGet(LoanType? loanType)
    {
        _unitOfWorkMock.Setup(p => p.LoanTypes.GetSingleAsync(
                It.IsAny<Expression<Func<LoanType, bool>>>(),
                It.IsAny<Func<IQueryable<LoanType>, IIncludableQueryable<LoanType, object>>>(),
                CancellationToken.None))
            .ReturnsAsync(loanType);
    }

    private void SetupMockRepositoryGetAll(IReadOnlyList<LoanType> loanTypes)
    {
        _unitOfWorkMock.Setup(p => p.LoanTypes.GetAllAsync(
                It.IsAny<Expression<Func<LoanType, bool>>>(),
                It.IsAny<Func<IQueryable<LoanType>, IIncludableQueryable<LoanType, object>>>(),
                CancellationToken.None))
            .ReturnsAsync(loanTypes);
    }

    private void SetupMockRepositoryIsUnique(bool result)
    {
        _unitOfWorkMock.Setup(p => p.LoanTypes.AnyAsync(
                It.IsAny<Expression<Func<LoanType, bool>>>(),
                CancellationToken.None))
            .ReturnsAsync(!result);
    }

    private void SetupMockMapper(LoanType loanType)
    {
        _mapperMock.Setup(p => p.Map<LoanType>(
                It.IsAny<LoanTypeDto>()))
            .Returns(loanType);
    }

    private void SetupMockMapper(LoanTypeDto loanTypeDto)
    {
        _mapperMock.Setup(p => p.Map<LoanTypeDto>(
                It.IsAny<LoanType>()))
            .Returns(loanTypeDto);
    }

    private void SetupMockMapper(IReadOnlyList<LoanTypeDto> loanTypes)
    {
        _mapperMock.Setup(p => p.Map<IReadOnlyList<LoanTypeDto>>(
                It.IsAny<IReadOnlyList<LoanType>>()))
            .Returns(loanTypes);
    }

    private static LoanTypeDto GetLoanTypeDto(LoanType loanType)
    {
        return new LoanTypeDto
        {
            Id = loanType.Id,
            Name = loanType.Name,
            Description = loanType.Description,
            InterestRate = loanType.InterestRate
        };
    }
}