using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paylike.NET.Entities
{
    public class Source
    {
        [JsonProperty("merchantId")]
        public string merchantId { get; set; }
        //public string currency { get; set; }
        //public int amount { get; set; }
        //public Fee fee { get; set; }
        //public object lineId { get; set; }
    }
}
