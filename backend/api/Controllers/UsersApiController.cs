using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using bal.services;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersApiController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersApiController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("getusers")]
        public IActionResult GetUsers()
        {
            try
            {               
                var data = _userService.GetUsers();
                return new JsonResult(new { data = data, code = HttpStatusCode.OK});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }
    }
}