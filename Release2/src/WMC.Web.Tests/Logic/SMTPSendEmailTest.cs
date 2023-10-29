using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WMC.Data;
using WMC.Data.Enums;
using WMC;
using WMC.Logic;
using WMC.Utilities;

namespace WMC.Web.Tests.Logic
{
    [TestClass]
    public class SMTPSendEmailTest
    {
        //dr!Email, "RequestTxSecret", New Dictionary(Of String, Object) From
        //                   {
        //    { "UserIdentity", dr!CreditCardUserIdentity},
        //                       { "SiteName", dr!Text},
        //                       { "UserFirstName", dr!Name},
        //                       { "OrderNumber", dr!Number},
        //                       { "OrderCompleted", dr!Quoted},
        //                       { "OrderAmount", dr!Amount},
        //                       { "OrderCurrency", dr!Code},
        //                       { "CreditCard", dr!CardNumber}
        //}, dr!Text)
        [TestMethod]
        public void TestRequestTx()
        {
            EmailHelper.SendEmail("sunkuomkarsai@gmail.com", "RequestTxSecret",
                        new Dictionary<string, object>
                        {
                            { "UserIdentity", "55C59E51-7C67-47B4-8B10-22E255A8B88B" },
                            { "SiteName", "localhost" },
                            { "UserFirstName", "Omkar"}, // ;
                            { "OrderNumber", "123456" },
                            { "OrderCompleted", "2020-04-08 15:00:44.087" },
                            { "OrderAmount", "200" }, // if paymenttype is credit card set the card number
                            { "OrderCurrency", "INR" },
                            { "CreditCard", "10000000000000000" }
                        }, "localhost");
        }
        
        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void ResendEmail()
        {
            string orderNumber = "671550";
            using (MonniData context = new MonniData())
            {                
                var order = context.Orders.FirstOrDefault(x=> x.Number == orderNumber);
                var currency = context.Currencies.FirstOrDefault( x => x.Id == order.CurrencyId);
                var cryptoCurrency = context.Currencies.FirstOrDefault( x => x.Id == order.CryptoCurrencyId);
                //var cultureInfo = context.Countries.GetCultureCodeByCurrency(currency.Code);
                //var ci = new CultureInfo(cultureInfo.Code);
                var user = order.User;
                var siteId = order.SiteId;
                var site = context.Sites.FirstOrDefault(x => x.Id == siteId);
                var currentSite = site.Url;
                var trustPilotAddress = site.TrustPilotAddress;
                EmailHelper.SendEmail(user.Email, (order.Type == (int)Data.Enums.OrderType.Sell ? order.PaymentType == 1 ? "SellOrderCompleted" : "OrderSellCompleted" : "OrderCompleted"), ///Pending SellOrderCompleted
                        new Dictionary<string, object>
                        {
                            { "UserFirstName", user.Fname },
                            { "OrderNumber", order.Number },
                            { "OrderAmount", order.Type == (int)Data.Enums.OrderType.Buy ? order.Amount.Value.ToString("N2") : (order.Amount.Value * (1 - (Convert.ToDecimal(order.OurFee) / 100))).ToString("N2")}, // ;
                            { "TransactionExtRef", order.TransactionHash },
                            { "OrderCurrency", currency.Code },
                            { "CardNumber", order.PaymentType == 1 ? order.CardNumber : "" }, // if paymenttype is credit card set the card number
                            { "OrderCommission", (order.Amount * (order.CommissionProcent / 100)).Value.ToString("N2") },
                            { "OrderOurFee", (order.Amount * (order.OurFee / 100)).Value.ToString("N2") },
                            { "OrderRate", order.Rate },
                            { "CryptoAddress", order.CryptoAddress },
                            { "TxAmount", order.BTCAmount.Value.ToString("N8") },
                            { "BccTrustPilotAddress", trustPilotAddress},
                            { "MinersFee", (order.MinersFee.HasValue ? (order.MinersFee.Value * order.Rate.Value) : 0M).ToString("N2") + " " + currency.Code}
                        }, site.Text, order.BccAddress);
            }
    }
      
        public void TestSendEmailShouldSendEmailWithoutError()
        {
            string bankPaymentDetailHtmlPart = string.Empty;
            var strHtml = @"<tr>";
            strHtml += @"    <td align=""left"" background=""#m_4798224270076370977_f9f9f9"" style=""word-break:break-word;background:#f9f9f9;font-size:0px;padding:0px 25px 0px 25px;padding-right:25px;padding-left:25px"">";
            strHtml += @"        <div style=""color:#000000;font-family:Ubuntu,Helvetica,Arial,sans-serif;font-size:13px;line-height:10px"">";
            strHtml += @"            <p><span style=""font-size:14px"">" + WebUtility.HtmlEncode(Helpers.ResourceExtensions.Resource(null, "Bank_BankName", "en", "localhost")) + ":</span></p>";
            strHtml += @"        </div>";
            strHtml += @"    </td>";
            strHtml += @"    <td align=""right"" background=""#m_4798224270076370977_f9f9f9"" style=""word-break:break-word;background:#f9f9f9;font-size:0px;padding:0px 25px 0px 25px;padding-right:25px;padding-left:25px"">";
            strHtml += @"        <div style=""color:#000000;font-family:Ubuntu,Helvetica,Arial,sans-serif;font-size:13px;line-height:10px"">";
            strHtml += @"            <p style=""text-align: right;""><span style=""font-size:14px"">" + WebUtility.HtmlEncode("State Bank Of India") + "</span></p>";
            strHtml += @"        </div>";
            strHtml += @"    </td>";
            strHtml += @"</tr>";
            bankPaymentDetailHtmlPart += strHtml;
            strHtml = @"<tr>";
            strHtml += @"    <td align=""left"" background=""#m_4798224270076370977_f9f9f9"" style=""word-break:break-word;background:#f9f9f9;font-size:0px;padding:0px 25px 0px 25px;padding-right:25px;padding-left:25px"">";
            strHtml += @"        <div style=""color:#000000;font-family:Ubuntu,Helvetica,Arial,sans-serif;font-size:13px;line-height:10px"">";
            strHtml += @"            <p><span style=""font-size:14px"">" + WebUtility.HtmlEncode(Helpers.ResourceExtensions.Resource(null, "Bank_AccountNumber", "en", "localhost")) + ":</span></p>";
            strHtml += @"        </div>";
            strHtml += @"    </td>";
            strHtml += @"    <td align=""right"" background=""#m_4798224270076370977_f9f9f9"" style=""word-break:break-word;background:#f9f9f9;font-size:0px;padding:0px 25px 0px 25px;padding-right:25px;padding-left:25px"">";
            strHtml += @"        <div style=""color:#000000;font-family:Ubuntu,Helvetica,Arial,sans-serif;font-size:13px;line-height:10px"">";
            strHtml += @"            <p style=""text-align: right;""><span style=""font-size:14px"">" + WebUtility.HtmlEncode("1234567890") + "</span></p>";
            strHtml += @"        </div>";
            strHtml += @"    </td>";
            strHtml += @"</tr>";
            bankPaymentDetailHtmlPart += strHtml;

            EmailHelper.SendEmail("shijuprakasan@live.com", "PaymentInstructions", new Dictionary<string, object> {
                { "UserFirstName", "FName" },
                                { "OrderAmount", 250.ToString("N2", new CultureInfo("en")) },
                                { "OrderCurrency", "EUR" },
                                { "OrderNumber", "101" },
                                { "CryptoAddress", "2MxhAiobktNcbzvPikpYLX3jML1FpCM4ew9" },
                            }, "localhost"
                            , "shiju@blocktech.dk"
                            , bankPaymentDetailHtmlPart);
        }

        [TestMethod]
        public void SendSimpleEmailTest()
        {
            var serverSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<SMTPServerSettings2>(ConfigurationManager.AppSettings["statusEmailSettings"]);
            EmailHelper.SendSimpleEmail(serverSettings,
                  serverSettings.To.ToArray(),
                  "test",
                  "Test email",
                  serverSettings.From,
                  "Monni Apps",
                  null,
                  false);
        }

        [TestMethod]
        public void SenRequestTxSecretEmailTest()
        {
            var test = Uri.EscapeUriString("{{SiteURL}}");
            Assert.AreNotEqual(test, "{{SiteURL}}");
            // Assert.AreEqual(test, "{{SiteURL}}");
            //EmailHelper.SendEmail("shiju@blocktech.dk", "RequestTxSecret", new Dictionary<string, object>
            //                {
            //                    { "UserIdentity", "TEst123" },
            //                    { "SiteName", "test.app.blocktech.dk" },
            //                    { "UserFirstName", "Shiju Madamchery" },
            //                    { "OrderNumber", "12345678" },
            //                    { "OrderCompleted", "Quoted" },
            //                    { "OrderAmount", "0.0" },
            //                    { "OrderCurrency", "INR" },
            //                    { "CreditCard", "1000 0000 0008" },
            //                }, "test.app.blocktech.dk");
        }
    }
}
