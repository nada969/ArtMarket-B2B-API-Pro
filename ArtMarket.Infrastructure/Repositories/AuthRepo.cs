using B2B_Procurement___Order_Management_Platform.ArtMarket.Application.DTOs;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Enums;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models;
using B2B_Procurement___Order_Management_Platform.src.ArtMarket.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Infrastructure.Repositories
{
    public interface IAuthRepo
    {
        Task<bool> UserExistAsync(string Email);
        Task<User?> GetByEmailAsync(string Email);
        Task<IdentityResult?> Register(RegisterDTO authDTO);
        Task<UserRole?> GetRolesAsync(string username);
        Task<IList<Claim>> GetClaimsAsync(User user);
    }

    public class AuthRepo : IAuthRepo
    {
        private readonly AppDb _appDb;
        private readonly UserManager<User> _userManager;
        //private readonly ILogger<AuthRepo> _logger;
        public AuthRepo(AppDb appDb, UserManager<User> userManager)
        {
            _appDb = appDb;
            _userManager = userManager;
        }

        public async Task<IList<Claim>> GetClaimsAsync(User user) => await _userManager.GetClaimsAsync(user);
        public async Task<UserRole?> GetRolesAsync(string username) => 
                 await _appDb.Users
                .Where(u => u.UserName == username)
                .Select(u => u.Role)
                .FirstOrDefaultAsync();
        public async Task<bool> UserExistAsync(string Email) => await _userManager.FindByEmailAsync(Email) is not null;
        

        /// search for user by his email
        public async Task<User?> GetByEmailAsync(string Email) => await _appDb.Users.FirstOrDefaultAsync(u => u.Email == Email);

        public async Task<IdentityResult?> Register(RegisterDTO authDTO) 
        {
            //// 2.Chect the Role Selection
            if (!Enum.TryParse<UserRole>(authDTO.role, true, out var parsedRole))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidRole",
                    Description = $"'{authDTO.role}' is not a valid role. Valid values are: Buyer, Artist, Admin."
                });
            }
            
            /// 1.create new user
            User newUser = new User
            {  Email= authDTO.email,
               UserName= authDTO.userName,
               CreatedAt = DateTime.UtcNow,
               Role = parsedRole
            };

            //// 3.save to dataBase
            //// using "userManager" to:hash password automaticly& normalize the email
            return await _userManager.CreateAsync(newUser,authDTO.password);
        }



    }
}
