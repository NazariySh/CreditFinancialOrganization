using AutoFixture;
using AutoMapper;
using CreditFinancialOrganization.Application.Services;
using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Repositories;
using Moq;
using System.Linq.Expressions;
using CreditFinancialOrganization.Application.DTOs.Loans;
using CreditFinancialOrganization.Application.DTOs.Users;
using CreditFinancialOrganization.Domain.Exceptions;
using CreditFinancialOrganization.Domain.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;

namespace CreditFinancialOrganization.UnitTests.Services;

public class LoanServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly LoanService _loanService;

    public LoanServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Customize<DateOnly>(composer =>
            composer.FromFactory<DateTime>(DateOnly.FromDateTime)
                .OmitAutoProperties());
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loanService = new LoanService(
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task DeleteAsync_Should_ThrowNotFoundException_WhenLoanDoesNotExists()
    {
        var id = _fixture.Create<Guid>();
        var userId = _fixture.Create<Guid>();

        SetupMockRepositoryGetSingle(null);

        var act = async () => await _loanService.DeleteAsync(id, userId);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Loan with id {id} not found");
    }

    [Fact]
    public async Task DeleteAsync_Should_ThrowForbiddenException_WhenCustomerDoesNotOwnLoan()
    {
        var loan = _fixture.Create<Loan>();
        var id = loan.Id;
        var userId = _fixture.Create<Guid>();

        SetupMockRepositoryGetSingle(loan);

        var act = async () => await _loanService.DeleteAsync(id, userId);

        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage($"You do not have permission to delete the loan with id '{id}'.");
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_WhenLoanDoesNotExists()
    {
        var id = _fixture.Create<Guid>();
        var userId = _fixture.Create<Guid>();

        SetupMockRepositoryGet(null);

        var result = await _loanService.GetByIdAsync(id, userId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnLoanDto_WhenLoanExists()
    {
        var loan = _fixture.Create<Loan>();
        var id = loan.Id;
        var userId = loan.CustomerId;
        var loanDto = GetLoanDto(loan);

        SetupMockRepositoryGet(loan);
        SetupMockMapper(loanDto);

        var result = await _loanService.GetByIdAsync(id, userId);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(loanDto);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnEmptyList_WhenNoLoansExists()
    {
        const int pageNumber = 1;
        const int pageSize = 10;

        var userId = _fixture.Create<Guid>();
        var loans = new PagedList<Loan>();
        var loansDto = new PagedList<LoanDto>();

        SetupMockRepositoryGetAll(loans);
        SetupMockMapper(loansDto);

        var result = await _loanService.GetAllAsync(userId, null, pageNumber, pageSize);

        result.Items.Should().BeEmpty();
        result.Should().BeEquivalentTo(loansDto);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnLoans_WhenLoansExists()
    {
        const int pageNumber = 1;
        const int pageSize = 10;
        var userId = _fixture.Create<Guid>();
        var loans = _fixture.Build<PagedList<Loan>>()
            .With(p => p.Items, _fixture.CreateMany<Loan>(10)
                .Select(loan => { loan.CustomerId = userId; return loan; })
                .ToList())
            .Create();

        var loansDto = GetPagedListLoanDto(loans);

        SetupMockRepositoryGetAll(loans);
        SetupMockMapper(loansDto);

        var result = await _loanService.GetAllAsync(userId, null, pageNumber, pageSize);

        result.Items.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(loansDto);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnLoans_WhenLoansExistsAndFilterByLoanTypeName()
    {
        const int pageNumber = 1;
        const int pageSize = 10;

        var userId = _fixture.Create<Guid>();
        var loans = _fixture.Build<PagedList<Loan>>()
            .With(p => p.Items, _fixture.CreateMany<Loan>(10)
                .Select(loan => { loan.CustomerId = userId; return loan; })
                .ToList())
            .Create();
        var loanTypeName = loans.Items.First().LoanType.Name;
        var filteredLoans = GetPagedListFilteredLoans(loans, loanTypeName);
        var loansDto = GetPagedListLoanDto(filteredLoans);
        
        SetupMockRepositoryGetAll(loans);
        SetupMockMapper(loansDto);

        var result = await _loanService.GetAllAsync(userId, loanTypeName, pageNumber, pageSize);

        result.Items.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(loansDto);
        result.Items.Should().OnlyContain(x => x.LoanType.Name == loanTypeName);
    }

    private void SetupMockRepositoryGetSingle(Loan? loan)
    {
        _unitOfWorkMock.Setup(p => p.Loans.GetSingleAsync(
                It.IsAny<Expression<Func<Loan, bool>>>(),
                default,
                default))
            .ReturnsAsync(loan);
    }

    private void SetupMockRepositoryGet(Loan? loan)
    {
        _unitOfWorkMock.Setup(p => p.Loans.GetAsync(
                It.IsAny<Expression<Func<Loan, bool>>>(),
                It.IsAny<Func<IQueryable<Loan>, IIncludableQueryable<Loan, object>>>(),
                default))
            .ReturnsAsync(loan);
    }

    private void SetupMockRepositoryGetAll(PagedList<Loan> loans)
    {
        _unitOfWorkMock.Setup(p => p.Loans.GetAllAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<Loan, bool>>>(),
                It.IsAny<Func<IQueryable<Loan>, IIncludableQueryable<Loan, object>>>(),
                default))
            .ReturnsAsync(loans);
    }

    private void SetupMockMapper(PagedList<LoanDto> loansDto)
    {
        _mapperMock.Setup(p => p.Map<PagedList<LoanDto>>(
                It.IsAny<PagedList<Loan>>()))
            .Returns(loansDto);
    }

    private void SetupMockMapper(LoanDto loanDto)
    {

        _mapperMock.Setup(p => p.Map<LoanDto>(
                It.IsAny<Loan>()))
            .Returns(loanDto);
    }

    private static LoanDto GetLoanDto(Loan loan)
    {
        return new LoanDto
        {
            Id = loan.Id,
            Customer = new UserDto
            {
                Id = loan.Customer.Id,
                FirstName = loan.Customer.FirstName,
                LastName = loan.Customer.LastName,
                Email = loan.Customer.Email ?? string.Empty
            },
            LoanType = new LoanTypeDto
            {
                Id = loan.LoanType.Id,
                Name = loan.LoanType.Name
            },
            Status = loan.Status,
            Amount = loan.Amount,
            InterestRate = loan.InterestRate,
            StartDate = loan.StartDate,
            EndDate = loan.EndDate
        };
    }

    private static PagedList<LoanDto> GetPagedListLoanDto(PagedList<Loan> loans)
    {
        return new PagedList<LoanDto>
        {
            Items = loans.Items.Select(GetLoanDto).ToList(),
            PageNumber = loans.PageNumber,
            PageSize = loans.PageSize,
            TotalPages = loans.TotalPages,
            TotalCount = loans.TotalCount
        };
    }

    private static PagedList<Loan> GetPagedListFilteredLoans(PagedList<Loan> loans, string loanTypeName)
    {
        var filteredLoans = loans.Items.Where(x => x.LoanType.Name == loanTypeName).ToList();

        return new PagedList<Loan>(
            filteredLoans,
            loans.PageNumber,
            loans.PageSize,
            filteredLoans.Count);
    }
}