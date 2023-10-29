using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMC.Data;

namespace WMC.Logic.Models
{
    public class WebhookResponse
    {
        public List<TransactionsLite> Transactions { get; set; }
        public string OrderStatus { get; set; }
        public long OrderId { get; set; }
        public string ReferenceId { get; set; }
        public decimal? Rate { get; set; }
        public string Currency { get; set; }
        public string MerchantName { get; set; }
    }

    public class TransactionsLite
    {
        public long? OrderId { get; set; }
        public string Type { get; set; }
        public string ExtRef { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }

    }
}
