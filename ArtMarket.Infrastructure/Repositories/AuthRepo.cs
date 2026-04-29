using B2B_Procurement___Order_Management_Platform.ArtMarket.Application.DTOs;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Enums;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models;
using B2B_Procurement___Order_Management_Platform.src.ArtMarket.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Infrastructure.Repositories
{
    public interface IAuthRepo
    {
        Task<bool> UserExistAsync(string Email);
        Task<User?> GetByEmailAsync(string Email);
        Task<User?> Register(RegisterDTO authDTO);
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

        public async Task<bool> UserExistAsync(string Email)
        {
            return await _userManager.FindByEmailAsync(Email) is not null;
        }

        /// search for user by his email
        public async Task<User?> GetByEmailAsync(string Email)
        {
            return await _appDb.Users.FirstOrDefaultAsync(u => u.Email == Email);
        }

        public async Task<User?> Register(RegisterDTO authDTO) 
        {
            
            
            /// 1.create new user
            User newUser = new User();
            newUser.Email = authDTO.email;
            newUser.UserName = authDTO.userName;
            newUser.CreatedAt = DateTime.UtcNow;
            

            /// 2.save to DB
            //await _appDb.Users.AddAsync(newUser);
            //await _appDb.SaveChangesAsync();
            //// using "userManager" to:hash password automaticly& normalize the email
            var result = await _userManager.CreateAsync(newUser, authDTO.password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    _logger.LogError("Identity error for {Email}: {Code} - {Description}",
                        authDTO.email, error.Code, error.Description);

                return null;
            }
            //Assign Identity role
            await _userManager.AddToRoleAsync(newUser, parsedRole.ToString());
            _logger.LogInformation("User registered successfully: {Email}", authDTO.email);

            /// 3.Return the new user            
            return (newUser);
        }



    }
}
