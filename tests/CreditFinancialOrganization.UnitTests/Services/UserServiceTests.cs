using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using CreditFinancialOrganization.Application.DTOs.Users;
using CreditFinancialOrganization.Application.Services;
using CreditFinancialOrganization.Domain.Entities.Users;
using CreditFinancialOrganization.Domain.Enums;
using CreditFinancialOrganization.Domain.Exceptions;
using CreditFinancialOrganization.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace CreditFinancialOrganization.UnitTests.Services;

public class UserServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<RegisterDto>> _validatorMock;
    private readonly Mock<IValidator<AddressDto>> _addressValidatorMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _fixture = new Fixture();

        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(),
            null!, null!, null!, null!, null!, null!, null!, null!);

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<RegisterDto>>();
        _addressValidatorMock = new Mock<IValidator<AddressDto>>();
        _userService = new UserService(
            _userManagerMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _addressValidatorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_ThrowValidationException_WhenValidationFails()
    {
        var registerDto = _fixture.Create<RegisterDto>();

        SetupMockValidatorThrowsValidationException();

        var act = async () => await _userService.CreateAsync(registerDto, RoleType.Customer);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateAsync_Should_ThrowAlreadyExistsException_WhenEmailIsNotUnique()
    {
        var registerDto = _fixture.Create<RegisterDto>();

        SetupMockRepositoryIsEmailUnique(false);

        var act = async () => await _userService.CreateAsync(registerDto, RoleType.Customer);

        await act.Should().ThrowAsync<AlreadyExistsException>();
    }

    [Fact]
    public async Task CreateAsync_Should_ThrowInvalidOperationException_WhenCreateFails()
    {
        var user = _fixture.Create<User>();
        var registerDto = _fixture.Build<RegisterDto>()
            .With(r => r.Email, user.Email)
            .Create();

        SetupMockRepositoryIsEmailUnique(true);
        SetupMockMapper(user);
        SetupMockUserManagerCreate(false);

        var act = async () => await _userService.CreateAsync(registerDto, RoleType.Customer);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task CreateAsync_Should_ThrowInvalidOperationException_WhenAddToRoleFails()
    {
        var user = _fixture.Create<User>();
        var registerDto = _fixture.Build<RegisterDto>()
            .With(r => r.Email, user.Email)
            .Create();

        SetupMockRepositoryIsEmailUnique(true);
        SetupMockMapper(user);
        SetupMockUserManagerCreate(true);
        SetupMockUserManagerAddToRole(false);

        var act = async () => await _userService.CreateAsync(registerDto, RoleType.Customer);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task CreateAsync_Should_NotThrowException_WhenAllSucceed()
    {
        var user = _fixture.Create<User>();
        var registerDto = _fixture.Build<RegisterDto>()
            .With(r => r.Email, user.Email)
            .Create();

        SetupMockRepositoryIsEmailUnique(true);
        SetupMockMapper(user);
        SetupMockUserManagerCreate(true);
        SetupMockUserManagerAddToRole(true);
        SetupMockRepositoryAdd(new RefreshToken { UserId = user.Id });

        var act = async () => await _userService.CreateAsync(registerDto, RoleType.Customer);
        await act.Should().NotThrowAsync();

        _unitOfWorkMock.Verify(
            u => u.RefreshTokens.Add(It.IsAny<RefreshToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAddress_Should_ThrowValidationException_WhenValidationFails()
    {
        var userId = _fixture.Create<Guid>();
        var addressDto = _fixture.Create<AddressDto>();

        SetupMockAddressValidatorThrowsValidationException();

        var act = async () => await _userService.UpdateAddressAsync(userId, addressDto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateAddress_Should_CreateAddress_WhenUserDoesNotHaveAddress()
    {
        var userId = _fixture.Create<Guid>();
        var address = _fixture.Create<Address>();
        var addressDto = GetAddressDto(address);

        SetupMockRepositoryAnyAddress(false);
        SetupMockMapper(address);

        var act = async () => await _userService.UpdateAddressAsync(userId, addressDto);

        await act.Should().NotThrowAsync();

        _unitOfWorkMock.Verify(
            u => u.Addresses.Add(It.IsAny<Address>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAddress_Should_UpdateAddress_WhenUserHasAddress()
    {
        var userId = _fixture.Create<Guid>();
        var address = _fixture.Create<Address>();
        var addressDto = GetAddressDto(address);

        SetupMockRepositoryAnyAddress(true);
        SetupMockMapper(address);

        var act = async () => await _userService.UpdateAddressAsync(userId, addressDto);

        await act.Should().NotThrowAsync();

        _unitOfWorkMock.Verify(
            u => u.Addresses.Update(It.IsAny<Address>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_Should_ThrowNotFoundException_WhenUserNotFound()
    {
        var userId = _fixture.Create<Guid>();
        var changePasswordDto = _fixture.Create<ChangePasswordDto>();

        SetupMockRepositoryGetSingle(null);

        var act = async () => await _userService.ChangePasswordAsync(userId, changePasswordDto);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ChangePasswordAsync_Should_ThrowInvalidOperationException_WhenChangePasswordFails()
    {
        var user = _fixture.Create<User>();
        var changePasswordDto = _fixture.Create<ChangePasswordDto>();

        SetupMockRepositoryGetSingle(user);
        SetupMockUserManagerChangePassword(false);

        var act = async () => await _userService.ChangePasswordAsync(user.Id, changePasswordDto);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ChangePasswordAsync_Should_NotThrowException_WhenAllSucceed()
    {
        var user = _fixture.Create<User>();
        var changePasswordDto = _fixture.Create<ChangePasswordDto>();

        SetupMockRepositoryGetSingle(user);
        SetupMockUserManagerChangePassword(true);

        var act = async () => await _userService.ChangePasswordAsync(user.Id, changePasswordDto);

        await act.Should().NotThrowAsync();

        _userManagerMock.Verify(
            u => u.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_ThrowNotFoundException_WhenUserNotFound()
    {
        var userId = _fixture.Create<Guid>();

        SetupMockRepositoryGetSingle(null);

        var act = async () => await _userService.DeleteAsync(userId);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_Should_ThrowInvalidOperationException_WhenDeleteFails()
    {
        var user = _fixture.Create<User>();

        SetupMockRepositoryGetSingle(user);
        SetupMockUserManagerDelete(false);

        var act = async () => await _userService.DeleteAsync(user.Id);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task DeleteAsync_Should_NotThrowException_WhenAllSucceed()
    {
        var user = _fixture.Create<User>();

        SetupMockRepositoryGetSingle(user);
        SetupMockUserManagerDelete(true);

        var act = async () => await _userService.DeleteAsync(user.Id);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_WhenUserNotFound()
    {
        var userId = _fixture.Create<Guid>();

        SetupMockRepositoryGetSingle(null);

        var result = await _userService.GetByIdAsync(userId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnUser_WhenUserExists()
    {
        var user = _fixture.Create<User>();
        var userDto = _fixture.Create<UserDto>();

        SetupMockRepositoryGetSingle(user);
        SetupMockMapper(userDto);
        SetupMockUserManagerGetRoles();

        var result = await _userService.GetByIdAsync(user.Id);

        result.Should().BeEquivalentTo(userDto);
    }

    private void SetupMockValidatorThrowsValidationException()
    {
        _validatorMock.Setup(p => p.ValidateAsync(
                It.Is<ValidationContext<RegisterDto>>(context => context.ThrowOnFailures),
                default))
            .Throws(new ValidationException("error"));
    }

    private void SetupMockAddressValidatorThrowsValidationException()
    {
        _addressValidatorMock.Setup(p => p.ValidateAsync(
                It.Is<ValidationContext<AddressDto>>(context => context.ThrowOnFailures),
                default))
            .Throws(new ValidationException("error"));
    }

    private void SetupMockRepositoryIsEmailUnique(bool result)
    {
        _unitOfWorkMock.Setup(u => u.Users.IsEmailUniqueAsync(
                It.IsAny<string>(),
                default))
            .ReturnsAsync(result);
    }

    private void SetupMockRepositoryAdd(RefreshToken token)
    {
        _unitOfWorkMock.Setup(u => u.RefreshTokens.Add(
                It.IsAny<RefreshToken>()))
            .Returns(token);
    }

    private void SetupMockRepositoryGetSingle(User? user)
    {
        _unitOfWorkMock.Setup(u => u.Users.GetSingleAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                default,
                default))
            .ReturnsAsync(user);
    }

    private void SetupMockRepositoryAnyAddress(bool result)
    {
        _unitOfWorkMock.Setup(u => u.Addresses.AnyAsync(
                It.IsAny<Expression<Func<Address, bool>>>(),
                default))
            .ReturnsAsync(result);
    }


    private void SetupMockMapper(User user)
    {
        _mapperMock.Setup(m => m.Map<User>(
                It.IsAny<RegisterDto>()))
            .Returns(user);
    }

    private void SetupMockMapper(UserDto user)
    {
        _mapperMock.Setup(m => m.Map<UserDto>(
                It.IsAny<User>()))
            .Returns(user);
    }

    private void SetupMockMapper(Address address)
    {
        _mapperMock.Setup(m => m.Map<Address>(
                It.IsAny<AddressDto>()))
            .Returns(address);
    }

    private void SetupMockUserManagerCreate(bool result)
    {
        _userManagerMock.Setup(u => u.CreateAsync(
                It.IsAny<User>(),
                It.IsAny<string>()))
            .ReturnsAsync(result ? IdentityResult.Success : IdentityResult.Failed());
    }

    private void SetupMockUserManagerAddToRole(bool result)
    {
        _userManagerMock.Setup(u => u.AddToRoleAsync(
                It.IsAny<User>(),
                It.IsAny<string>()))
            .ReturnsAsync(result ? IdentityResult.Success : IdentityResult.Failed());
    }

    private void SetupMockUserManagerDelete(bool result)
    {
        _userManagerMock.Setup(u => u.DeleteAsync(
                It.IsAny<User>()))
            .ReturnsAsync(result ? IdentityResult.Success : IdentityResult.Failed());
    }

    private void SetupMockUserManagerChangePassword(bool result)
    {
        _userManagerMock.Setup(u => u.ChangePasswordAsync(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(result ? IdentityResult.Success : IdentityResult.Failed());
    }

    private void SetupMockUserManagerGetRoles()
    {
        _userManagerMock.Setup(u => u.GetRolesAsync(
                It.IsAny<User>()))
            .ReturnsAsync([]);
    }

    //private void SetupMockUserManagerFindById(User? user)
    //{
    //    _userManagerMock.Setup(u => u.FindByIdAsync(
    //            It.IsAny<string>()))
    //        .ReturnsAsync(user);
    //}

    private static AddressDto GetAddressDto(Address address)
    {
        return new AddressDto
        {
            Line = address.Line,
            City = address.City,
            State = address.State,
            Country = address.Country,
            PostalCode = address.PostalCode
        };
    }
}