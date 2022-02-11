using AutoAdminLib.Controllers;
using AutoAdminLib.TestWebApp.Infrastructure;
using AutoAdminLib.TestWebApp.Model;

namespace AutoAdminLib.TestWebApp.Controllers
{
    public class UserController : CrudController<uint, User>
    {
        public UserController(UserRepository userRepository) : base(userRepository)
        {
        }
    }
}