using Newtonsoft.Json;
using Paylike.NET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paylike.NET.RequestModels.Transactions
{
    public class CreateTransferRequest : RequestBase
    {
        public CreateTransferRequest()
        {
            base.Uri = "/transfers";
            base.Name = "CreateTransfer";
            base.HttpMethod = System.Net.WebRequestMethods.Http.Post;
        }

        [JsonProperty("source")]
        public Source source { get; set; }
        [JsonProperty("destination")]
        public Destination destination { get; set; }
        [JsonProperty("currency")]
        public string currency { get; set; }
        [JsonProperty("amount")]
        public int amount { get; set; }
        [JsonProperty("custom")]
        public string custom { get; set; }
    }
}
