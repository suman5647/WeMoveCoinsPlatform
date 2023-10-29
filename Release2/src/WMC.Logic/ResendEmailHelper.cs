using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using WMC.Data;
using WMC.Logic;
using WMC.Utilities;
using System.Text.RegularExpressions;
using WMC.Logic.Models;
using static WMC.Data.Enums.OrderType;
using WMC.Data.Enums;

namespace WMC.Logic
{
    public class ResendEmailHelper
    {
        //Resend email if orderId is passed
        public static void ResendMail(int orderId)
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            try
            {
                var order = DataUnitOfWork.Orders.GetById(orderId);
                var user = DataUnitOfWork.Users.GetById(order.UserId);
                var site = DataUnitOfWork.Sites.GetById(order.SiteId.Value);
                var currency = DataUnitOfWork.Currencies.GetById(order.CurrencyId);
                var cryptoCurrency = DataUnitOfWork.Currencies.GetById(order.CryptoCurrencyId);
                var cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCurrency(currency.Code);
                var currentSite = DataUnitOfWork.Sites.Get(q => q.Url == site.Url).FirstOrDefault();
                var ci = new CultureInfo(cultureInfo);

                EmailHelper.SendEmail(user.Email, (order.Type == (int)Data.Enums.OrderType.Sell ? order.PaymentType == 1 ? "SellOrderCompleted" : "OrderSellCompleted" : "OrderCompleted"), ///Pending SellOrderCompleted
                        new Dictionary<string, object>
                        {
                            { "UserFirstName", user.Fname},
                            { "OrderNumber", order.Number},
                            { "OrderAmount", order.Type == (int) Data.Enums.OrderType.Buy ? order.Amount.Value.ToString("N2", ci) : (order.Amount.Value * (1 - (Convert.ToDecimal(order.OurFee) / 100))).ToString("N2", ci)}, // ;
                            { "TransactionExtRef", order.TransactionHash },
                            { "OrderCurrency", currency.Code },
                            { "CardNumber", order.PaymentType == 1 ? order.CardNumber : "" }, // if paymenttype is credit card set the card number
                            { "OrderCommission", (order.Amount* (order.CommissionProcent / 100)).Value.ToString("N2", ci) },
                            { "OrderOurFee", (order.Amount* (order.OurFee / 100)).Value.ToString("N2", ci) },
                            { "OrderRate", order.Rate.Value.ToString("N2", ci) },
                            { "CryptoAddress", order.CryptoAddress },
                            { "TxAmount", order.BTCAmount.Value.ToString("N8", ci) },
                            { "BccTrustPilotAddress", currentSite.TrustPilotAddress},
                            { "MinersFee", (order.MinersFee.HasValue? (order.MinersFee.Value* order.Rate.Value) : 0M).ToString("N2", ci) + " " + currency.Code}
                        }, site.Text, order.BccAddress);

            }
            catch (Exception ex)
            {
                AuditLog.log("Error in ResendEmail() " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        //Resend email if orderNumber is passed
        public static void ResendMail(string orderNumber)
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            try
            {
                var orderId = DataUnitOfWork.Orders.Get(q => q.Number == orderNumber).Select(q => q.Id).FirstOrDefault();
                var order = DataUnitOfWork.Orders.GetById(orderId);
                var user = DataUnitOfWork.Users.GetById(order.UserId);
                var site = DataUnitOfWork.Sites.GetById(order.SiteId.Value);
                var currency = DataUnitOfWork.Currencies.GetById(order.CurrencyId);
                var cryptoCurrency = DataUnitOfWork.Currencies.GetById(order.CryptoCurrencyId);
                var cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCurrency(currency.Code);
                var currentSite = DataUnitOfWork.Sites.Get(q => q.Url == site.Url).FirstOrDefault();
                var ci = new CultureInfo(cultureInfo);

                EmailHelper.SendEmail(user.Email, (order.Type == (int)Data.Enums.OrderType.Sell ? order.PaymentType == 1 ? "SellOrderCompleted" : "OrderSellCompleted" : "OrderCompleted"), ///Pending SellOrderCompleted
                        new Dictionary<string, object>
                        {
                            { "UserFirstName", user.Fname},
                            { "OrderNumber", order.Number},
                            { "OrderAmount", order.Type == (int) Data.Enums.OrderType.Buy ? order.Amount.Value.ToString("N2", ci) : (order.Amount.Value * (1 - (Convert.ToDecimal(order.OurFee) / 100))).ToString("N2", ci)}, // ;
                            { "TransactionExtRef", order.TransactionHash },
                            { "OrderCurrency", currency.Code },
                            { "CardNumber", order.PaymentType == 1 ? order.CardNumber : "" }, // if paymenttype is credit card set the card number
                            { "OrderCommission", (order.Amount* (order.CommissionProcent / 100)).Value.ToString("N2", ci) },
                            { "OrderOurFee", (order.Amount* (order.OurFee / 100)).Value.ToString("N2", ci) },
                            { "OrderRate", order.Rate.Value.ToString("N2", ci) },
                            { "CryptoAddress", order.CryptoAddress },
                            { "TxAmount", order.BTCAmount.Value.ToString("N8", ci) },
                            { "BccTrustPilotAddress", currentSite.TrustPilotAddress},
                            { "MinersFee", (order.MinersFee.HasValue? (order.MinersFee.Value* order.Rate.Value) : 0M).ToString("N2", ci) + " " + currency.Code}
                        }, site.Text, order.BccAddress);

            }
            catch (Exception ex)
            {
                AuditLog.log("Error in ResendEmail() " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }
    }
}
