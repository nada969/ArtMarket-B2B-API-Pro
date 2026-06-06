using B2B_Procurement___Order_Management_Platform.ArtMarket.Application.DTOs;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Enums;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Infrastructure;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;


namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> Register(RegisterDTO authDTO);
        void Login();
        void ForgotPassword();
        void ResetPassword();
        void Logout();

    }
    public class AuthService: IAuthService
    {
        private readonly JWT _jwt;
        private readonly IAuthRepo _authRepo;
        private readonly ILogger<AuthService> _logger;


        public string Message = string.Empty;


        public AuthService (IOptions<JWT> jwt,IAuthRepo authRepo, ILogger<AuthService> logger)
        {
            _jwt = jwt.Value;
            _authRepo = authRepo;
            _logger = logger;

        }



        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var userClaims = await _authRepo.GetClaimsAsync(user);
            var role = await _authRepo.GetRolesAsync(user.UserName);
            var roleClaims = new List<Claim>();
            
            if (role.HasValue)
                roleClaims.Add(new Claim(ClaimTypes.Role, role.Value.ToString()));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

    

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public async Task<AuthResponseDTO?> Register(RegisterDTO authDTO)
        {
            /// the flow: (exist check → save → verify → token → return)

            ///1.check if user exist in database using email search:
            if (await _authRepo.UserExistAsync(authDTO.email))
            {
                _logger.LogWarning("Register attempt with existing email: {Email}", authDTO.email);
                return new AuthResponseDTO
                {
                    IsAuthenticated = false,
                    Message = "This email is already registered."
                };
            }

            ///2. save this in DB and check if it done greate
            var result = await _authRepo.Register(authDTO);
            if(result is null || !result.Succeeded)
            {
                _logger.LogWarning("Register failed for {Email}: {Errors}",
                authDTO.email,
                string.Join(", ", result?.Errors.Select(e => e.Description) ?? new[] { "null result" }));

                return new AuthResponseDTO
                {
                    IsAuthenticated = false,
                    /// use the Identity ti=o show exactly what is failed
                    Message = result?.Errors.Any() == true
                    ? string.Join(" ", result.Errors.Select(e => e.Description))
                    : "Registration failed."
                };
            }
            
            ///3. verify the register
            var user = await _authRepo.GetByEmailAsync(authDTO.email);
            if (user is null)
            {
                _logger.LogError("User not found after successful registration: {Email}", authDTO.email);
                return new AuthResponseDTO 
                { 
                    IsAuthenticated = false, 
                    Message = "Registration error. Please try again." 
                };
            }

            /// 4. generate JWT token
            var jwtSecurityToken = await CreateJwtToken(user);
            _logger.LogInformation("User registered successfully: {Email}", authDTO.email);

            /// 5. return user with his token
            return new AuthResponseDTO
            {
                IsAuthenticated = true,
                Email = authDTO.email,
                UserName = authDTO.userName,
                Message = "Register Successfully",
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn=jwtSecurityToken.ValidTo,
            };
        } 
        public void ForgotPassword()
        {
            throw new NotImplementedException();
        }

        public void Login()
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

     

        public void ResetPassword()
        {
            throw new NotImplementedException();
        }
    }

}
