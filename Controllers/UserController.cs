using B2B_Procurement___Order_Management_Platform.Data;
using B2B_Procurement___Order_Management_Platform.Models.Identity;
using B2B_Procurement___Order_Management_Platform.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace B2B_Procurement___Order_Management_Platform.Controllers
{
    [Route("api/v1/[controller]")] /// resourse URL: api/User
    [ApiController]

    public class UserController : ControllerBase
    {

        //public readonly UserDb _userDb;
        //public UserController(UserDb userDb)
        //{
        //    _userDb = userDb;
        //}
        /// Use Services
        public readonly IUserService _UserService;
        public UserController(IUserService userService)
        {
            _UserService = userService;
        }



        [HttpGet]   //// need to be more spacific for SWAGGER 
        public IActionResult GetUsers()
        {
            //List<User> users = _UserService.GetUsers().ToList();
            List<User> users = _UserService.GetUsers();
            return Ok(users);
        } 

        [HttpGet]   //// api/User
        [Route("{id:int}")] ///// concate to get this url: api/User/{id}
        public IActionResult GetUserId([FromRoute]int id)
        {
            /// Model Binding: looking in( if primitive--> Route(parameter OR querystriny) && if complex--> Request body)
            List<User> users = _UserService.GetUsers().ToList();
            var result = users.FirstOrDefault(x => x.Id == id);
            return Ok(result);
        }
    }
}
