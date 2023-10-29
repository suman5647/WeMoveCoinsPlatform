using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMC.Web.Models
{
    //TODO Varirables to pascal case
    public class QuickPayPaymentModel
    {
        public string paymentLink { get; set; }

        public long paymentId { get; set; }

        public string Currency { get; set; }

        public string OrderNumber { get; set; }

        public string CardNumber { get; set; }

        public decimal Amount { get; set; }

        public string AmountStr { get; set; }

    }
}