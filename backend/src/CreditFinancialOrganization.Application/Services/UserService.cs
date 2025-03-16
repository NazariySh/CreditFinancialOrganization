using AutoMapper;
using CreditFinancialOrganization.Application.DTOs.Users;
using CreditFinancialOrganization.Application.Interfaces;
using CreditFinancialOrganization.Domain.Entities.Users;
using CreditFinancialOrganization.Domain.Enums;
using CreditFinancialOrganization.Domain.Exceptions;
using CreditFinancialOrganization.Domain.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace CreditFinancialOrganization.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<RegisterDto> _validator;
    private readonly IValidator<AddressDto> _addressValidator;

    public UserService(
        UserManager<User> userManager,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<RegisterDto> validator,
        IValidator<AddressDto> addressValidator)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _addressValidator = addressValidator ?? throw new ArgumentNullException(nameof(addressValidator));
    }

    public async Task CreateAsync(
        RegisterDto registerDto,
        RoleType roleType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(registerDto);

        await _validator.ValidateAndThrowAsync(registerDto, cancellationToken);

        if (!await _unitOfWork.Users.IsEmailUniqueAsync(registerDto.Email, cancellationToken))
        {
            throw new AlreadyExistsException($"User with email {registerDto.Email} already exists");
        }

        var user = _mapper.Map<User>(registerDto);

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        var roleResult = await _userManager.AddToRoleAsync(user, roleType.ToString());

        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException($"Failed to add role '{roleType}'");
        }

        var refreshToken = new RefreshToken
        {
            UserId = user.Id
        };

        _unitOfWork.RefreshTokens.Add(refreshToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAddressAsync(Guid id, AddressDto addressDto, CancellationToken cancellationToken = default)
    {
        ArgumentEmptyException.ThrowIfEmpty(id);
        ArgumentNullException.ThrowIfNull(addressDto);

        await _addressValidator.ValidateAndThrowAsync(addressDto, cancellationToken);

        var address = _mapper.Map<Address>(addressDto);
        address.CustomerId = id;

        if (await _unitOfWork.Addresses.AnyAsync(x => x.CustomerId == id, cancellationToken))
        {
            _unitOfWork.Addresses.Update(address);
        }
        else
        {
            _unitOfWork.Addresses.Add(address);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangePasswordAsync(
        Guid id,
        ChangePasswordDto changePasswordDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentEmptyException.ThrowIfEmpty(id);
        ArgumentNullException.ThrowIfNull(changePasswordDto);

        var user = await _unitOfWork.Users.GetSingleAsync(
            x => x.Id == id,
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException($"User with id {id} not found");

        var result = await _userManager.ChangePasswordAsync(
            user,
            changePasswordDto.CurrentPassword,
            changePasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Failed to reset password");
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ArgumentEmptyException.ThrowIfEmpty(id);

        var user = await _unitOfWork.Users.GetSingleAsync(
            x => x.Id == id,
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException($"User with id {id} not found");

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Failed to delete user");
        }
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ArgumentEmptyException.ThrowIfEmpty(id);

        var user = await _unitOfWork.Users.GetSingleAsync(
            x => x.Id == id,
            cancellationToken: cancellationToken);

        if (user == null)
        {
            return null;
        }

        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = (await _userManager.GetRolesAsync(user)).AsReadOnly();

        return userDto;
    }
}