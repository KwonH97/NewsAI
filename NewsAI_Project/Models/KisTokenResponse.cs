using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NewsAI_Project.Models
{
    public class KisTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = "";
    }
}
