using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMC.Data;

namespace WMC.Web.Models
{
    public class PaymentInstructionsModel
    {
        public string User { get; set; }
        public string Amount { get; set; }
        public string MessageToReciever { get; set; }
        public string Bank { get; set; }
        public string RegistrationNumber { get; set; }
        public string AccountNumber { get; set; }
        public string BankAccountName { get; set; }
        public string BitcoinAddress { get; set; }
        public string Currency { get; internal set; }
        public Dictionary<string, string> PaymentDetails { get; set; }
        public string ReferenceId {get; set;}
        public string OrderStatus { get; set; }
        public DateTime? OrderDate { get; set; }
        public long OrderId { get; set; }
        public decimal? OrderAmount { get; set; }
    }
}