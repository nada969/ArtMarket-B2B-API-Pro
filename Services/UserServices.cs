using B2B_Procurement___Order_Management_Platform.Models.Identity_Authorization;
using Microsoft.AspNetCore.Identity;

namespace B2B_Procurement___Order_Management_Platform.Services
{
    ///interface 
    public interface IUserService
    {
        List<User> GetUsers();
        User CreateUser(string email, string password);
    }

    public class UserServices: IUserService
    {
        private readonly List<User> _users;
        public UserServices(List<User> users)
        {
            _users = users;
        }
        public List<User> GetUsers()
        {
            return this._users;
        }
        public User CreateUser(string email, string password)
        {
            var user = new User(email, password);
            password.GetHashCode();
            return user;
        }
       

    }
}
