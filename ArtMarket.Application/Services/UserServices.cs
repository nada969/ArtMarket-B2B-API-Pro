using B2B_Procurement___Order_Management_Platform.src.ArtMarket.Domain.Models.Identity;
using B2B_Procurement___Order_Management_Platform.src.ArtMarket.Infrastructure.Repositories.Identity;
using Microsoft.AspNetCore.Identity;

namespace B2B_Procurement___Order_Management_Platform.src.ArtMarket.Application.Services
{
    ///interface 
    public interface IUserService
    {
        List<User> GetUsers();
        //User CreateUser(string email, string password);
    }

    public class UserServices: IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public List<User> GetUsers()
        {
            return _userRepository.GetUsers();
        }
        //public User CreateUser(string email, string password)
        //{
        //    var user = new User(email, password);
        //    password.GetHashCode();
        //    return user;
        //}
       

    }
}
