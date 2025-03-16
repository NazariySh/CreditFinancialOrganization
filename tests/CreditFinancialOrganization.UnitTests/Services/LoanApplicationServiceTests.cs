using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using CreditFinancialOrganization.Application.DTOs.Loans;
using CreditFinancialOrganization.Application.Services;
using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Enums;
using CreditFinancialOrganization.Domain.Models;
using CreditFinancialOrganization.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace CreditFinancialOrganization.UnitTests.Services;

public class LoanApplicationServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<LoanApplicationCreateDto>> _validatorMock;
    private readonly LoanApplicationService _loanApplicationService;

    public LoanApplicationServiceTests()
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
        _validatorMock = new Mock<IValidator<LoanApplicationCreateDto>>();
        _loanApplicationService = new LoanApplicationService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_ThrowValidationException_WhenValidationFails()
    {
        var userId = _fixture.Create<Guid>();
        var loanApplicationCreateDto = _fixture.Create<LoanApplicationCreateDto>();

        SetupMockValidatorThrowsValidationException();

        var act = async () => await _loanApplicationService.CreateAsync(loanApplicationCreateDto, userId);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateAsync_Should_NotThrowException_WhenValidRequest()
    {
        var loan = _fixture.Create<Loan>();
        var userId = loan.CustomerId;
        var loanApplicationCreateDto = _fixture.Build<LoanApplicationCreateDto>()
            .With(p => p.Amount, loan.Amount)
            .With(p => p.StartDate, loan.StartDate)
            .With(p => p.LoanTypeId, loan.LoanTypeId)
            .With(p => p.InterestRate, loan.InterestRate)
            .Create();

        var loanApplication = _fixture.Build<LoanApplication>()
            .With(p => p.Id, loan.Id)
            .Create();

        SetupMockMapper(loan);
        SetupMockRepositoryAdd(loan);
        SetupMockRepositoryAdd(loanApplication);

        var act = async () => await _loanApplicationService.CreateAsync(loanApplicationCreateDto, userId);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateStatusAsync_Should_NotThrowException_WhenValidRequest()
    {
        var id = _fixture.Create<Guid>();
        var status = _fixture.Create<ApplicationStatus>();

        SetupMockRepositoryUpdateApplicationStatus();
        SetupMockRepositoryUpdateLoanStatus();

        var act = async () => await _loanApplicationService.UpdateStatusAsync(id, status);

        await act.Should().NotThrowAsync();

        _unitOfWorkMock.Verify(u => u.LoanApplications.UpdateStatus(id, status), Times.Once);
        _unitOfWorkMock.Verify(u => u.Loans.UpdateStatus(id, It.IsAny<LoanStatus>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnEmptyList_WhenNoLoanApplicationsExists()
    {
        const int pageNumber = 1;
        const int pageSize = 10;

        var loanApplications = new PagedList<LoanApplication>();
        var loanApplicationsDto = new PagedList<LoanApplicationDto>();

        SetupMockRepositoryGetAll(loanApplications);
        SetupMockMapper(loanApplicationsDto);

        var result = await _loanApplicationService.GetAllAsync(pageNumber, pageSize);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnLoanApplications_WhenLoanApplicationsExists()
    {
        const int pageNumber = 1;
        const int pageSize = 10;

        var loanApplications = _fixture.Create<PagedList<LoanApplication>>();
        var loanApplicationsDto = GetPagedListLoanApplication(loanApplications);

        SetupMockRepositoryGetAll(loanApplications);
        SetupMockMapper(loanApplicationsDto);

        var result = await _loanApplicationService.GetAllAsync(pageNumber, pageSize);

        result.Items.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(loanApplicationsDto);
    }

    private void SetupMockValidatorThrowsValidationException()
    {
        _validatorMock.Setup(p => p.ValidateAsync(
                It.Is<ValidationContext<LoanApplicationCreateDto>>(context => context.ThrowOnFailures),
                default))
            .Throws(new ValidationException("error"));
    }

    private void SetupMockRepositoryAdd(Loan loan)
    {
        _unitOfWorkMock.Setup(u => u.Loans.Add(loan))
            .Returns(loan);
    }

    private void SetupMockRepositoryAdd(LoanApplication loanApplication)
    {
        _unitOfWorkMock.Setup(u => u.LoanApplications.Add(
                loanApplication))
            .Returns(loanApplication);
    }

    private void SetupMockRepositoryUpdateApplicationStatus()
    {
        _unitOfWorkMock.Setup(u => u.LoanApplications.UpdateStatus(
                It.IsAny<Guid>(),
                It.IsAny<ApplicationStatus>()))
            .Verifiable();
    }

    private void SetupMockRepositoryUpdateLoanStatus()
    {
        _unitOfWorkMock.Setup(u => u.Loans.UpdateStatus(
                It.IsAny<Guid>(),
                It.IsAny<LoanStatus>()))
            .Verifiable();
    }

    private void SetupMockRepositoryGetAll(PagedList<LoanApplication> loanApplications)
    {
        _unitOfWorkMock.Setup(p => p.LoanApplications.GetAllAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<LoanApplication, bool>>>(),
                It.IsAny<Func<IQueryable<LoanApplication>, IIncludableQueryable<LoanApplication, object>>>(),
                default))
            .ReturnsAsync(loanApplications);
    }

    private void SetupMockMapper(Loan loan)
    {
        _mapperMock.Setup(p => p.Map<Loan>(
                It.IsAny<LoanApplicationCreateDto>()))
            .Returns(loan);
    }

    private void SetupMockMapper(PagedList<LoanApplicationDto> loanApplications)
    {
        _mapperMock.Setup(p => p.Map<PagedList<LoanApplicationDto>>(
                It.IsAny<PagedList<LoanApplication>>()))
            .Returns(loanApplications);
    }

    private static LoanApplicationDto GetLoanApplicationDto(LoanApplication loanApplication)
    {
        return new LoanApplicationDto
        {
            Id = loanApplication.Id,
            Date = loanApplication.Date,
            Status = loanApplication.Status
        };
    }

    private static PagedList<LoanApplicationDto> GetPagedListLoanApplication(PagedList<LoanApplication> loanApplications)
    {
        return new PagedList<LoanApplicationDto>
        {
            Items = loanApplications.Items.Select(GetLoanApplicationDto).ToList(),
            PageNumber = loanApplications.PageNumber,
            PageSize = loanApplications.PageSize,
            TotalCount = loanApplications.TotalCount,
            TotalPages = loanApplications.TotalPages
        };
    }
}