using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMC.Web.Models
{
    public class PayLikePaymentModel
    {
        public string Commission { get; set; }
        public string GoogleTagManagerId { get; set; }
        public string MerchantNumber { get; set; }
        public string ShopPlatform { get; set; }
        public int Time { get; set; }
        public string Use3d { get; set; }
        public string Amount { get; set; }
        public decimal YourPayAmount { get; set; }
        public string Cardholder { get; set; }
        public string BTCaddress { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CurrencyCode { get; set; }
        public long Cartid { get; set; }
        public string Lang { get; set; }
        public string CT { get; set; }
        public string Comments { get; set; }
        public string Currency { get; set; }
        public string OrderNumber { get; set; }
        public string TxSecret { get; set; }
        public string SiteName { get; set; }
    }
}