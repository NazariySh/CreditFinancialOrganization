using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreditFinancialOrganization.Api.Extensions;
using CreditFinancialOrganization.Application.DTOs.Payments;
using CreditFinancialOrganization.Application.Interfaces;
using CreditFinancialOrganization.Domain.Enums;

namespace CreditFinancialOrganization.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
    }

    [HttpGet("loans/{loanId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PaymentDto>>> GetAll(
        Guid loanId,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetId();

        var payments = await _paymentService.GetAllAsync(
            loanId,
            userId,
            pageNumber,
            pageSize,
            cancellationToken);

        return Ok(payments);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetId();

        var payment = await _paymentService.GetByIdAsync(id, userId, cancellationToken);

        if (payment is null)
        {
            return NotFound();
        }

        return Ok(payment);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PaymentDto>> Create(
        PaymentCreateDto payment,
        CancellationToken cancellationToken)
    {
        await _paymentService.CreateAsync(payment, cancellationToken);

        return Ok();
    }

    [Authorize(Roles = nameof(RoleType.Employee))]
    [HttpDelete("employee/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _paymentService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}