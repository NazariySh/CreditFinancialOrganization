using CreditFinancialOrganization.Application.DTOs.Users;
using CreditFinancialOrganization.Domain.Enums;

namespace CreditFinancialOrganization.Application.Interfaces;

public interface IUserService
{
    Task CreateAsync(
        RegisterDto registerDto,
        RoleType roleType,
        CancellationToken cancellationToken = default);

    Task UpdateAddressAsync(Guid id, AddressDto addressDto, CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(
        Guid id,
        ChangePasswordDto changePasswordDto,
        CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}