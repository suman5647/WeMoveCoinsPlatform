using Newtonsoft.Json;
using Paylike.NET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paylike.NET.RequestModels.Transactions
{
    public class FetchTransferRequest : RequestBase
    {
        public FetchTransferRequest()
        {
            base.UriTemplate = "/transfers/{0}";
            base.Name = "FetchTransfer";
            base.HttpMethod = System.Net.WebRequestMethods.Http.Get;
        }

        [JsonIgnore]
        private string transferId;

        [JsonIgnore]
        public string TransferId
        {
            get
            {
                return transferId;
            }
            set
            {
                transferId = value;
                base.Uri = string.Format(base.UriTemplate, transferId);
            }
        }
    }
}
