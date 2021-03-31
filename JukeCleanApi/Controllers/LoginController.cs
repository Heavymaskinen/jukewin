using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JukeApiLibrary;
using JukeApiModel;
using Microsoft.AspNetCore.Mvc;

namespace JukeCleanApi.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private LoginApi controller = new LoginApi();

        [HttpGet]
        public ActionResult<LoginToken> Get([FromQuery] string userName, [FromQuery] string password)
        {
            return controller.Login(userName, password);
        }

        [HttpGet("{id}")]
        public ActionResult<List<User>> Get()
        {
            return controller.GetLoggedInUsers();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            controller.Logout(id);
        }
    }
}
