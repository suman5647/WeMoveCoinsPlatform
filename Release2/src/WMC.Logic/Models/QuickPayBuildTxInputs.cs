using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Logic.Models
{
    class QuickPayBuildTxInput
    {
        public List<Recipients> recipients { get; set; }

        public QuickPayBuildTxInput(List<Recipients> recipients)
        {
            this.recipients = recipients;
        }
    }

    class QuickPayRecipients
    {
        public long order_id { get; set; }

        public string currency { get; set; }

        public string text_on_statement { get;  set; }

        public QuickPayRecipients(long order_id, string currency, string text_on_statement)
        {
            this.order_id = order_id;
            this.currency = currency;
            this.text_on_statement = text_on_statement;
        }
    }

    class QuickPayPaymentLink
    {
        public long amount { get; set; }

        public bool framed { get; set; }

        public string paymentMethods { get; set; }

        public string continue_url { get; set; }

        public string cancel_url { get; set; }

        //public string callback_url { get; set; }


        public QuickPayPaymentLink(long amount, bool framed, string paymentMethods,string continue_url, string cancel_url)
        {
            this.amount = amount;
            this.framed = framed;
            this.paymentMethods = paymentMethods;
            this.continue_url = continue_url;
            this.cancel_url = cancel_url;
        }
    }

    class QuickPayCapturePayment
    {
        public long amount { get; set; }

        public QuickPayCapturePayment(long amount)
        {
            this.amount = amount;
        }
    }
}

//public string payment_methods { get; set; }

//public string continue_url { get; set; }

//public string cancel_url { get; set; }

//public string callback_url { get; set; }

//public QuickPayPaymentLink(long amount, bool framed, string payment_methods, string continue_url)
//{
//    this.amount = amount;
//    this.framed = framed;
//    this.payment_methods = payment_methods;
//    //  this.continue_url = continue_url
//}
//    }
