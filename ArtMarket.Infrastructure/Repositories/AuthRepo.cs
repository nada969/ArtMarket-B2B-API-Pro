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
        private readonly ILogger<AuthRepo> _logger;
        public AuthRepo(AppDb appDb, UserManager<User> userManager,ILogger<AuthRepo> logger)
        {
            _appDb = appDb;
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<IList<Claim>> GetClaimsAsync(User user)
        {
            return await _userManager.GetClaimsAsync(user);
        }
        public async Task<UserRole?> GetRolesAsync(string username)
        {
            var roles =
                await _appDb.Users
                .Where(u => u.UserName == username)
                .Select(u => u.Role)
                .FirstOrDefaultAsync();
        
            return roles;
        }
        public async Task<bool> UserExistAsync(string Email)
        {
            return await _userManager.FindByEmailAsync(Email) is not null;
        }

        /// search for user by his email
        public async Task<User?> GetByEmailAsync(string Email)
        {
            return await _appDb.Users.FirstOrDefaultAsync(u => u.Email == Email);
        }

        
        public async Task<IdentityResult?> Register(RegisterDTO authDTO) 
        {

            /// 1.create new user
            User newUser = new User
            {  Email= authDTO.email,
               UserName= authDTO.userName,
               CreatedAt = DateTime.UtcNow };

            //// 2.Chect the Role Selection
            if (!Enum.TryParse<UserRole>(authDTO.role, true, out var parsedRole))
            {
                //string Message = ("Register attempt with existing email: {Email}" + authDTO.email);
                return null;
            }
            newUser.Role = parsedRole;
            
            //// 3.save to dataBase
            //// using "userManager" to:hash password automaticly& normalize the email
            return await _userManager.CreateAsync(newUser,authDTO.password);
        }



    }
}
