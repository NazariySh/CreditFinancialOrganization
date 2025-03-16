using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreditFinancialOrganization.Api.Extensions;
using CreditFinancialOrganization.Application.DTOs.Users;
using CreditFinancialOrganization.Application.Interfaces;
using CreditFinancialOrganization.Domain.Enums;

namespace CreditFinancialOrganization.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IUserService _userService;

    public AccountsController(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto, CancellationToken cancellationToken)
    {
        await _userService.CreateAsync(registerDto, RoleType.Customer, cancellationToken);

        return Ok();
    }

    [HttpDelete("profile")]
    public async Task<IActionResult> DeleteProfile()
    {
        var userId = User.GetId();

        await _userService.DeleteAsync(userId);

        return NoContent();
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userId = User.GetId();

        var user = await _userService.GetByIdAsync(userId, cancellationToken);

        return Ok(user);
    }

    [HttpPatch("address")]
    public async Task<IActionResult> UpdateAddress(AddressDto addressDto, CancellationToken cancellationToken)
    {
        var userId = User.GetId();

        await _userService.UpdateAddressAsync(userId, addressDto, cancellationToken);

        return NoContent();
    }

    [HttpPatch("reset-password")]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordDto changePasswordDto,
        CancellationToken cancellationToken)
    {
        var userId = User.GetId();

        await _userService.ChangePasswordAsync(userId, changePasswordDto, cancellationToken);

        return NoContent();
    }

    [Authorize(Roles = nameof(RoleType.Admin))]
    [HttpPost("admin/register/{roleName}")]
    public async Task<IActionResult> Register(
        string roleName,
        RegisterDto registerDto, 
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<RoleType>(roleName, true, out var roleType))
        {
            throw new ArgumentException("Invalid role name");
        }

        await _userService.CreateAsync(registerDto, roleType, cancellationToken);

        return Ok();
    }
}