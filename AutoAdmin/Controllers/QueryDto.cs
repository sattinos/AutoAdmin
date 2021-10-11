using Newtonsoft.Json.Linq;

namespace AutoAdmin.Controllers
{
    public class QueryDto
    {
        public string[] Columns { get; set; } = null;
        public string Condition { get; set; } = null;
        public string Parameters { get; set; } = null;
    }
}