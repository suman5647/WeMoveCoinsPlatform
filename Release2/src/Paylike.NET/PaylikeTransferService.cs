using Paylike.NET.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paylike.NET.RequestModels.Transactions;
using Paylike.NET.ResponseModels;
using Newtonsoft.Json.Linq;
using Paylike.NET.ResponseModels.Transactions;
using Paylike.NET.Entities;
using System.Net.Http;

namespace Paylike.NET
{
    public class PaylikeTransferService : BaseService, IPaylikeTransferService
    {
        public PaylikeTransferService(string privateKey) :
            base(privateKey)
        {

        }

        public ApiResponse<Transfer> CreateTransfer(CreateTransferRequest request)
        {
            return SendApiRequest<CreateTransferRequest, Transfer>(request);
        }

        public ApiResponse<Transfer> FetchTransfer(FetchTransferRequest request)
        {
            throw new NotImplementedException();
        }

        public ApiResponse<Transfer> ApproveTransfer(ApproveTransferRequest request)
        {
            throw new NotImplementedException();
        }

        protected override string ProcessApiResponse(string json, string requestName)
        {
            string processedJson = json;
            switch (requestName)
            {
                case "CreateTransfer":
                case "FetchTransfer":
                case "ApproveTransfer":
                    {
                        processedJson = JObject.Parse(json).SelectToken("transfer").ToString();
                        break;
                    }
            }

            return processedJson;
        }
    }
}
