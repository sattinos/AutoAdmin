using System.Dynamic;
using Newtonsoft.Json;

namespace AutoAdmin.Dto
{
    public class QueryDto
    {
        public string[] Columns { get; set; } = null;
        public string Condition { get; set; } = null;
        public string Parameters { get; set; } = null;
        public ExpandoObject ConvertParametersToExpando()
        {
            ExpandoObject parameters = null;
            if (Parameters != null)
            {
                parameters = JsonConvert.DeserializeObject<ExpandoObject>(Parameters);
            }
            return parameters;
        }
    }
}