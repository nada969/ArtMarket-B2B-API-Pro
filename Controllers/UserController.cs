using B2B_Procurement___Order_Management_Platform.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace B2B_Procurement___Order_Management_Platform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {

        public readonly IUserService _UserService;
        public UserController(IUserService userService)
        {
            _UserService = userService;
        }
        // Actions:
        /// 1- cant be private
        /// 2- cant be static
        /// 3- cant be overload
        /// Users/(method)
        //public
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _UserService.GetUsers();
            return Ok(users);

        }
    }
}
