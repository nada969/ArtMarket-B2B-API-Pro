using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Application.DTOs;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Application.Services;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Enums;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Infrastructure;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Infrastructure.Repositories;
using System.Security.Claims;

namespace B2B_Procurement___Order_Management_Platform.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepo> _authRepoMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _authService;

        // Minimal valid JWT settings so CreateJwtToken() doesn't throw
        private readonly JWT _jwtSettings = new JWT
        {
            Key = "ThisIsAVeryLongSecretKeyForTestingPurposes123!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            DurationInDays = 7
        };

        public AuthServiceTests()
        {
            _authRepoMock = new Mock<IAuthRepo>();
            _loggerMock = new Mock<ILogger<AuthService>>();

            var jwtOptions = Options.Create(_jwtSettings);
            _authService = new AuthService(jwtOptions, _authRepoMock.Object, _loggerMock.Object);
        }

        // Helper: builds a minimal valid User for JWT creation
        private User BuildValidUser(string email = "user@example.com", string username = "testuser") =>
            new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                UserName = username,
                Role = UserRole.Buyer,
                CreatedAt = DateTime.UtcNow
            };

        // ─── HAPPY PATH ───────────────────────────────────────────────────────────

        [Fact]
        public async Task Register_ValidDTO_ReturnsIsAuthenticatedTrue()
        {
            // Arrange
            var dto = new RegisterDTO("user@example.com", "Password1!", "buyerUser", "Buyer")
            {
            };
            var user = BuildValidUser(dto.email, dto.userName);

            _authRepoMock.Setup(r => r.Register(dto))
                .ReturnsAsync(IdentityResult.Success);
            _authRepoMock.Setup(r => r.GetByEmailAsync(dto.email))
                .ReturnsAsync(user);
            _authRepoMock.Setup(r => r.GetClaimsAsync(user))
                .ReturnsAsync(new List<Claim>());
            _authRepoMock.Setup(r => r.GetRolesAsync(user.UserName))
                .ReturnsAsync(UserRole.Buyer);

            // Act
            var result = await _authService.Register(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result!.IsAuthenticated);
        }

        [Fact]
        public async Task Register_ValidDTO_ReturnsNonEmptyToken()
        {
            // Arrange
            var dto = new RegisterDTO("token@example.com", "Password1!", "artistUser", "Artist")
            {
                
            };
            var user = BuildValidUser(dto.email, dto.userName);

            _authRepoMock.Setup(r => r.Register(dto)).ReturnsAsync(IdentityResult.Success);
            _authRepoMock.Setup(r => r.GetByEmailAsync(dto.email)).ReturnsAsync(user);
            _authRepoMock.Setup(r => r.GetClaimsAsync(user)).ReturnsAsync(new List<Claim>());
            _authRepoMock.Setup(r => r.GetRolesAsync(user.UserName)).ReturnsAsync(UserRole.Artist);

            // Act
            var result = await _authService.Register(dto);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(result?.Token));
        }

        [Fact]
        public async Task Register_ValidDTO_EmailAndUsernameMappedCorrectly()
        {
            // Arrange
            var dto = new RegisterDTO("mapped@example.com", "Password1!", "adminUser", "Admin")
            {
                
            };
            var user = BuildValidUser(dto.email, dto.userName);

            _authRepoMock.Setup(r => r.Register(dto)).ReturnsAsync(IdentityResult.Success);
            _authRepoMock.Setup(r => r.GetByEmailAsync(dto.email)).ReturnsAsync(user);
            _authRepoMock.Setup(r => r.GetClaimsAsync(user)).ReturnsAsync(new List<Claim>());
            _authRepoMock.Setup(r => r.GetRolesAsync(user.UserName)).ReturnsAsync(UserRole.Admin);

            // Act
            var result = await _authService.Register(dto);

            // Assert
            Assert.Equal(dto.email, result?.Email);
            Assert.Equal(dto.userName, result?.UserName);
        }

        [Fact]
        public async Task Register_ValidDTO_ExpiresOnIsInTheFuture()
        {
            // Arrange
            var dto = new RegisterDTO("expires@example.com", "Password1!", "expiresUser","Buyer")
            {
                
            };
            var user = BuildValidUser(dto.email, dto.userName);

            _authRepoMock.Setup(r => r.Register(dto)).ReturnsAsync(IdentityResult.Success);
            _authRepoMock.Setup(r => r.GetByEmailAsync(dto.email)).ReturnsAsync(user);
            _authRepoMock.Setup(r => r.GetClaimsAsync(user)).ReturnsAsync(new List<Claim>());
            _authRepoMock.Setup(r => r.GetRolesAsync(user.UserName)).ReturnsAsync(UserRole.Buyer);

            // Act
            var result = await _authService.Register(dto);

            // Assert
            Assert.True(result?.ExpiresOn > DateTime.UtcNow);
        }

        // ─── EXPECTED FAILURES ────────────────────────────────────────────────────

        [Fact]
        public async Task Register_RepoReturnsNull_ReturnsIsAuthenticatedFalse()
        {
            // Arrange
            var dto = new RegisterDTO("null@example.com", "Password1!","", "Buyer");
            _authRepoMock.Setup(r => r.Register(dto)).ReturnsAsync((IdentityResult?)null);

            // Act
            var result = await _authService.Register(dto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result!.IsAuthenticated);
            Assert.Equal("Register Failed", result.Message);
        }

        [Fact]
        public async Task Register_IdentityResultFailed_ReturnsIsAuthenticatedFalse()
        {
            // Arrange
            var dto = new RegisterDTO("fail@example.com", "weak", " ","Buyer");
            var failedResult = IdentityResult.Failed(
                new IdentityError { Description = "Password too short." });

            _authRepoMock.Setup(r => r.Register(dto)).ReturnsAsync(failedResult);

            // Act
            var result = await _authService.Register(dto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result!.IsAuthenticated);
            Assert.Equal("Register Failed", result.Message);
        }

        // ─── EDGE CASES ───────────────────────────────────────────────────────────

        [Fact]
        public async Task Register_SuccessfulRepo_GetByEmailCalledWithCorrectEmail()
        {
            // Arrange
            var dto = new RegisterDTO("verify@example.com", "Password1!","", "Buyer")
            {
                userName = "verifyUser"
            };
            var user = BuildValidUser(dto.email, dto.userName);

            _authRepoMock.Setup(r => r.Register(dto)).ReturnsAsync(IdentityResult.Success);
            _authRepoMock.Setup(r => r.GetByEmailAsync(dto.email)).ReturnsAsync(user);
            _authRepoMock.Setup(r => r.GetClaimsAsync(user)).ReturnsAsync(new List<Claim>());
            _authRepoMock.Setup(r => r.GetRolesAsync(user.UserName)).ReturnsAsync(UserRole.Buyer);

            // Act
            await _authService.Register(dto);

            // Assert – email lookup uses the same email supplied in the DTO
            _authRepoMock.Verify(r => r.GetByEmailAsync(dto.email), Times.Once);
        }
    }
}
