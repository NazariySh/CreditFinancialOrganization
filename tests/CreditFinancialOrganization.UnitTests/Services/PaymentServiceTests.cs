using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using CreditFinancialOrganization.Application.DTOs.Payments;
using CreditFinancialOrganization.Application.Services;
using CreditFinancialOrganization.Domain.Entities.Payments;
using CreditFinancialOrganization.Domain.Exceptions;
using CreditFinancialOrganization.Domain.Models;
using CreditFinancialOrganization.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace CreditFinancialOrganization.UnitTests.Services;

public class PaymentServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<PaymentCreateDto>> _validatorMock;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
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
        _validatorMock = new Mock<IValidator<PaymentCreateDto>>();
        _paymentService = new PaymentService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_ThrowValidationException_WhenValidationFails()
    {
        var paymentCreateDto = _fixture.Create<PaymentCreateDto>();

        SetupMockValidatorThrowsValidationException();

        var act = async () => await _paymentService.CreateAsync(paymentCreateDto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateAsync_Should_NotThrowException_WhenValidRequest()
    {
        var payment = _fixture.Create<Payment>();
        var paymentCreateDto = _fixture.Build<PaymentCreateDto>()
            .With(p => p.Amount, payment.Amount)
            .With(p => p.LoanId, payment.LoanId)
            .With(p => p.PaymentMethod, payment.PaymentMethod)
            .Create();

        SetupMockMapper(payment);
        SetupMockRepositoryAdd(payment);

        var act = async () => await _paymentService.CreateAsync(paymentCreateDto);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteAsync_Should_ThrowNotFoundException_WhenPaymentDoesNotExists()
    {
        var id = _fixture.Create<Guid>();

        SetupMockRepositoryGetSingle(null);

        var act = async () => await _paymentService.DeleteAsync(id);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Payment with id {id} not found");
    }

    [Fact]
    public async Task DeleteAsync_Should_NotThrowException_WhenValidRequest()
    {
        var payment = _fixture.Create<Payment>();
        var id = payment.Id;

        SetupMockRepositoryGetSingle(payment);

        var act = async () => await _paymentService.DeleteAsync(id);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task GetAsync_Should_ReturnNull_WhenPaymentDoesNotExists()
    {
        var id = _fixture.Create<Guid>();
        var userId = _fixture.Create<Guid>();

        SetupMockRepositoryGet(null);

        var result = await _paymentService.GetByIdAsync(id, userId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_Should_ReturnPayment_WhenPaymentExists()
    {
        var payment = _fixture.Create<Payment>();
        var id = payment.Id;
        var userId = payment.Loan.CustomerId;
        var paymentDto = GetPaymentDto(payment);

        SetupMockRepositoryGet(payment);
        SetupMockMapper(paymentDto);

        var result = await _paymentService.GetByIdAsync(id, userId);

        result.Should().BeEquivalentTo(paymentDto);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnEmptyList_WhenNoPaymentsExists()
    {
        const int pageNumber = 1;
        const int pageSize = 10;

        var loanId = _fixture.Create<Guid>();
        var userId = _fixture.Create<Guid>();
        var payments = new PagedList<Payment>();
        var paymentsDto = new PagedList<PaymentDto>();

        SetupMockRepositoryGetAll(payments);
        SetupMockMapper(paymentsDto);

        var result = await _paymentService.GetAllAsync(loanId, userId, pageNumber, pageSize);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnPayments_WhenPaymentsExists()
    {
        const int pageNumber = 1;
        const int pageSize = 10;

        var loanId = _fixture.Create<Guid>();
        var userId = _fixture.Create<Guid>();
        var payments = _fixture.Create<PagedList<Payment>>();
        var paymentsDto = GetPaginationResponse(payments);

        SetupMockRepositoryGetAll(payments);
        SetupMockMapper(paymentsDto);

        var result = await _paymentService.GetAllAsync(loanId, userId, pageNumber, pageSize);

        result.Should().BeEquivalentTo(paymentsDto);
    }

    private void SetupMockValidatorThrowsValidationException()
    {
        _validatorMock.Setup(p => p.ValidateAsync(
                It.Is<ValidationContext<PaymentCreateDto>>(context => context.ThrowOnFailures),
                default))
            .Throws(new ValidationException("error"));
    }

    private void SetupMockRepositoryAdd(Payment payment)
    {
        _unitOfWorkMock.Setup(u => u.Payments.Add(
                It.IsAny<Payment>()))
            .Returns(payment);
    }

    private void SetupMockRepositoryGetSingle(Payment? payment)
    {
        _unitOfWorkMock.Setup(p => p.Payments.GetSingleAsync(
                It.IsAny<Expression<Func<Payment, bool>>>(),
                default,
                default))
            .ReturnsAsync(payment);
    }

    private void SetupMockRepositoryGet(Payment? payment)
    {
        _unitOfWorkMock.Setup(p => p.Payments.GetAsync(
                It.IsAny<Expression<Func<Payment, bool>>>(),
                default,
                default))
            .ReturnsAsync(payment);
    }

    private void SetupMockRepositoryGetAll(PagedList<Payment> payment)
    {
        _unitOfWorkMock.Setup(p => p.Payments.GetAllAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<Payment, bool>>>(),
                default,
                default))
            .ReturnsAsync(payment);
    }

    private void SetupMockMapper(Payment payment)
    {
        _mapperMock.Setup(m => m.Map<Payment>(
                It.IsAny<PaymentCreateDto>()))
            .Returns(payment);
    }

    private void SetupMockMapper(PaymentDto payment)
    {
        _mapperMock.Setup(m => m.Map<PaymentDto>(
                It.IsAny<Payment>()))
            .Returns(payment);
    }

    private void SetupMockMapper(PagedList<PaymentDto> payments)
    {
        _mapperMock.Setup(m => m.Map<PagedList<PaymentDto>>(
                It.IsAny<PagedList<Payment>>()))
            .Returns(payments);
    }

    private static PaymentDto GetPaymentDto(Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            Loan = payment.Loan,
            Date = payment.Date,
            Amount = payment.Amount,
            Status = payment.Status,
            PaymentMethod = payment.PaymentMethod
        };
    }

    private static PagedList<PaymentDto> GetPaginationResponse(PagedList<Payment> payment)
    {
        var items = payment.Items.Select(GetPaymentDto).ToList();

        return new PagedList<PaymentDto>
        {
            Items = items,
            PageNumber = payment.PageNumber,
            PageSize = payment.PageSize,
            TotalCount = payment.TotalCount,
            TotalPages = payment.TotalPages
        };
    }
}