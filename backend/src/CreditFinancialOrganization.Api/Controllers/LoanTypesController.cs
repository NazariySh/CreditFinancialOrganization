using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreditFinancialOrganization.Application.DTOs.Loans;
using CreditFinancialOrganization.Application.Interfaces;
using CreditFinancialOrganization.Domain.Enums;

namespace CreditFinancialOrganization.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class LoanTypesController : ControllerBase
{
    private readonly ILoanTypeService _loanTypeService;

    public LoanTypesController(ILoanTypeService loanTypeService)
    {
        _loanTypeService = loanTypeService ?? throw new ArgumentNullException(nameof(loanTypeService));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LoanTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LoanTypeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var loanTypes = await _loanTypeService.GetAllAsync(cancellationToken);

        return Ok(loanTypes);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LoanTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanTypeDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var loanType = await _loanTypeService.GetByIdAsync(id, cancellationToken);

        if (loanType is null)
        {
            return NotFound();
        }

        return Ok(loanType);
    }

    [Authorize(Roles = nameof(RoleType.Admin))]
    [HttpPost("admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<LoanTypeDto>> Create(
        LoanTypeDto loanType,
        CancellationToken cancellationToken)
    {
        await _loanTypeService.CreateAsync(loanType, cancellationToken);

        return Ok();
    }

    [Authorize(Roles = nameof(RoleType.Admin))]
    [HttpPut("admin/{id:guid}")]
    [ProducesResponseType(typeof(LoanTypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<LoanTypeDto>> Update(
        Guid id,
        LoanTypeDto loanType,
        CancellationToken cancellationToken)
    {
        await _loanTypeService.UpdateAsync(id, loanType, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = loanType.Id }, loanType);
    }

    [Authorize(Roles = nameof(RoleType.Admin))]
    [HttpDelete("admin/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _loanTypeService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}