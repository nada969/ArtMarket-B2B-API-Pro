using B2B_Procurement___Order_Management_Platform.Data;
using B2B_Procurement___Order_Management_Platform.Models.Identity;
using System.Net;

namespace B2B_Procurement___Order_Management_Platform.Repositories.Identity
{
    public interface IUserRepository
    {
        List<User> GetUsers();
        //User CreateUser(string email, string password);
    }
    public class UserRepository : IUserRepository
    {
        private readonly UserDb _userDb;
        public UserRepository(UserDb users)
        {
            _userDb = users;
        }
        public List<User> GetUsers()
        {
            return _userDb.Users.ToList();
        }
        //public User CreateUser(string email, string password)
        //{
        //    var user = new User(email, password);
        //    password.GetHashCode();
        //    return user;
        //}
    }
}
