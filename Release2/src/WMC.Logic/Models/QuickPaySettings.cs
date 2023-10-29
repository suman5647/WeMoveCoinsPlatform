using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Logic.Models
{
    public class QuickPaySettings
    {
        public long SiteId { get; set; }

        public string SiteName { get; set; }

        public long MerchantId { get; set; }

        public long APIAgreementId { get; set; }

        public string UserAPIKey { get; set; }

        public long PaymentWindowAgreementId { get; set; }

        public string PaymentWindowAPIKey { get; set; }

        public string MerchantPrivateKey { get; set; }

        public string PaymentMethod { get; set; }

        public bool isFramed { get; set; }

    }
}
