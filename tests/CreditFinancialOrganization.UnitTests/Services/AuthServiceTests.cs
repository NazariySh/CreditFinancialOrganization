using System.Security.Claims;
using AutoFixture;
using CreditFinancialOrganization.Application.DTOs.Auth;
using CreditFinancialOrganization.Application.Interfaces;
using CreditFinancialOrganization.Application.Services;
using CreditFinancialOrganization.Domain.Entities.Users;
using CreditFinancialOrganization.Domain.Exceptions;
using CreditFinancialOrganization.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;

namespace CreditFinancialOrganization.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITokenProvider> _tokenProviderMock;
    private readonly Mock<IValidator<LoginDto>> _validatorMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _fixture = new Fixture();

        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(),
            null!, null!, null!, null!, null!, null!, null!, null!);
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tokenProviderMock = new Mock<ITokenProvider>();
        _validatorMock = new Mock<IValidator<LoginDto>>();
        _authService = new AuthService(
            _userManagerMock.Object,
            _unitOfWorkMock.Object,
            _tokenProviderMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task LoginAsync_Should_ThrowValidationException_WhenValidationFails()
    {
        var loginDto = _fixture.Create<LoginDto>();

        SetupMockValidatorThrowsValidationException();

        var act = async () => await _authService.LoginAsync(loginDto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task LoginAsync_Should_ThrowArgumentException_WhenUserDoesNotExists()
    {
        var loginDto = _fixture.Create<LoginDto>();

        SetupMockRepositoryGetByEmail(null);

        var act = async () => await _authService.LoginAsync(loginDto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid username/password");
    }

    [Fact]
    public async Task LoginAsync_Should_ThrowArgumentException_WhenPasswordIsIncorrect()
    {
        var loginDto = _fixture.Create<LoginDto>();
        var user = _fixture.Create<User>();

        SetupMockRepositoryGetByEmail(user);
        SetupMockUserManagerCheckPassword(false);

        var act = async () => await _authService.LoginAsync(loginDto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid username/password");
    }

    [Fact]
    public async Task LoginAsync_Should_ReturnToken_WhenLoginIsSuccessful()
    {
        var user = _fixture.Create<User>();
        var loginDto = _fixture.Build<LoginDto>()
            .With(x => x.Email, user.Email)
            .Create();
        var accessToken = _fixture.Create<string>();
        var refreshToken = _fixture.Create<RefreshTokenDto>();

        SetupMockRepositoryGetByEmail(user);
        SetupMockUserManagerCheckPassword(true);
        SetupMockUserManagerGetRoles();
        SetupMockTokenProviderGenerateAccessToken(accessToken);
        SetupMockTokenProviderGenerateRefreshToken(refreshToken);
        SetupMockRepositoryUpdateToken(true);

        var result = await _authService.LoginAsync(loginDto);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be(accessToken);
        result.RefreshToken.Should().BeEquivalentTo(refreshToken);
    }

    [Fact]
    public async Task LogoutAsync_Should_ResetRefreshToken()
    {
        var userId = _fixture.Create<Guid>();

        SetupMockRepositoryUpdateToken(false);

        var act = async () => await _authService.LogoutAsync(userId);

        await act.Should().NotThrowAsync();

        _unitOfWorkMock.Verify(p => p.RefreshTokens.UpdateTokenAsync(
            userId,
            null,
            null,
            true,
            default), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_NotFoundException_WhenUserDoesNotExists()
    {
        var token = _fixture.Create<TokenDto>();
        var principal = new ClaimsPrincipal();

        SetupMockTokenProviderGetPrincipal(principal);
        SetupMockRepositoryGetByEmail(null);

        var act = async () => await _authService.RefreshTokenAsync(token);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_ThrowArgumentException_WhenRefreshTokenExpired()
    {
        var userRefreshToken = _fixture.Build<RefreshToken>()
            .With(x => x.ExpiryTime, DateTime.UtcNow.AddDays(-1))
            .Create();
        var user = _fixture.Build<User>()
            .With(x => x.RefreshToken, userRefreshToken)
            .Create();
        var token = _fixture.Build<TokenDto>()
            .With(x => x.RefreshToken, GetRefreshTokenDto(user.RefreshToken.Token!))
            .Create();
        var principal = new ClaimsPrincipal();

        SetupMockTokenProviderGetPrincipal(principal);
        SetupMockRepositoryGetByEmail(user);

        var act = async () => await _authService.RefreshTokenAsync(token);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid refresh token");
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_ReturnToken_WhenRefreshTokenIsValid()
    {
        var userRefreshToken = _fixture.Build<RefreshToken>()
            .With(x => x.ExpiryTime, DateTime.UtcNow.AddDays(5))
            .Create();
        var user = _fixture.Build<User>()
            .With(x => x.RefreshToken, userRefreshToken)
            .Create();
        var token = _fixture.Build<TokenDto>()
            .With(x => x.RefreshToken, GetRefreshTokenDto(user.RefreshToken.Token!))
            .Create();
        var accessToken = _fixture.Create<string>();
        var refreshToken = _fixture.Create<RefreshTokenDto>();
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.Name, user.Email!)])
        );

        SetupMockTokenProviderGetPrincipal(principal);
        SetupMockRepositoryGetByEmail(user);
        SetupMockUserManagerGetRoles();
        SetupMockTokenProviderGenerateAccessToken(accessToken);
        SetupMockTokenProviderGenerateRefreshToken(refreshToken);
        SetupMockRepositoryUpdateToken(false);

        var result = await _authService.RefreshTokenAsync(token);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be(accessToken);
        result.RefreshToken.Should().BeEquivalentTo(refreshToken);
    }

    private void SetupMockValidatorThrowsValidationException()
    {
        _validatorMock.Setup(p => p.ValidateAsync(
                It.Is<ValidationContext<LoginDto>>(context => context.ThrowOnFailures),
                default))
            .Throws(new ValidationException("error"));
    }

    private void SetupMockRepositoryGetByEmail(User? user)
    {
        _unitOfWorkMock.Setup(p => p.Users.GetByEmailAsync(
                It.IsAny<string>(),
                It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
                default))
            .ReturnsAsync(user);
    }

    private void SetupMockRepositoryUpdateToken(bool populateExp)
    {
        _unitOfWorkMock.Setup(p => p.RefreshTokens.UpdateTokenAsync(
                It.IsAny<Guid>(),
                It.IsAny<string?>(),
                It.IsAny<DateTime?>(),
                populateExp,
                default))
            .Returns(Task.CompletedTask);
    }

    private void SetupMockUserManagerCheckPassword(bool result)
    {
        _userManagerMock.Setup(x => x.CheckPasswordAsync(
                It.IsAny<User>(),
                It.IsAny<string>()))
            .ReturnsAsync(result);
    }

    private void SetupMockUserManagerGetRoles()
    {
        _userManagerMock.Setup(x => x.GetRolesAsync(
                It.IsAny<User>()))
            .ReturnsAsync([]);
    }

    private void SetupMockTokenProviderGenerateAccessToken(string token)
    {
        _tokenProviderMock.Setup(p => p.GenerateAccessToken(
                It.IsAny<User>(),
                It.IsAny<IEnumerable<string>>()))
            .Returns(token);
    }

    private void SetupMockTokenProviderGetPrincipal(ClaimsPrincipal principal)
    {
        _tokenProviderMock.Setup(p => p.GetPrincipalAsync(
                It.IsAny<string>()))
            .ReturnsAsync(principal);
    }

    private void SetupMockTokenProviderGenerateRefreshToken(RefreshTokenDto refreshToken)
    {
        _tokenProviderMock.Setup(p => p.GenerateRefreshToken())
            .Returns(refreshToken);
    }

    private static RefreshTokenDto GetRefreshTokenDto(string token)
    {
        return new RefreshTokenDto(token, default);
    }
}