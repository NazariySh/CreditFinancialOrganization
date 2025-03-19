using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CreditFinancialOrganization.Application.DTOs.Loans;
using CreditFinancialOrganization.Application.Interfaces;
using CreditFinancialOrganization.Domain.Enums;
using CreditFinancialOrganization.Api.Extensions;

namespace CreditFinancialOrganization.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;
    private readonly ILoanApplicationService _loanApplicationService;

    public LoansController(
        ILoanService loanService,
        ILoanApplicationService loanApplicationService)
    {
        _loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
        _loanApplicationService = loanApplicationService ?? throw new ArgumentNullException(nameof(loanApplicationService));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LoanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LoanDto>>> GetAll(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetId();

        var loans = await _loanService.GetAllAsync(
            userId,
            searchTerm,
            pageNumber,
            pageSize,
            cancellationToken);

        return Ok(loans);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LoanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetId();

        var loan = await _loanService.GetByIdAsync(id, userId, cancellationToken);

        if (loan is null)
        {
            return NotFound();
        }

        return Ok(loan);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<LoanDto>> ApplyForLoan(
        LoanApplicationCreateDto application,
        CancellationToken cancellationToken)
    {
        var userId = User.GetId();

        await _loanApplicationService.CreateAsync(application, userId, cancellationToken);

        return Ok();
    }

    [Authorize(Roles = nameof(RoleType.Employee))]
    [HttpPatch("employee/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoanDto>> UpdateApplicationStatus(
        Guid id,
        [FromQuery] ApplicationStatus status,
        CancellationToken cancellationToken)
    {
        await _loanApplicationService.UpdateStatusAsync(id, status, cancellationToken);

        return Ok();
    }

    [Authorize(Roles = nameof(RoleType.Employee))]
    [HttpDelete("employee/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        await _loanService.DeleteAsync(id, userId, cancellationToken);

        return NoContent();
    }

    [Authorize(Roles = nameof(RoleType.Employee))]
    [HttpGet("employee")]
    [ProducesResponseType(typeof(IReadOnlyList<LoanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LoanDto>>> GetAll(
        Guid userId,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var loans = await _loanService.GetAllAsync(
            userId,
            searchTerm,
            pageNumber,
            pageSize,
            cancellationToken);

        return Ok(loans);
    }

    [Authorize(Roles = nameof(RoleType.Employee))]
    [HttpGet("employee/{id:guid}")]
    [ProducesResponseType(typeof(LoanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanDto>> GetById(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var loan = await _loanService.GetByIdAsync(id, userId, cancellationToken);

        if (loan is null)
        {
            return NotFound();
        }

        return Ok(loan);
    }

    [Authorize(Roles = nameof(RoleType.Employee))]
    [HttpGet("employee/applications")]
    [ProducesResponseType(typeof(IReadOnlyList<LoanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LoanDto>>> GetAllApplications(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var loans = await _loanApplicationService.GetAllAsync(
            pageNumber,
            pageSize,
            cancellationToken);

        return Ok(loans);
    }
}