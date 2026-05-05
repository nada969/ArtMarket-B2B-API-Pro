using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using B2B_Procurement___Order_Management_Platform.ArtMarket.API.Controllers;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Application.DTOs;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Application.Services;

namespace B2B_Procurement___Order_Management_Platform.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        // ─── HAPPY PATH ───────────────────────────────────────────────────────────

        [Fact]
        public async Task Register_ValidRequest_Returns200OkWithResponse()
        {
            // Arrange
            var dto = new RegisterDTO("test@example.com", "Password1!", "testuser", "Buyer")
            { };

            var authResponse = new AuthResponseDTO
            {
                IsAuthenticated = true,
                Email = dto.email,
                UserName = dto.userName,
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                Message = "Register Successfully",
                ExpiresOn = DateTime.UtcNow.AddDays(7)
            };

            _authServiceMock
                .Setup(s => s.Register(dto))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponseDTO>(okResult.Value);
            Assert.True(response.IsAuthenticated);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Register_ValidRequest_ResponseContainsCorrectEmailAndUsername()
        {
            // Arrange
            var dto = new RegisterDTO("artist@example.com", "Secure@123" , "artistUser" , "Artist")
            {
            };

            var authResponse = new AuthResponseDTO
            {
                IsAuthenticated = true,
                Email = dto.email,
                UserName = dto.userName,
                Token = "sample.jwt.token",
                Message = "Register Successfully"
            };

            _authServiceMock.Setup(s => s.Register(dto)).ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponseDTO>(okResult.Value);
            Assert.Equal("artist@example.com", response.Email);
            Assert.Equal("artistUser", response.UserName);
        }

        // ─── EXPECTED FAILURES ────────────────────────────────────────────────────

        [Fact]
        public async Task Register_IsAuthenticatedFalse_Returns400BadRequest()
        {
            // Arrange
            var dto = new RegisterDTO("existing@example.com", "Password1!", "BuyerUser", "Buyer");

            var failedResponse = new AuthResponseDTO
            {
                IsAuthenticated = false,
                Message = "Register Failed"
            };

            _authServiceMock.Setup(s => s.Register(dto)).ReturnsAsync(failedResponse);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_IsAuthenticatedFalse_BadRequestBodyContainsErrorMessage()
        {
            // Arrange
            var dto = new RegisterDTO("bad@example.com", "Password1!","Buyer2", "Buyer");
            var errorMessage = "Register Failed";

            var failedResponse = new AuthResponseDTO
            {
                IsAuthenticated = false,
                Message = errorMessage
            };

            _authServiceMock.Setup(s => s.Register(dto)).ReturnsAsync(failedResponse);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errorMessage, badRequest.Value);
        }

        // ─── EDGE CASES ───────────────────────────────────────────────────────────

        [Fact]
        public async Task Register_ServiceReturnsNull_ThrowsOrReturns500()
        {
            // Arrange – service returns null (unexpected path; guards the caller)
            var dto = new RegisterDTO("null@example.com", "Password1!","Buyer3", "Admin");

            _authServiceMock
                .Setup(s => s.Register(dto))
                .ReturnsAsync((AuthResponseDTO?)null);

            // Act & Assert
            // When result is null, the controller accesses result.IsAuthenticated
            // and must throw NullReferenceException (current implementation has no null guard).
            await Assert.ThrowsAsync<NullReferenceException>(
                () => _controller.Register(dto));
        }

        [Fact]
        public async Task Register_ServiceIsCalled_ExactlyOnce()
        {
            // Arrange
            var dto = new RegisterDTO("once@example.com", "Pass1!","Buyer4", "Buyer");
            _authServiceMock.Setup(s => s.Register(dto))
                .ReturnsAsync(new AuthResponseDTO { IsAuthenticated = true });

            // Act
            await _controller.Register(dto);

            // Assert
            _authServiceMock.Verify(s => s.Register(dto), Times.Once);
        }
    }
}
