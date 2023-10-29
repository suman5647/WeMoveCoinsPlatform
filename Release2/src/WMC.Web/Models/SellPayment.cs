using WMC.Logic;
using WMC.Logic.Models;

namespace WMC.Web.Models
{
    public class SellPayment
    {
        public SellPaymentDetails PaymentDetails { get; set; }
        public CryptoCurrencyPaymentInstructionModel cryptoCurrencyPaymentInstructionModel { get; set; }
        public OrderModel orderModel { get; set; }
        public TrustPayPaymentModel trustPayPaymentModel { get; set; }
    }
}