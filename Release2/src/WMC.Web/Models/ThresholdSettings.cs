using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMC.Logic;

namespace WMC.Web.Models
{
    public class ThresholdSettings
    {
        public string BankBuyAmountThreshold { get; set; }
        public string BankBuyCurrency { get; set; }
        public List<PaymentDetails> BankBuySettings { get; set; }
    }
}