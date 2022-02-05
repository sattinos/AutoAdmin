using AutoAdmin.Core.Controllers;
using AutoAdmin.Infrastructure;
using AutoAdmin.Model;

namespace AutoAdmin.Controllers
{
    public class UserController : CrudController<uint, User>
    {
        public UserController(UserRepository userRepository) : base(userRepository)
        {
        }
    }
}