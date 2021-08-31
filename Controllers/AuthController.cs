using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlogAngularCore.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlogAngularCore.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public IActionResult IsAuthenticated(AdminUser adminUser)
        {
            bool status = false;

            if(adminUser.Email=="batuhanarik123@gmail.com" && adminUser.Password == "131839")
            {
                status = true;
            }

            var result = new
            {
                status = status
            };
            return Ok(result);
        }
    }
}
