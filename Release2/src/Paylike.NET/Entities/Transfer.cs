using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paylike.NET.Entities
{
    public class Transfer
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public DateTime created { get; set; }
        public string identityId { get; set; }
        public bool test { get; set; }
        public Source source { get; set; }
        public Destination destination { get; set; }
        public object custom { get; set; }
        public string currency { get; set; }
        public int amount { get; set; }
        public bool approved { get; set; }
        public bool processed { get; set; }
        public Error error { get; set; }
    }

    public class TransferError
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("merchant")]
        public bool Merchant { get; set; }

        [JsonProperty("client")]
        public bool Client { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
