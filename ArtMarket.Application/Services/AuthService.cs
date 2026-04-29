using B2B_Procurement___Order_Management_Platform.ArtMarket.Application.DTOs;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Enums;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Infrastructure;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Infrastructure.Repositories;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Application.Services
{
    public interface IAuthService
    {
        public Task<User?> Register(RegisterDTO authDTO);
        public void Login();
        public void ForgotPassword();
        public void ResetPassword();
        public void Logout();

    }
    public class AuthService: IAuthService
    {
        //private readonly JWT _jwt;
        private readonly IAuthRepo _authRepo;
        private readonly AuthDTO _authDTO1;
        private readonly ILogger<AuthRepo> _logger;


        public AuthService( IAuthRepo authRepo, AuthDTO authDTO1, ILogger<AuthRepo> logger)
        {
            //_jwt = jwt;
            _authRepo = authRepo;
            _authDTO1 = authDTO1;
            _logger = logger;

        }


        //private async Task<JwtSecurityToken> CreateJwtToken(User user)
        //{

        //    var userClaims = await _authRepo.GetClaimsAsync(user);
        //    var roles = await _userManager.GetRolesAsync(user);
        //    var roleClaims = new List<Claim>();

        //    foreach (var role in roles)
        //        roleClaims.Add(new Claim("roles", role));

        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        //        new Claim("uid", user.Id)
        //    }
        //    .Union(userClaims)
        //    .Union(roleClaims);

        //    var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        //    var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        //    var jwtSecurityToken = new JwtSecurityToken(
        //        issuer: _jwt.Issuer,
        //        audience: _jwt.Audience,
        //        claims: claims,
        //        expires: DateTime.Now.AddDays(_jwt.DurationInDays),
        //        signingCredentials: signingCredentials);

        //    return jwtSecurityToken;
        //}           

        public async Task<User?> Register(RegisterDTO authDTO)
        {
            string Message;
            if (await _authRepo.UserExistAsync(authDTO.email))
            {
                Message = ("Register attempt with existing email: {Email}"+ authDTO.email);
                return null;
            }

            if (!Enum.TryParse<UserRole>(authDTO.role, true, out var parsedRole))
            {
                Message = ("Register attempt with existing email: {Email}" + authDTO.email);
                return null;
            }

            User? newUser = await _authRepo.Register(authDTO);

            //var jwtSecurityToken = await CreateJwtToken(new_user);
            if (newUser is null)
            {
                return _authDTO1.Fail(Message);
            }
            newUser.Role = parsedRole;
            return _authDTO1.OK(newUser.Email,"token");
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
