using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paylike.NET.Entities
{
    public class Destination
    {
        [JsonProperty("cardId")]
        public string cardId { get; set; }
        [JsonProperty("text")]
        public string text { get; set; } = "";
    }
}
