using B2B_Procurement___Order_Management_Platform.Models.Identity;
using B2B_Procurement___Order_Management_Platform.Repositories.Identity;
using Microsoft.AspNetCore.Identity;

namespace B2B_Procurement___Order_Management_Platform.Services
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
