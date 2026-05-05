using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Application.DTOs;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Enums;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Infrastructure.Repositories;
using B2B_Procurement___Order_Management_Platform.src.ArtMarket.Infrastructure.Data;

namespace B2B_Procurement___Order_Management_Platform.Tests.Repositories
{
    public class AuthRepoTests
    {
        // ─── Helpers ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a fresh in-memory AppDb to avoid state leaking between tests.
        /// </summary>
        private static AppDb CreateInMemoryDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDb>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDb(options);
        }

        /// <summary>
        /// Creates a mocked UserManager with the minimum setup needed for Register tests.
        /// </summary>
        private static Mock<UserManager<User>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        // ─── HAPPY PATH ───────────────────────────────────────────────────────────

        [Fact]
        public async Task Register_ValidDTO_ReturnsIdentityResultSuccess()
        {
            // Arrange
            var db = CreateInMemoryDb(nameof(Register_ValidDTO_ReturnsIdentityResultSuccess));
            var userManagerMock = CreateUserManagerMock();

            var dto = new RegisterDTO("valid@example.com", "Password1!", "buyerUser", "Buyer")
            {
            };

            userManagerMock
                .Setup(m => m.CreateAsync(It.IsAny<User>(), dto.password))
                .ReturnsAsync(IdentityResult.Success);

            var repo = new AuthRepo(db, userManagerMock.Object);

            // Act
            var result = await repo.Register(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result!.Succeeded);
        }

        [Fact]
        public async Task Register_ValidDTO_SetsCreatedAtToUtcNow()
        {
            // Arrange
            var db = CreateInMemoryDb(nameof(Register_ValidDTO_SetsCreatedAtToUtcNow));
            var userManagerMock = CreateUserManagerMock();

            User? capturedUser = null;
            var dto = new RegisterDTO("created@example.com", "Password1!", "artistUser" , "Artist")
            {
            };

            var beforeCall = DateTime.UtcNow;

            userManagerMock
                .Setup(m => m.CreateAsync(It.IsAny<User>(), dto.password))
                .Callback<User, string>((user, _) => capturedUser = user)
                .ReturnsAsync(IdentityResult.Success);

            var repo = new AuthRepo(db, userManagerMock.Object);

            // Act
            await repo.Register(dto);
            var afterCall = DateTime.UtcNow;

            // Assert
            Assert.NotNull(capturedUser);
            Assert.True(capturedUser!.CreatedAt >= beforeCall);
            Assert.True(capturedUser.CreatedAt <= afterCall);
        }

        [Fact]
        public async Task Register_ValidRoleString_ParsesAndSetsRoleCorrectly()
        {
            // Arrange
            var db = CreateInMemoryDb(nameof(Register_ValidRoleString_ParsesAndSetsRoleCorrectly));
            var userManagerMock = CreateUserManagerMock();

            User? capturedUser = null;
            var dto = new RegisterDTO("admin@example.com", "Password1!", "adminUser", "Admin")
            {
            };

            userManagerMock
                .Setup(m => m.CreateAsync(It.IsAny<User>(), dto.password))
                .Callback<User, string>((user, _) => capturedUser = user)
                .ReturnsAsync(IdentityResult.Success);

            var repo = new AuthRepo(db, userManagerMock.Object);

            // Act
            await repo.Register(dto);

            // Assert
            Assert.Equal(UserRole.Admin, capturedUser!.Role);
        }

        [Theory]
        [InlineData("Buyer",  UserRole.Buyer)]
        [InlineData("Artist", UserRole.Artist)]
        [InlineData("Admin",  UserRole.Admin)]
        [InlineData("buyer",  UserRole.Buyer)]   // case-insensitive
        [InlineData("ARTIST", UserRole.Artist)]  // all-caps
        public async Task Register_AllValidRoleValues_ParsedCorrectly(
            string roleString, UserRole expectedRole)
        {
            // Arrange
            var db = CreateInMemoryDb($"RoleTest_{roleString}");
            var userManagerMock = CreateUserManagerMock();

            User? capturedUser = null;
            var dto = new RegisterDTO($"{roleString.ToLower()}@example.com", "Password1!", roleString + "User", roleString)
            {
               
            };

            userManagerMock
                .Setup(m => m.CreateAsync(It.IsAny<User>(), dto.password))
                .Callback<User, string>((user, _) => capturedUser = user)
                .ReturnsAsync(IdentityResult.Success);

            var repo = new AuthRepo(db, userManagerMock.Object);

            // Act
            var result = await repo.Register(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedRole, capturedUser!.Role);
        }

        // ─── EXPECTED FAILURES ────────────────────────────────────────────────────

        [Fact]
        public async Task Register_InvalidRoleString_ReturnsNull()
        {
            // Arrange
            var db = CreateInMemoryDb(nameof(Register_InvalidRoleString_ReturnsNull));
            var userManagerMock = CreateUserManagerMock();

            var dto = new RegisterDTO("badrole@example.com", "Password1!", "badRoleUser","SuperAdmin")
            {
                
            };

            var repo = new AuthRepo(db, userManagerMock.Object);

            // Act
            var result = await repo.Register(dto);

            // Assert
            Assert.Null(result);
            // UserManager.CreateAsync should never be called for an invalid role
            userManagerMock.Verify(
                m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Register_EmptyRoleString_ReturnsNull()
        {
            // Arrange
            var db = CreateInMemoryDb(nameof(Register_EmptyRoleString_ReturnsNull));
            var userManagerMock = CreateUserManagerMock();

            var dto = new RegisterDTO("empty@example.com", "Password1!", "emptyRoleUser" , "")
            {
                
            };

            var repo = new AuthRepo(db, userManagerMock.Object);

            // Act
            var result = await repo.Register(dto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Register_UserManagerFails_ReturnsFailedIdentityResult()
        {
            // Arrange
            var db = CreateInMemoryDb(nameof(Register_UserManagerFails_ReturnsFailedIdentityResult));
            var userManagerMock = CreateUserManagerMock();

            var dto = new RegisterDTO("fail@example.com", "weak", "failUser", "Buyer")
            {
                
            };

            var failedResult = IdentityResult.Failed(
                new IdentityError { Code = "PasswordTooShort", Description = "Password too short." });

            userManagerMock
                .Setup(m => m.CreateAsync(It.IsAny<User>(), dto.password))
                .ReturnsAsync(failedResult);

            var repo = new AuthRepo(db, userManagerMock.Object);

            // Act
            var result = await repo.Register(dto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result!.Succeeded);
        }
    }
}
