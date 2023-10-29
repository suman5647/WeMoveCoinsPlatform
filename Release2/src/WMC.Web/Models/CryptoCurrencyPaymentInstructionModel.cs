using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMC.Web.Models
{
    public class CryptoCurrencyPaymentInstructionModel
    {
        public string Amount { get; set; }
        public string BitcoinAddress { get; set; }
        public string QRCodeImage { get; set; } 
        public string OrderNumber { get; set; }
        public string GoogleTagManagerId { get; set; }//Needed for GoogleTagManagerId
        public string Commission { get; set; } //Needed for GoogleTagManagerId
        public string Currency { get; set; } //Needed for GoogleTagManagerId
        public string CryptoCurrencyCode { get; set; }
    }
}


