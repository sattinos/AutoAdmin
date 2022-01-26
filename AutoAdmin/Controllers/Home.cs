using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AutoAdmin.Config;
using AutoAdmin.Infrastructure;

namespace AutoAdmin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Home : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly DbConfigurations _dbConf;

        public Home(UserRepository userRepository, IOptions<DbConfigurations> dbConf)
        {
            _userRepository = userRepository;
            _dbConf = dbConf.Value;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetManyAsync(null, null, 0, 10000);
            return Ok($"The start of AutoAdmin: users count: {users.Count()}");
        }
    }
}