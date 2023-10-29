using MvcThrottle;
using Newtonsoft.Json;
using Paylike.NET;
using Paylike.NET.RequestModels.Transactions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using Web.ActionResults;
using WMC.Data;
using WMC.Data.Enums;
using WMC.FaceTec;
using WMC.Helpers;
using WMC.Logic;
using WMC.Logic.Models;
using WMC.Logic.TrustLogic;
using WMC.Utilities;
using WMC.Web.Filters;
using WMC.Web.ModelBinders;
using WMC.Web.Models;
using WMC.Web.Utilities;
using static WMC.Data.Enums.OrderType;
using User = WMC.Data.User;

namespace WMC.Web.Controllers
{
    public class PurchaseController : BaseController
    {
        #region Action Methods
        public void BitGoWebHook()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();
            AuditLog.log(json, (int)Data.Enums.AuditLogStatus.BitGo, (int)Data.Enums.AuditTrailLevel.Info);
            try
            {
                BitGoWebhook bitGoWebhook = JsonConvert.DeserializeObject<BitGoWebhook>(json);
                if (string.IsNullOrEmpty(bitGoWebhook.type) || string.IsNullOrEmpty(bitGoWebhook.wallet) || string.IsNullOrEmpty(bitGoWebhook.hash) || string.IsNullOrEmpty(bitGoWebhook.coin) || string.IsNullOrEmpty(bitGoWebhook.state))
                {
                    AuditLog.log(string.Format("BitGoWebHook: not satisfied"), (int)Data.Enums.AuditLogStatus.BitGo, (int)Data.Enums.AuditTrailLevel.Info);
                    return;
                }

                var cryptoCurrencies = DataUnitOfWork.Currencies.Get(curr => curr.IsActive == true && curr.CurrencyTypeId == (int)Data.Enums.CurrencyTypes.Digital).ToList();
                Dictionary<Currency, BitGoCurrencySettings> currencySettingPairs = cryptoCurrencies.ToDictionary(curr => curr, curr => JsonConvert.DeserializeObject<BitGoCurrencySettings>(curr.BitgoSettings));
                var cryptoCurrency = cryptoCurrencies.Where(q => q.Code.ToLower() == bitGoWebhook.coin.ToLower() || currencySettingPairs[q].TestCurrency.ToLower() == bitGoWebhook.coin.ToLower()).FirstOrDefault();
                Tuple<List<Tuple<string, decimal>>, DateTime, int, decimal> transactionDetail = BitGoUtil.GetTransactionDetails(bitGoWebhook.hash, cryptoCurrency.Code);
                var outputAddress = "";
                decimal txnBtcAmount = 0.0m;
                DateTime created = transactionDetail.Item2;
                //var cryptoCurrencyBitGoSettings = JsonConvert.DeserializeObject<BitGoCurrencySettings>(cryptoCurrency.BitgoSettings);
                var cryptoCurrencyBitGoSettings = currencySettingPairs[cryptoCurrency];
                if (transactionDetail.Item3 >= 0)
                {
                    long orderId = 0;
                    decimal minersFee = 0m;
                    foreach (var txnAddress in transactionDetail.Item1)
                    {
                        var filterOrder = DataUnitOfWork.Orders.Get(q => q.CryptoAddress == txnAddress.Item1).FirstOrDefault();
                        if (filterOrder != null)
                        {
                            orderId = filterOrder.Id;
                            outputAddress = txnAddress.Item1;
                            txnBtcAmount = (decimal)txnAddress.Item2 / cryptoCurrencyBitGoSettings.TxUnit;
                            minersFee = (decimal)transactionDetail.Item4 / cryptoCurrencyBitGoSettings.TxUnit;
                            break;
                        }
                    }
                    var order = DataUnitOfWork.Orders.Get(q => q.Id == orderId).FirstOrDefault();
                    if (order != null)
                    {
                        AuditLog.log("Executed BitGoWebHook(" + "created:" + created + "walletId:" + bitGoWebhook.wallet + "coin:" + cryptoCurrency.Code + "type:" + bitGoWebhook.type + "Hash:" + bitGoWebhook.hash + "txnAddress:" + outputAddress + "txnBtcAmount:" + txnBtcAmount + "OrderId:" + orderId + "MinersFee:" + minersFee.ToString() + ")", (int)Data.Enums.AuditLogStatus.BitGo, (int)Data.Enums.AuditTrailLevel.Info, orderId);

                        // Stop payout if Outgoing transaction is already made.
                        var incommingSellTransaction = DataUnitOfWork.Transactions.Get(q => q.OrderId == orderId && q.Type == 1);
                        if (incommingSellTransaction.Count() == 0)
                        {
                            var currency = DataUnitOfWork.Currencies.GetById(order.CurrencyId);
                            var rates = HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>;
                            var eurExchangeRate = OpenExchangeRates.GetEURExchangeRate(currency.Code, rates);
                            var dkkeurExchangeRate = OpenExchangeRates.GetEURExchangeRate("DKK", rates);
                            var btceurPrice = decimal.Parse(((Dictionary<string, decimal?>)HttpContext.Application["LatestBTCEURRate"])[cryptoCurrency.Code].ToString());
                            var exchangeRate = LatestCryptocurrencyRate(currency.Code, (int)Sell, cryptoCurrency.Code).Item1;
                            order.Status = (int)Data.Enums.OrderStatus.Paid;
                            order.TransactionHash = bitGoWebhook.hash;
                            order.BTCAmount = Convert.ToDecimal(txnBtcAmount);
                            order.Amount = Convert.ToDecimal(txnBtcAmount) * exchangeRate;
                            order.Rate = exchangeRate;
                            order.RateBase = decimal.Round(btceurPrice, 8);
                            order.RateHome = decimal.Round(eurExchangeRate, 8);
                            order.RateBooks = decimal.Round(dkkeurExchangeRate, 8);
                            order.MinersFee = decimal.Round(minersFee, 8);

                            DataUnitOfWork.Transactions.Add(new Transaction
                            {
                                Order = order,
                                Amount = order.BTCAmount,
                                MethodId = 3, //BTC, Bitcoin payment
                                Type = 1, // Incomming
                                ExtRef = bitGoWebhook.hash,
                                Currency = order.CryptoCurrencyId, //BTC 
                                FromAccount = DataUnitOfWork.Accounts.Get(x => (x.Type == order.Type) && (x.Currency == order.CryptoCurrencyId)
                                                   && (x.TransactionType == 1) && (x.ValueFor == "FromAccount")).Select(x => x.Id).FirstOrDefault(),
                                //25, // If order cryptocurrency is BTC then 1020 - Purchase of BTC(25), if ETH then 1021- Purchase of ETH, if LTC then 1023 - Purchase of LTC
                                ToAccount = DataUnitOfWork.Accounts.Get(x => (x.Type == order.Type) && (x.Currency == order.CryptoCurrencyId)
                                                   && (x.TransactionType == 1) && (x.ValueFor == "ToAccount")).Select(x => x.Id).FirstOrDefault(),
                                //3, //  BitGo HotWallet BTC , //TODO:: IF BITCOIN TRANSACTION THEN TOACCOUNT IS 3 (5950- BitGo HotWallet BTC)
                                //Completed = DateTime.Now, //Completed to be updated when order status changed to Completed only
                                Info = ""
                            });

                            DataUnitOfWork.Commit();

                            AuditLog.log("Order updated at BitGo Webhook. \r\nOrder details :\r\n" + JsonSerializerEx.SerializeObject(order, 1), (int)Data.Enums.AuditLogStatus.BitGo, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                            var request = "UserId:" + order.UserId + ", " + string.Format("{0}:{1}", bitGoWebhook.hash, outputAddress);
                            if (bitGoWebhook.coin.ToLower() == "btc")
                            {
                                var output = ChainalysisInterface.ReceivedOutputs(order.UserId.ToString(), string.Format("{0}:{1}", bitGoWebhook.hash, outputAddress));
                                AuditLog.log("Chainalysis ReceivedOutputs API call at BitGo Webhook. " +
                                    "\r\nChainalysis request :" + request + "\r\n" +
                                    "\r\nChainalysis response :\r\n" + JsonSerializerEx.SerializeObject(output, 1), (int)Data.Enums.AuditLogStatus.Chainalysis, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                            }
                        }
                    }
                    else
                        AuditLog.log("Executed BitGoWebHook(" + "created:" + created + "walletId:" + bitGoWebhook.wallet + "coin:" + cryptoCurrency.Code + "type:" + bitGoWebhook.type + "Hash:" + bitGoWebhook.hash + "txnAddress:" + outputAddress + "txnBtcAmount:" + txnBtcAmount + "OrderId:" + orderId + "MinersFee:" + minersFee.ToString() + ")", (int)Data.Enums.AuditLogStatus.BitGo, (int)Data.Enums.AuditTrailLevel.Info);
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in BitGoWebHook(" + json + ")\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        [EnableThrottling(PerSecond = 2, PerMinute = 20)]
        public ActionResult ValidateCouponCode(string couponCode, string lang)
        {
            dynamic result;
            try
            {
                // TODO: select
                //Coupon coupon = DataUnitOfWork.Coupons.Get(coupons => couponCode.ToUpper() == coupons.CouponCode.ToUpper() && coupons.IsActive ==true).FirstOrDefault();
                Coupon coupon = DataUnitOfWork.Coupons.Get(coupons => couponCode.ToUpper() == coupons.CouponCode.ToUpper() && coupons.IsActive == true && coupons.FromDate <= DateTime.Now && coupons.ToDate >= DateTime.Now).FirstOrDefault();
                if (coupon == null)
                    result = new BetterJsonResult<CouponModel> { Data = new CouponModel() { Validity = false, Discount = 0, ErrorMessage = "InvalidOrExpired" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                else
                    result = new BetterJsonResult<CouponModel> { Data = new CouponModel() { Validity = true, Discount = coupon.Discount }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return result;
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in ValidateCouponCode:\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult QRCode(string orderNumber)
        {
            try
            {
                // TODO: select mandatory
                var order = DataUnitOfWork.Orders.Get(q => q.Number == orderNumber).FirstOrDefault();
                if (order == null)
                {
                    AuditLog.log("Error in QRCode(" + orderNumber + " )\r\nUnable to find order.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    throw new Exception("Unable to find order with order id:  " + orderNumber);
                }

                // TODO: why [currency]?
                var currency = DataUnitOfWork.Currencies.Get(curr => order.CryptoCurrencyId == curr.Id).FirstOrDefault();
                var ciQR = new CultureInfo("en-US"); //get culture info

                string qrCode = string.Format("bitcoin:{0}?amount={1}", order.CryptoAddress, order.BTCAmount.Value.ToString("N8", ciQR));
                //             string qrCode = (currency.Text).ToLower() + ":" + order.CryptoAddress + "?amount=" + order.BTCAmount;

                byte[] buffer = QRCodeHelper.GenerateQRCode(qrCode);

                return File(buffer, System.Net.Mime.MediaTypeNames.Image.Jpeg);
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;
                AuditLog.log("Error in QRCode(" + orderNumber + " ):\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        // TODO: get miners fee or get balance??
        public ActionResult MinersFee(string cryptoCurrency)
        {
            var balance = BitGoUtil.GetBalance(cryptoCurrency);
            return new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = balance };
            // TODO: remove below code?
            var email = "vimalkumar.n@gmail.com";
            var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = "" };
            EmailHelper.SendEmail(email, "OrderCompleted", new Dictionary<string, object>
            {
                { "UserIdentity", "235433B5-FFC1-4DF0-9DED-B55D8B03B96F" },
                { "UserFirstName", "UserFirstName" },
                { "OrderNumber", "123456" },
                { "OrderCompleted", "QuotedDate" },
                { "OrderAmount", "8000" },
                { "CreditCard", "XXXXXXXXXXXXXXXXX" },
                { "BccTrustPilotAddress", "uday.wmc@gmail.com" },
            }, "apptest.wemovecoins.com", email);

            return result;
        }

        /// <summary>
        /// Method for initial setup
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="BTCAmount"></param>
        /// <param name="currency"></param>
        /// <param name="paymentMethod"></param>
        /// <param name="cryptoAddress"></param>
        /// <param name="name"></param>
        /// <param name="phoneCode"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="email"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [EnableThrottling(PerSecond = 2, PerMinute = 10)]
        public ActionResult Index(string amount = null, string currency = null, string sellCurrency = null, string cryptoCurrency = null, string paymentMethod = null,
            string cryptoAddress = null, string name = null, string phoneCode = null, string phoneNumber = null, string email = null, string returnUrl = null,
            string bcc = null, string partnerId = "", bool compact = false, string dev = null, int type = 0, string id = "DefaultTrustPayID",
            string couponCode = null, string Reg = "", string IBAN = "", string SwiftCode = "", string AccountNumber = "", string referenceId = "", string merchantCode = "")
        {
            try
            {
                string operationalStatus = "openservice";
                string reCaptchaPublicKey = string.Empty;
                string reCaptchaStatus = string.Empty;

                //get details of current site
                var currentSite = GetCurrentSite();
                //get operational status from app settings
                ReceiptModel receiptModel = null;

                string paymentProvider = (Request.Form["PaymentProvider"]);
                bool isDirectindex = true;
                if (paymentProvider != null && paymentProvider == "QuickPay")
                {
                    isDirectindex = false;
                    int PaymentId = Convert.ToInt32(Request.Form["PaymentId"]);
                    int OrderNumber = Convert.ToInt32(Request.Form["OrderNumber"]);
                    decimal Amount = Convert.ToDecimal(Request.Form["Amount"]);
                    receiptModel = QuickPay(PaymentId, OrderNumber, Amount);
                    if (type == 0) type = (int)Buy;
                }
                else if (id != "DefaultTrustPayID")
                {
                    isDirectindex = false;
                    string orderNumber = "OrderNumber:";
                    try
                    {
                        var paymentStatus = new TrustPayService().PaymentStatus(id, currentSite.Id);
                        if (System.Text.RegularExpressions.Regex.IsMatch(paymentStatus.result.code, "^000.000.|^000.100.1|^000.[36]|^000.400.0[^3]0|^000.400.100"))
                        {
                            orderNumber += paymentStatus.customParameters.SHOPPER_OrderNumber;
                            var order = DataUnitOfWork.Orders.Get(q => q.Number == paymentStatus.customParameters.SHOPPER_OrderNumber).FirstOrDefault();
                            var user = DataUnitOfWork.Users.GetById(order.UserId);
                            var purchaseCurrency = DataUnitOfWork.Currencies.GetById(order.CurrencyId).Code;
                            var eurRate = OpenExchangeRates.GetEURExchangeRate(purchaseCurrency, HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>);
                            var kycRequirement = GetKYCRequirement(user, order.Amount.Value, purchaseCurrency, eurRate, false, paymentMethod);
                            AuditLog.log("PaymentStatus response(" + id + ") for order with phone number " + user.Phone + ":" + SimpleJson.SerializeObject(paymentStatus), (int)Data.Enums.AuditLogStatus.TrustPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                            if (order.Type == (int)Sell)
                                order.Status = (int)Data.Enums.OrderStatus.Paying;
                            else
                            {
                                order.CreditCardUserIdentity = Guid.NewGuid();
                                if (kycRequirement == "NONE") //if KycRequirement is NONE change order status to Sending else change to Paid
                                    order.Status = (int)Data.Enums.OrderStatus.Sending;
                                else
                                    order.Status = (int)Data.Enums.OrderStatus.Paid;
                                //TODO later for credit cards
                                var transaction = new Transaction
                                {
                                    Amount = order.Amount,
                                    MethodId = 1,
                                    Type = 1,
                                    ExtRef = paymentStatus.id,
                                    Currency = order.CurrencyId,
                                    FromAccount = 1,
                                    ToAccount = 8,
                                    Completed = DateTime.Now,
                                    Info = paymentStatus.card.bin + "XXXXXX" + paymentStatus.card.last4Digits + "," + order.Name
                                };
                                order.Transactions.Add(transaction);
                                AuditLog.log("Updated Order(" + order.Id + ") transaction " + JsonSerializerEx.SerializeObject(transaction, 1) + " while Processiong Accept().", (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                            }
                            order.CardNumber = paymentStatus.card.bin + "XXXXXX" + paymentStatus.card.last4Digits;
                            order.ExtRef = paymentStatus.id;
                            DataUnitOfWork.Commit();
                            var userCurrency = DataUnitOfWork.Currencies.Get(q => q.Id == order.CurrencyId).FirstOrDefault();
                            var cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCurrency(userCurrency.Code);
                            // var ci = new CultureInfo(cultureInfo); //get culture info
                            receiptModel = new ReceiptModel(cultureInfo, order.Type)
                            {
                                Amount = order.Type == (int)Sell ? order.BTCAmount.GetValueOrDefault() : order.Amount.GetValueOrDefault(),
                                BitcoinAddress = order.CryptoAddress,
                                CardHolderName = order.Name,
                                Commission = ((order.Amount.GetValueOrDefault() - order.FixedFee.GetValueOrDefault()) / 100 * order.CommissionProcent.GetValueOrDefault()),
                                OurFee = (order.Amount.GetValueOrDefault() / 100 * order.OurFee.GetValueOrDefault() * DataUnitOfWork.Orders.GetOrderDiscount(order.Id)),
                                CreditCardNumber = order.CardNumber,
                                Currency = (order.Type == (int)Sell ? DataUnitOfWork.Currencies.GetById(order.CryptoCurrencyId).Code : order.CurrencyCode),
                                OrderNumber = order.Number,
                                TransactionId = order.ExtRef,
                                TransactionHash = order.TransactionHash,
                                Rate = order.Rate.GetValueOrDefault(),
                                CommissionPercent = order.CommissionProcent.ToString(),
                                GoogleTagManagerId = DataUnitOfWork.Sites.GetById(currentSite.Id).GoogleTagManagerId
                            };
                        }
                        else
                        {
                            if (paymentStatus.result.code == "200.300.404")
                                return RedirectToAction("index");
                            else
                            {
                                AuditLog.log("Error in TrustPay PaymentStatus(" + id + ")\r\nPaymentStatus:" + SimpleJson.SerializeObject(paymentStatus), (int)Data.Enums.AuditLogStatus.TrustPay, (int)Data.Enums.AuditTrailLevel.Error);
                                return View("Error", new ErrorModel { ErrorText = "Error in payment process.", ErrorDescription = "Please contact the site administrator." });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AuditLog.log("Error in TrustPay Receipt(" + id + ") " + orderNumber + "\r\n: " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.TrustPay, (int)Data.Enums.AuditTrailLevel.Error);
                        return View("Error", new ErrorModel { ErrorText = "Error in payment process.", ErrorDescription = "Please contact the site administrator." });
                    }
                }
                else
                {
                    operationalStatus = SettingsManager.GetDefault().Get("OperationalStatus", true).Value;
                    if (operationalStatus == null)
                        AuditLog.log("OperationalStatus is not defined in AppSettings in the database.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    else
                    {
                        //if operational status config value is "soldout" then redirect to https://wemovecoins.com/soldout/ 

                        if (operationalStatus.ToLower() == "opensellclosebuy") //OpenSellCloseBuy or soldout
                        {
                            // TODO: redirect to url either based on site get from web.config/configuration
                            if (type == (int)Buy)
                                return Redirect($"https://{ViewBag.fSiteName}/soldout/"); //redirect to we are closed for Buy
                            type = (int)Sell;
                        }
                        if (operationalStatus.ToLower() == "closesellopenbuy")
                        {
                            if (type == (int)Sell)
                                return Redirect($"https://{ViewBag.fSiteName}/soldout/"); //redirect to we are closed for sell
                            type = (int)Buy;
                        }
                        if (operationalStatus.ToLower() == "serviceclose")
                        {
                            if (string.IsNullOrEmpty(dev) && (id == "DefaultTrustPayID"))
                                return Redirect($"https://{ViewBag.fSiteName}/soldout/");
                        }
                        if (type == 0) type = (int)Buy;
                    }

                    reCaptchaStatus = SettingsManager.GetDefault().Get("reCaptchaStatus").Value;
                    RecaptchaAccessSettings reCaptchaSettings = SettingsManager.GetDefault().Get("reCaptchaSettings").GetJsonData<RecaptchaAccessSettings>();
                    reCaptchaPublicKey = reCaptchaSettings.RecaptchaPublicKey;
                    if (reCaptchaStatus == null)
                        AuditLog.log("reCaptchaStatus is not defined in AppSettings in the database.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);

                    Session["ReturnUrl"] = returnUrl ?? "";
                    Session["ReferrerUrl"] = ReferrerUrl ?? "";
                    Session["BitcoinAddress"] = cryptoAddress ?? "";

                    Session["GoogleCaptchaSiteKey"] = reCaptchaSettings.RecaptchaPublicKey;
                }

                //set url to viewbag.tc
                SetTCUrl();

                // TODO: why cant it get from [currentSite] object??
                //get GoogleTagManagerId from Sites table
                ViewBag.GoogleTagManagerId = DataUnitOfWork.Sites.GetById(currentSite.Id).GoogleTagManagerId;

                // TODO: no nakes access of session
                //store Googlerecaptchasitekey in session

                //set IP information to session 
                Session["IPInfoJSONData"] = IPInfoService.GetIPInfo(GetLanIPAddress());

                //get country code from user IP or from query string
                var userCountryCode = "";
                if (!string.IsNullOrEmpty(phoneCode))
                {
                    int userPhoneCode = Convert.ToInt32(phoneCode);
                    userCountryCode = DataUnitOfWork.Countries.Get(x => x.PhoneCode == userPhoneCode).Select(x => x.Code).FirstOrDefault();
                }
                else
                {
                    userCountryCode = GetCountryCodeFromUserIP();
                }

                //get country information using user country code
                var userSettings = DataUnitOfWork.Countries.GetUserSettings(userCountryCode);
                if (currentSite.CurrencyId.HasValue)
                    userSettings.CurrencyCode = DataUnitOfWork.Currencies.GetById(currentSite.CurrencyId.Value).Code;

                //get currency list from Currency table with PaymentTypeAcceptance
                var currenciesWithType = GetCurrenciesWithType(currentSite);

                List<string> currencies = new List<string>(); //all currencies

                foreach (Tuple<string, int> currencyModel in currenciesWithType)
                {
                    currencies.Add(currencyModel.Item1);
                }

                //get digital currency list from Currency table (value 1 for currencyType parameter denotes Fiat, 2 denotes Digital)
                //var digitalCurrencies = currencies.ToList();
                //digitalCurrencies.Add("BTC");
                var digitalCurrencies = DataUnitOfWork.Currencies.Get(curr => curr.CurrencyTypeId == (int)Data.Enums.CurrencyTypes.Digital && curr.IsActive == true).Select(q => q.Code).ToList();

                var forDigitalCurrency = cryptoCurrency ?? digitalCurrencies.FirstOrDefault();

                var forSellCurrency = sellCurrency ?? forDigitalCurrency;

                if (forSellCurrency == null)
                {
                    AuditLog.log("Digital Currency not found!!", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    throw new Exception("Unable to find Digital Currency");
                }
                if (String.IsNullOrEmpty(paymentMethod))
                {
                    paymentMethod = PaymentMethods.CC;
                }
                //get payment method details from app settings
                var currentPaymentMethodDetail = GetPaymentMethodDetail(currentSite, (int)Buy, paymentMethod);
                var currentSellPaymentMethodDetail = GetPaymentMethodDetail(currentSite, (int)Sell, paymentMethod);

                var buyAmount = 0.0M;
                decimal.TryParse(amount, out buyAmount);
                if (buyAmount < 0) buyAmount = 0;

                var paymentMethodData = currentPaymentMethodDetail;

                var paymentMethodObject = currentPaymentMethodDetail.PaymentMethods.FirstOrDefault(q => q.Name == paymentMethod);  //Selecting the CreditCard paymentMethodDetails as default

                for (var i = 0; i < paymentMethodData.PaymentMethods.Count; i++)
                {
                    paymentMethodData.PaymentMethods[i].DisplayName = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", paymentMethodData.PaymentMethods[i].Name);
                }
                var sellPaymentMethodData = currentSellPaymentMethodDetail;

                for (var i = 0; i < sellPaymentMethodData.PaymentMethods.Count; i++)
                {
                    sellPaymentMethodData.PaymentMethods[i].DisplayName = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", sellPaymentMethodData.PaymentMethods[i].Name);
                }
                var sellPaymentMethodObject = currentSellPaymentMethodDetail.PaymentMethods.FirstOrDefault();

                //if currency list not contains currency the assign empty string to currency
                if (!currencies.Contains(currency))
                    currency = "";

                var _bankBuyCurrency = currenciesWithType.Where(x => ((CurrencyAcceptance)x.Item2).HasFlag(CurrencyAcceptance.BankBuy));
                var _bankSellCurrency = currenciesWithType.Where(x => ((CurrencyAcceptance)x.Item2).HasFlag(CurrencyAcceptance.BankSell));
                var _cccurrency = currenciesWithType.Where(x => ((CurrencyAcceptance)x.Item2).HasFlag(CurrencyAcceptance.CreditCard));

                var hasBankBuyCurrency = _bankBuyCurrency.Any(x => x.Item1 == userSettings.CurrencyCode);
                var hasBankSellCurrency = _bankSellCurrency.Any(x => x.Item1 == userSettings.CurrencyCode);
                var hasCCCurrency = _cccurrency.Any(x => x.Item1 == userSettings.CurrencyCode);

                var currencyValue = string.IsNullOrEmpty(currency) ? hasBankBuyCurrency ? userSettings.CurrencyCode : _bankBuyCurrency.First().Item1 : currency;
                var sellCurrencyValue = string.IsNullOrEmpty(sellCurrency) ? hasBankSellCurrency ? userSettings.CurrencyCode : _bankSellCurrency.First().Item1 : sellCurrency;
                var ccCurrencyValue = string.IsNullOrEmpty(sellCurrency) ? hasCCCurrency ? userSettings.CurrencyCode : _cccurrency.First().Item1 : currency;

                //get phone code list from Country table
                var phoneCodes = DataUnitOfWork.Countries.GetPhoneCodes();

                //if phone code list does not contains user entered phone code then assign empty string  
                if (!phoneCodes.Select(q => q.Item3.Replace("+", "")).Contains(phoneCode))
                    phoneCode = "";
                else
                {
                    var phoneCodeData = phoneCodes.Where(q => q.Item3 == "+" + phoneCode);
                    userSettings.PhoneCodeId = phoneCodeData.First().Item1;
                    userSettings.PhoneNumberStyle = phoneCodeData.First().Item4;
                }
                if (!string.IsNullOrEmpty(phoneNumber))
                    phoneNumber = long.Parse(phoneNumber).ToString(userSettings.PhoneNumberStyle.Replace('9', '#'));
                //var minersFeeAppSettingsJSON = appSettings.FirstOrDefault(q => q.ConfigKey == "BitGoMinersFeeSettings").ConfigValue;
                var forDigitalCurrencyModel = DataUnitOfWork.Currencies.Get(curr => curr.Code == forDigitalCurrency).FirstOrDefault();
                var minersFeeAppSettings = JsonConvert.DeserializeObject<BitGoCurrencySettings>(forDigitalCurrencyModel.BitgoSettings);
                // TODO: no naked access of Application/Session
                var euroRates = HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>;
                var euroCurrencyRate = OpenExchangeRates.GetEURExchangeRate(currencyValue, euroRates);

                bool skipOrderForm = false;
                bool showOnlyTopOfOrderForm = false;
                bool showOnlyBottomOfOrderForm = false;

                // TODO: need this documented, very confusing
                if (buyAmount > 0 && (!string.IsNullOrEmpty(cryptoCurrency) && digitalCurrencies.Contains(cryptoCurrency)) && //Validate ordersize range
                    (type == (int)Sell ? (!string.IsNullOrEmpty(sellCurrency) && (digitalCurrencies.Contains(sellCurrency) || currencies.Contains(sellCurrency))) : true) &&
                    (!string.IsNullOrEmpty(currency) && currencies.Contains(currency)) &&
                    (!string.IsNullOrEmpty(paymentMethod) && (type == (int)Sell ? (sellPaymentMethodObject.Name != null) : (paymentMethodObject.Name != null))) &&
                    (ValidateOrderSizeBoundary(currentSite.Id, buyAmount, type, paymentMethod, currency, sellCurrency)) &&
                    (type == (int)Sell ? true : (!string.IsNullOrEmpty(cryptoAddress) && BitGoUtil.ValidateAddress(cryptoAddress, cryptoCurrency))) &&
                    (!string.IsNullOrEmpty(name) && name.Length <= 50 && IsValidateFullName(name)) &&
                    (!string.IsNullOrEmpty(email) && email.Length <= 50 && IsValidEmail(email)) &&
                    (!string.IsNullOrEmpty(phoneCode) && (phoneCode + phoneNumber).Trim().Replace(" ", "").Length <= 20) && //Check phone code is in the list
                    (!string.IsNullOrEmpty(phoneNumber) && (phoneCode + phoneNumber).Trim().Replace(" ", "").Length <= 20))
                    skipOrderForm = true;

                // TODO: need this documented, very confusing
                if ((!string.IsNullOrEmpty(name) && name.Length <= 50 && IsValidateFullName(name)) &&
                    (!string.IsNullOrEmpty(email) && email.Length <= 50 && IsValidEmail(email)) &&
                    (!string.IsNullOrEmpty(phoneCode) && (phoneCode + phoneNumber).Trim().Replace(" ", "").Length <= 20) && //Check phone code is in the list
                    (!string.IsNullOrEmpty(phoneNumber) && (phoneCode + phoneNumber).Trim().Replace(" ", "").Length <= 20) &&
                    (type == (int)Sell ? true : (!string.IsNullOrEmpty(cryptoAddress) && BitGoUtil.ValidateAddress(cryptoAddress, cryptoCurrency))))
                    showOnlyTopOfOrderForm = true;

                // TODO: need this documented, very confusing
                if (buyAmount > 0 && (!string.IsNullOrEmpty(cryptoCurrency) && digitalCurrencies.Contains(cryptoCurrency)) && //Validate ordersize range
                    (type == (int)Sell ? (!string.IsNullOrEmpty(sellCurrency) && (digitalCurrencies.Contains(sellCurrency) || currencies.Contains(sellCurrency))) : true) &&
                    (!string.IsNullOrEmpty(currency) && currencies.Contains(currency)) &&
                    (!string.IsNullOrEmpty(paymentMethod) && (type == (int)Sell ? (currentSellPaymentMethodDetail.BankCommission != null) : (currentPaymentMethodDetail.BankCommission != null))) &&
                    (ValidateOrderSizeBoundary(currentSite.Id, buyAmount, type, paymentMethod, currency, sellCurrency)))
                    showOnlyBottomOfOrderForm = true;

                if (skipOrderForm || (!skipOrderForm && showOnlyBottomOfOrderForm))
                {
                    //get minimum and maximum size boundry for particular currency 
                    var orderSizeBoundary = GetOrderSizeBoundary(currentSite.Id, paymentMethod, type == (int)Buy ? currency : sellCurrency, type); //Note: For Sell, range should always be in BTC
                    if (buyAmount < orderSizeBoundary.Min || buyAmount > orderSizeBoundary.Max)
                    {
                        if (skipOrderForm)
                            skipOrderForm = false;
                        showOnlyBottomOfOrderForm = false;

                    }
                }


                var sendOrderSizeBoundary = GetOrderSizeBoundary(currentSite.Id, type == (int)Buy ? paymentMethodObject.Name : sellPaymentMethodObject.Name, type == (int)Buy ? currencyValue : forSellCurrency, type);
                sendOrderSizeBoundary.Min = Math.Ceiling(sendOrderSizeBoundary.Min);
                sendOrderSizeBoundary.Max = Math.Ceiling(sendOrderSizeBoundary.Max);

                bool _IsExternalReferer = true;
                string referrerUrlUrl = this.ReferrerUrl;
                if (!string.IsNullOrEmpty(referrerUrlUrl))
                {
                    var sname1 = HttpContext.Request.Url.Host.Contains("localhost") ? "localhost" : GetSiteName(HttpContext.Request.Url.Host);

                    var sname2 = referrerUrlUrl.Contains("localhost") ? "localhost" : GetSiteName(new Uri(referrerUrlUrl).Host);
                    _IsExternalReferer = !sname1.Equals(sname2, StringComparison.InvariantCultureIgnoreCase);
                }

                ViewBag.IsExternalReferer = _IsExternalReferer;

                Session["MerchantCode"] = merchantCode;
                Session["ReferenceId"] = referenceId;

                if (_IsExternalReferer && !string.IsNullOrEmpty(returnUrl))
                {
                    var referer_baseUri = new Uri(returnUrl).GetLeftPart(UriPartial.Authority);
                    ViewBag.UrlExternal = referer_baseUri;
                }
                // isDirectindex: not payment redirect - quickpay
                // _IsExternalReferer: not from monni/123bitcoin/hafniatrading
                // returnUrl: no redirecturl
                if (isDirectindex && _IsExternalReferer && string.IsNullOrEmpty(returnUrl))
                {
                    var islocalHost = HttpContext.Request.Url.Host.Contains("localhost");
                    if (islocalHost)
                    {
                        return Redirect($"http://localhost/"); //redirect to we are closed for Buy
                    }
                    else
                    {
                        return Redirect($"https://{ViewBag.fSiteName}/"); //redirect to we are closed for Buy
                    }
                }
                BitGoAccessSettings bitGoAccessSettings = SettingsManager.GetDefault().Get("BitGoAccessCode", true).GetJsonData<BitGoAccessSettings>(); // JsonConvert.DeserializeObject<BitGoAccessSettings>(bitGoAccessCodeJSON);
                bool isBitgoTest = bitGoAccessSettings.Environment.Equals("Test", StringComparison.InvariantCultureIgnoreCase);
                try
                {
                    // TODO: suggest not to have inline operations: LatestBTCRate, decimal.Parse, PhoneCode, OperationalStatus
                    //assign values to OrderModel properties
                    return View(new OrderModel
                    {
                        BuyAmount = buyAmount == 0 ? (decimal?)null : buyAmount,
                        BitcoinAddress = type == (int)Sell ? "" : cryptoAddress,
                        FullName = name,
                        EMail = email,
                        CardFee = userSettings.CardFee,
                        OrderSizeBoundary = sendOrderSizeBoundary,
                        Btc2LocalCurrency = LatestBTCRate(currencyValue, forDigitalCurrency),
                        Btc2LocalCurrencyNumeric = LatestCryptocurrencyRate(currencyValue, (int)Buy, forDigitalCurrency).Item1,
                        Btc2SellCurrencyNumeric = LatestCryptocurrencyRate(sellCurrencyValue, (int)Sell, forDigitalCurrency).Item1,
                        Btc2LocalBuyTicker = LatestCryptocurrencyRate(currencyValue, (int)Buy, forDigitalCurrency).Item2.ToString(),
                        Btc2LocalCurrencyBuyNumeric = LatestBTCBuySellRate(currencyValue, (int)Buy, forDigitalCurrency),
                        Btc2LocalSellTicker = LatestCryptocurrencyRate(currencyValue, (int)Sell, forDigitalCurrency).Item2.ToString(),
                        Btc2LocalCurrencySellNumeric = LatestBTCBuySellRate(currencyValue, (int)Sell, forDigitalCurrency),
                        Reg = Reg,
                        IBAN = IBAN,
                        SwiftCode = SwiftCode,
                        AccountNumber = AccountNumber,
                        EuroBtcRate = decimal.Parse(((Dictionary<string, decimal?>)HttpContext.Application["LatestBTCEURRate"])[forDigitalCurrency].ToString()),
                        EuroCurrencyRate = euroCurrencyRate,
                        CultureCode = userSettings.CultureCode,
                        Currencies = currenciesWithType,
                        DigitalCurrencies = digitalCurrencies,
                        ForCurrency = currencyValue,
                        ForSellCurrency = forSellCurrency,
                        ForBuyCCCurrency = ccCurrencyValue,
                        SellBankCurrency = sellCurrencyValue,
                        ForDigitalCurrency = forDigitalCurrency,
                        PaymentMethodDetail = currentPaymentMethodDetail,
                        SellPaymentMethodDetail = currentSellPaymentMethodDetail,
                        BuyPaymentMethods = paymentMethodObject,
                        SellPaymentMethods = sellPaymentMethodObject,
                        PhoneCodes = phoneCodes,
                        PhoneCode = "+" + (string.IsNullOrEmpty(phoneCode) ? userSettings.PhoneCode.Value.ToString() : phoneCode),
                        PhoneCodeId = userSettings.PhoneCodeId.Value,
                        Mobile = phoneNumber,
                        MobileNumberFormat = userSettings.PhoneNumberStyle,
                        ReturnUrl = returnUrl,
                        MinersFee = minersFeeAppSettings.MinersFee.fee,
                        BccAddress = bcc,
                        PartnerId = partnerId,
                        Compact = compact,
                        SkipOrderForm = skipOrderForm,
                        ShowOnlyTopOfOrderForm = showOnlyTopOfOrderForm,
                        ShowOnlyBottomOfOrderForm = showOnlyBottomOfOrderForm,
                        Type = type,
                        OperationalStatus = operationalStatus.ToLower(),
                        ReCaptchaStatus = reCaptchaStatus,
                        ReCaptchaPublicKey = reCaptchaPublicKey,
                        CouponCode = couponCode,
                        ReceiptModel = receiptModel,
                        IsBitgoTest = isBitgoTest
                    });
                }
                catch (Exception ex)
                {
                    // TODO: what is the use of variable [result] ?? remove it?
                    var result = NullChecker.GetResult(() => { return buyAmount == 0 ? (decimal?)null : buyAmount; },
                        () => { return cryptoAddress; },
                        () => { return name; },
                        () => { return email; },
                        () => { return GetOrderSizeBoundary(currentSite.Id, paymentMethod, currencyValue, type); },
                        () => { return LatestBTCRate(currencyValue, forDigitalCurrency); },
                        () => { return LatestCryptocurrencyRate(currencyValue, (int)Sell, forDigitalCurrency).Item2; },
                        () => { return decimal.Parse(((Dictionary<string, decimal?>)HttpContext.Application["LatestBTCEURRate"])[forDigitalCurrency].ToString()); },
                        () => { return euroCurrencyRate; },
                        () => { return userSettings.CultureCode; },
                        () => { return currencies; },
                        () => { return currencyValue; },
                        () => { return currentPaymentMethodDetail; },
                        () => { return paymentMethodData; },
                        () => { return phoneCodes; },
                        () => { return "+" + (string.IsNullOrEmpty(phoneCode) ? userSettings.PhoneCode.Value.ToString() : phoneCode); },
                        () => { return userSettings.PhoneCodeId.Value; },
                        () => { return phoneNumber; },
                        () => { return userSettings.PhoneNumberStyle; },
                        () => { return returnUrl; },
                        () => { return minersFeeAppSettings.MinersFee; },
                        () => { return bcc; },
                        () => { return partnerId; },
                        () => { return compact; },
                        () => { return skipOrderForm; },
                        () => { return showOnlyTopOfOrderForm; },
                        () => { return showOnlyBottomOfOrderForm; });
                    throw new Exception(string.Format("Error while creating OrderViewModel.", ex.ToMessageAndCompleteStacktrace()));
                }
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;
                AuditLog.log("Error in Index():\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        /// <summary>
        /// method to return OrderInfo partial view
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderInfo()
        {
            var hostname = HttpContext.Request.Url.Host;
            SetSiteNameViewBag(hostname);
            SetTCUrl();
            return PartialView();
        }

        public ActionResult GetMinersFee(string cryptoCurrency)
        {
            // TODO: select specific properties/fields?
            var currency = DataUnitOfWork.Currencies.Get(curr => curr.Code == cryptoCurrency).FirstOrDefault();
            if (currency == null || currency.CurrencyTypeId != (int)Data.Enums.CurrencyTypes.Digital)
            {
                return null;
            }
            else
            {
                var bitgoSettings = JsonConvert.DeserializeObject<BitGoCurrencySettings>(currency.BitgoSettings);
                return new BetterJsonResult { Data = new { minersFee = bitgoSettings.MinersFee.fee } };
            }
        }

        /// <summary>
        /// method for verification of order
        /// </summary>
        /// <param name="product"></param>
        /// <param name="buyAmount">Amount entered by user</param>
        /// <param name="purchaseCurrency">Currency entered by user</param>
        /// <param name="paymentMethod">Payment method selcted by user</param>
        /// <param name="btcAddress">Wallet address</param>
        /// <param name="fullname">full name of user</param>
        /// <param name="email">email address of user</param>
        /// <param name="phoneCode">phone code selected by user</param>
        /// <param name="phoneNumber">user's phone number</param>
        /// <returns></returns>
        [HttpPost]
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult Verification(decimal buyAmount, string purchaseCurrency, string forSellCurrency, string paymentMethod, string btcAddress, string fullname, string email, int phoneCodeId, string phoneNumber, string bccAddress, string partnerId, bool respondWithView, int type, bool recieveNewsletters, string couponCode, string Reg, string IBAN, string SwiftCode, string AccountNumber, string reCaptcha, string cryptoCurrency, string lang)
        {
            try
            {
                //get requst input stream and convert it into JSON, store in RequestInfo
                string requsetObject = JsonConvert.SerializeObject(new { VerificationRequest = GetRequestObjectModel() });
                //get url referrer from request object
                var referrer = HttpContext?.Request?.UrlReferrer?.OriginalString;
                //get app settings from database
                // var appSettings = DataUnitOfWork.AppSettings.GetAll();
                //get details of current site
                var currentSite = GetCurrentSite();
                //get currencies from Currency table
                var currencies = GetCurrencies(null);
                //get digital currency list from Currency table (value 1 for currencyType parameter denotes Fiat, 2 denotes Digital)
                //TODO: select specific??
                var digitalCurrencies = DataUnitOfWork.Currencies.Get(curr => curr.CurrencyTypeId == (int)Data.Enums.CurrencyTypes.Digital && curr.IsActive == true).ToList();
                //get payment method details from app settings
                //TODO: select specific??
                var phoneCountry = DataUnitOfWork.Countries.Get(q => q.Id == phoneCodeId).FirstOrDefault();
                var phoneCode = "+" + phoneCountry.PhoneCode.ToString();
                var transactionLimitsDetails = "";
                var paymentMethodDetailsJSON = "";
                var sellpaymentMethodDetailsJSON = "";
                var CreditCardLimitsDetails = "";
                var currentPaymentMethod = "";
                decimal? fixedFee = 0;
                decimal spread = 0;
                var hostname = HttpContext.Request.Url.Host;
                string str = "";
                SetSiteNameViewBag(hostname);
                SetTCUrl();

                string referenceId = Session["ReferenceId"]?.ToString();
                string merchantCode = Session["MerchantCode"]?.ToString();

                var currenciesWithType = GetCurrenciesWithType(currentSite);
                int paymentTypeId = 0;
                if (paymentMethod != null && paymentMethod.Equals("CreditCard"))
                {
                    paymentTypeId = 4;
                }
                else
                {
                    if (type == 1)
                        paymentTypeId = 1;
                    else
                        paymentTypeId = 2;
                }

                var checkCurrencyExists = currenciesWithType.Where(obj => ((obj.Item2 & paymentTypeId)) > 0).Select(x => x.Item1);
                if (purchaseCurrency != null && !checkCurrencyExists.Contains(purchaseCurrency))
                {
                    AuditLog.log("Error in Verifcations() Currency used is not in active list", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    str = "CurrencyNotFound";
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    result.AddErrorKey(str, lang);
                    return result;
                }

                var reCaptchaStatus = SettingsManager.GetDefault().Get("reCaptchaStatus").Value;
                if (reCaptchaStatus == "Enabled")
                {
                    RecaptchaAccessSettings reCaptchaSettings = SettingsManager.GetDefault().Get("reCaptchaSettings").GetJsonData<RecaptchaAccessSettings>();
                    if (!Validate(reCaptcha, reCaptchaSettings.RecaptchaPrivateKey))
                    {
                        //Todo: Show proper error message to user
                        //throw new Exception("Recaptcha Failure");
                        str = "GoogleRecaptchaExpried";
                        var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        result.AddErrorKey(str, lang);
                        return result;
                    }
                }
                PaymentBoundaryDetails paymentMethodDetail = null;
                try
                {
                    //add phone code
                    phoneNumber = phoneCode + phoneNumber; // remove phone code
                    phoneNumber = phoneNumber.Trim().Replace(" ", ""); //remove spaces in phone number

                    // TODO: select specific fields?
                    var userResult = DataUnitOfWork.Users.Get(q => q.Phone == phoneNumber).Select(q => new { q.SellPaymentMethodDetails, q.PaymentMethodDetails }).FirstOrDefault();

                    if (userResult != null && userResult.SellPaymentMethodDetails != null)
                    {
                        sellpaymentMethodDetailsJSON = userResult.SellPaymentMethodDetails;
                    }
                    else
                    {
                        sellpaymentMethodDetailsJSON = SettingsManager.GetDefault().Get("SellPaymentMethodDetails").Value;
                    }
                    if (userResult != null && userResult.PaymentMethodDetails != null)
                    {
                        paymentMethodDetailsJSON = userResult.PaymentMethodDetails;
                    }
                    else
                    {
                        paymentMethodDetailsJSON = SettingsManager.GetDefault().Get("SitePaymentMethodDetails").Value;
                    }

                    transactionLimitsDetails = SettingsManager.GetDefault().Get("TransactionLimits").Value;

                    CreditCardLimitsDetails = SettingsManager.GetDefault().Get("CreditCardLimits").Value;

                    if (type == (int)Sell)
                    {
                        var sellpaymentMethodDetails = JsonConvert.DeserializeObject<PaymentBoundaryDetails>(sellpaymentMethodDetailsJSON);
                        paymentMethodDetail = sellpaymentMethodDetails;
                    }
                    else
                    {
                        var paymentMethodDetails = JsonConvert.DeserializeObject<PaymentBoundaryDetails>(paymentMethodDetailsJSON);
                        paymentMethodDetail = paymentMethodDetails;
                    }
                    spread = paymentMethodDetail.Spread;

                    if (paymentMethod.Equals("Bank"))
                        currentPaymentMethod = paymentMethod;
                    if (paymentMethod.Equals("Bank") && (type == (int)Sell) && paymentMethodDetail.BankCommission.FixedFee.HasValue)
                        fixedFee = paymentMethodDetail.BankCommission.FixedFee;
                }
                catch (Exception ex)
                {
                    AuditLog.log("Error in Verification(). Error in reading SitePaymentMethodDetails from AppSettings for user phone " + phoneCode + phoneNumber + ": " + ex.ToString(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    throw ex;
                }

                //get minimum and maximum size boundry for particular currency 
                //var orderSizeBoundary = GetOrderSizeBoundary(currentSite.Id, currentPaymentMethod.Name, type == (int)Buy ? purchaseCurrency : cryptoCurrency, type); //Note: For Sell, range should always be in BTC
                var orderSizeBoundary = GetOrdersizeBoundary(type == (int)Buy ? purchaseCurrency : cryptoCurrency, forSellCurrency, paymentMethod, type, currentSite.Id, paymentMethodDetail);

                if (type == (int)Sell && !digitalCurrencies.Any(curr => curr.Code == forSellCurrency) && forSellCurrency != cryptoCurrency)
                    buyAmount = buyAmount / LatestCryptocurrencyRate2(forSellCurrency, purchaseCurrency, (int)Sell, cryptoCurrency).Item1;

                ThresholdSettings thresholdSettings = SettingsManager.GetDefault().Get("ThresholdDetails").GetJsonData<ThresholdSettings>();
                string thresholdConfigOverride = thresholdSettings.BankBuyAmountThreshold;
                var amountThreshold = Convert.ToDecimal(thresholdConfigOverride);

                //Limit the bank buy orders to 999DKK only
                //if (buyAmount >= amountThreshold && paymentMethod == "Bank" && type == (int)Buy)
                //{
                //    AuditLog.log("Order amount allowed is only 999 DKK, user amount is " + buyAmount + "validation failed for user phone " + phoneCode + phoneNumber + ".\r\nverification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
                //   + phoneCode + "," + phoneNumber + "," + bccAddress + ")", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                //    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                //    result.AddErrorKey("OrderAmountOnly999DKK", lang);
                //    return result;
                //}

                if (!digitalCurrencies.Any(curr => curr.Code == cryptoCurrency))
                {
                    AuditLog.log("Product " + cryptoCurrency + " is not valid.\r\nVerification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
                    + phoneCode + "," + phoneNumber + "," + bccAddress + ")", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "CryptoCurrencyNotValid");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }

                //if payment method is neither CreditCard nor Bank
                if (!(paymentMethod == "CreditCard" || paymentMethod == "Bank"))
                {
                    AuditLog.log(string.Format("PaymentMethod '{0}' is incorrect for user phone {1}{2}.\r\nVerification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
                    + phoneCode + "," + phoneNumber + "," + bccAddress + ")", paymentMethod, phoneCode, phoneNumber), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Warn);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "PaymentMethodIncorrect");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }

                //if user entered currency not present in currencies fetched from database
                if (!currencies.Contains(purchaseCurrency))
                {
                    AuditLog.log(string.Format("Currency {0} does not exist for user with phone {1}{2}.\r\nVerification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
                    + phoneCode + "," + phoneNumber + "," + bccAddress + ")", purchaseCurrency, phoneCode, phoneNumber), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "CurrencyNotFound");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }


                //if buy amount is less than or greater than min or max of orderSizeBoundary
                if (buyAmount < orderSizeBoundary.Min || buyAmount > orderSizeBoundary.Max)
                {
                    AuditLog.log("Order amount boundary(Min: " + orderSizeBoundary.Min + ", Max: " + orderSizeBoundary.Max + ") validation failed for user phone " + phoneCode + phoneNumber + ".\r\nVerification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
                    + phoneCode + "," + phoneNumber + "," + bccAddress + ")", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "OrderAmountOutOfRange");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }

                // TODO: select specific fields only??
                //get user currency details from Currency table
                var userCurrency = DataUnitOfWork.Currencies.Get(q => q.Code == purchaseCurrency).FirstOrDefault();
                if (userCurrency == null)
                {
                    AuditLog.log(string.Format("Selected currency " + purchaseCurrency + " is not availabe for user phone {0}{1}.\r\nVerification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
                    + phoneCode + "," + phoneNumber + "," + bccAddress + ")", phoneCode, phoneNumber), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "PurchaseCurrencyEmpty");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }

                decimal fxMarkUp = userCurrency.FXMarkUp.Value;

                if (type == (int)Buy)
                {
                    //if length of user entered btcAddress is null
                    if (string.IsNullOrEmpty(btcAddress))
                    {
                        AuditLog.log("BTCAddress " + btcAddress + " is empty.\r\nVerification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
                        + phoneCode + "," + phoneNumber + "," + bccAddress + ")", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                        var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "BTCAddressEmpty");
                        result.AddErrorKey(errorMesaage, lang);
                        return result;
                    }

                    //if user entered bitcoin address is invalid
                    if (!BitGoUtil.ValidateAddress(btcAddress, cryptoCurrency))
                    {
                        AuditLog.log(string.Format("Bitcoin address validation failed for user phone {0}{1}.\r\nVerification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
                        + phoneCode + "," + phoneNumber + "," + bccAddress + ")", phoneCode, phoneNumber), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                        var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        //result.AddError("BitcoinAddressInvalid");
                        var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "BitcoinAddressInvalid");
                        result.AddErrorKey(errorMesaage, lang);
                        return result;
                    }
                }

                //if user full name is empty
                if (string.IsNullOrEmpty(fullname))
                {
                    AuditLog.log(string.Format("Fullname " + fullname + " is null for user phone {0}{1}.\r\nVerification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
                    + phoneCode + "," + phoneNumber + "," + bccAddress + ")", phoneCode, phoneNumber), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "FullNameEmptyMessage");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }

                //if length of user full name is greater than 50
                if (fullname.Length > 50)
                {
                    AuditLog.log(string.Format("Fullname " + fullname + " is exceeded the limit for user phone {0}{1}.\r\nVerification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
                    + phoneCode + "," + phoneNumber + "," + bccAddress + ")", phoneCode, phoneNumber), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "FirstNameSurNameTooLongMessage");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }

                //if user email is empty
                if (string.IsNullOrEmpty(email))
                {
                    AuditLog.log(string.Format("Email " + email + " is null for user phone {0}{1}.\r\nVerification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
                    + phoneCode + "," + phoneNumber + "," + bccAddress + ")", phoneCode, phoneNumber), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "EmailEmptyMessage");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }

                //if length of user entered email is greater than 50
                if (email.Length > 50)
                {
                    AuditLog.log(string.Format("Email " + email + " is exceeded the limit for user phone {0}{1}.\r\nVerification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
                    + phoneCode + "," + phoneNumber + "," + bccAddress + ")", phoneCode, phoneNumber), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "EmailTooLongMessage");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }
                var phoneNumberCheck = phoneNumber.Remove(0, 3);
                //if length of user entered phone number is null
                if (string.IsNullOrEmpty(phoneNumberCheck))
                {
                    AuditLog.log("Phone number " + phoneNumber + " is empty.\r\nVerification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
                    + phoneCode + "," + phoneNumber + "," + bccAddress + ")", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "MobileEmptyMessage");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }

                if (phoneNumber.Length < 7 && phoneNumber.Length > 16)
                {
                    AuditLog.log("Phone number " + phoneNumber + " is invalid.\r\nVerification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
+ phoneCode + "," + phoneNumber + "," + bccAddress + ")", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "MobileInvalidMessage");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;

                }

                //get user country details based on phone code selected by user
                var userCountry = DataUnitOfWork.Countries.Get(q => q.PhoneCode.ToString() == phoneCode.Replace("+", "")).FirstOrDefault();
                if (userCountry == null)
                {
                    AuditLog.log("PhoneCode " + phoneCode + " is not valid or does not exist.\r\nVerification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
                    + phoneCode + "," + phoneNumber + "," + bccAddress + ")", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "PhoneCodeError");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }

                //get user details from User table based on phone number entered by user
                var user = GetUser(fullname, email, phoneNumber, paymentMethodDetailsJSON, sellpaymentMethodDetailsJSON, transactionLimitsDetails, CreditCardLimitsDetails, userCountry, recieveNewsletters);

                if (user.Blocked == true) throw new Exception("The user is blocked"); //TODO:Show proper error message
                var buyAmountCheck = buyAmount;
                if (type == (int)Sell)
                    buyAmountCheck = buyAmount * LatestCryptocurrencyRate(purchaseCurrency, (int)Sell, cryptoCurrency).Item1;
                var eurRate = OpenExchangeRates.GetEURExchangeRate(purchaseCurrency, HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>);
                //get KYC requirement
                string kycRequirement = GetKYCRequirement(user, buyAmountCheck, purchaseCurrency, eurRate, true, paymentMethod);

                //get comission for order
                var overwriteCardFee = SettingsManager.GetDefault().Get("OverwriteCardFee").Value;
                //var commission = GetCommissionPercent(paymentMethod, currentSite, paymentMethodDetailsJSON, overwriteCardFee.ConfigValue, type);
                var commission = 0.0M;
                if (paymentMethod == "CreditCard")
                    commission = DataUnitOfWork.Countries.Get(q => q.Id == phoneCodeId).Select(q => q.CardFee).FirstOrDefault().Value;
                var ourFee = 0.0M;
                if (type == (int)Sell)
                {
                    ourFee = GetOurFeePercent(paymentMethod, currentSite, sellpaymentMethodDetailsJSON, overwriteCardFee, type).Value;
                }
                else
                {
                    ourFee = GetOurFeePercent(paymentMethod, currentSite, paymentMethodDetailsJSON, overwriteCardFee, type).Value;
                }

                //get currency details
                var currency = DataUnitOfWork.Currencies.GetById(userCurrency.Id);

                //calculate payment gateway type
                var paymentGatewayType = SettingsManager.GetDefault().Get("CreditCardGatewayName").Value;
                paymentGatewayType = CalculatePaymentGatewayType(currentSite.Id, paymentGatewayType, currency.Code, user.Trusted.HasValue);

                var countryCode = "";
                //get country code
                if (HttpContext.Request.UserLanguages != null)
                    countryCode = string.Join(",", HttpContext.Request.UserLanguages);

                //get unique order number for orders
                var orderNumber = getUniqueOrderNumber();

                //get IP info
                if (Session["IPInfoJSONData"] as string == null)
                    Session["IPInfoJSONData"] = IPInfoService.GetIPInfo(GetLanIPAddress());
                var ipinfo = Session["IPInfoJSONData"] as string;
                AuditLog.log("IPInfoJSONData:" + Session["IPInfoJSONData"], (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Info);
                if (ipinfo != null)
                    ipinfo = ipinfo.Replace("\"", "");
                else
                    ipinfo = "?";

                //get latestBTCRate
                var latestBTCRate = LatestCryptocurrencyRate(currency.Code, type, cryptoCurrency).Item1;
                var cryptoCurrencyModel = digitalCurrencies.Where(cur => cur.Code == cryptoCurrency).FirstOrDefault();

                //calculate miners fee
                var minersFeeAppSettings = JsonConvert.DeserializeObject<BitGoCurrencySettings>(cryptoCurrencyModel.BitgoSettings);
                decimal discountAmount = 0;
                // TODO: constant? create a constant for "CreditCard" and "Bank"
                if (couponCode != null && paymentMethod == "CreditCard")
                    discountAmount = buyAmount * ((commission / 100) * (DataUnitOfWork.Coupons.Get(q => q.CouponCode == couponCode).FirstOrDefault().Discount / 100));

                if (couponCode != null && paymentMethod == "Bank")
                    discountAmount = buyAmount * ((ourFee / 100) * (DataUnitOfWork.Coupons.Get(q => q.CouponCode == couponCode).FirstOrDefault().Discount / 100));

                if (user.Trusted.HasValue != true)
                    str = CheckNewUserLimits(buyAmountCheck, userCurrency.Id, user.Id);

                if (user.Trusted.HasValue != true && paymentMethod == "CreditCard")
                    str = CheckNewUserCrdeitCardLimits(buyAmountCheck, userCurrency.Id, user.Id);

                if (str != "")
                {
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    result.AddErrorKey(str, lang);
                    return result;

                }

                //update user Tier level
                UpdateUserTier(user, buyAmountCheck, eurRate);
                //Create a new Order
                var orderId = CreateNewOrder(buyAmount, paymentMethod, btcAddress, fullname, email, bccAddress,
                    partnerId, requsetObject, referrer, currentSite.Id, userCurrency.Id, user.Id, commission, ourFee,
                    paymentGatewayType, countryCode, orderNumber, ipinfo, latestBTCRate, minersFeeAppSettings, type, couponCode, fixedFee, discountAmount, Reg, IBAN, SwiftCode, AccountNumber, fxMarkUp, spread, cryptoCurrencyModel.Id, referenceId, merchantCode);

                //Create user session model
                CreateUserSession(buyAmount, paymentMethod, btcAddress, fullname, email, phoneNumber, userCurrency, kycRequirement, commission, ourFee, latestBTCRate, orderId, type, couponCode, fixedFee, cryptoCurrencyModel);

                if (respondWithView)
                {
                    FaceTecAppSettings facetecSettings = SettingsManager.GetDefault().Get("FaceTecKeys").GetJsonData<FaceTecAppSettings>();
                    ViewBag.FaceTecKYC = facetecSettings.KYC;
                    ViewBag.FaceTecProdKey = JsonConvert.SerializeObject(facetecSettings.FaceTecProdKey);
                    ViewBag.FaceTecDeviceKey = facetecSettings.FaceTecDeviceKey;
                    return PartialView(new VerificationModel { KycRequirement = kycRequirement });
                }
                else
                    return new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new VerificationModel { KycRequirement = kycRequirement } };
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in Verification(" + cryptoCurrency + "," + buyAmount + "," + purchaseCurrency + "," + paymentMethod + "," + btcAddress + "," + fullname + "," + email + ","
                    + phoneCodeId + "," + phoneNumber + "," + bccAddress + "):\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                //result.AddError("An error has occured. Please contact the site administrator.");
                result.AddErrorKey("UnableToVerifyOrder", lang);
                return result;
            }
        }

        [HttpPost]
        [AuthorizeUser]
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult BitcoinAddressConfirmationView(string lang)
        {
            try
            {
                var userSessionModel = ViewBag.UserSessionModel as UserSessionModel;
                var user = DataUnitOfWork.Users.Get(q => q.Phone == userSessionModel.PhoneNumber).FirstOrDefault();
                user.PhoneVerificationCode = null;
                DataUnitOfWork.Commit();
                AuditLog.log("Updated phone verification code is null in BitcoinAddressConfirmationView(), for user phone number " + user.Phone, (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info, userSessionModel.OrderId);
                return new BetterJsonResult { Data = new { } };
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in BitcoinAddressConfirmationView():\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        /// <summary>
        /// method to send verification code to user's mobile number
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeUser]
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult SendVerificationCode(string lang)
        {
            // TODO: SHI, send and resend should have some commonality, and shouldnot repeat samething??
            try
            {
                var currentSite = GetCurrentSite();
                TwilioService twilio = (TwilioService)TwilioService.GetDefault(currentSite.Id);
                if (!twilio.IsConfigured())
                {
                    throw new Exception("Unable to find 'TwilioTestOrProd' key in AppSettings.");
                }

                var userSessionModel = ViewBag.UserSessionModel as UserSessionModel;
                var phoneVerificationAttempts = SettingsManager.GetDefault().Get("PhoneVerificationAttempts").Value;
                if (phoneVerificationAttempts == null)
                    AuditLog.log("Unable to find 'PhoneVerificationAttempts' key in AppSettings.", (int)Data.Enums.AuditLogStatus.Twilio, (int)Data.Enums.AuditTrailLevel.Error);
                var phoneVerificationAttemptsValue = 0;
                if (int.TryParse(phoneVerificationAttempts, out phoneVerificationAttemptsValue))
                {
                    if (phoneVerificationAttemptsValue > 0 && userSessionModel.PhoneVerificationCodeCounter >= phoneVerificationAttemptsValue)
                    {
                        var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        result.StatusCode = 429;
                        var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "VerificationCodeRequestDenied");
                        result.AddErrorKey(errorMesaage, lang);
                        return result;
                    }
                }

                //get user details from User table
                var user = DataUnitOfWork.Users.Get(q => q.Phone == userSessionModel.PhoneNumber).FirstOrDefault();
                //generate new random verification code
                // TODO: DEFAULT value fpr test be from config?
                var verificationCode = twilio.IsTest() ? 654321 : new Random().Next(100000, 999999);
                var trn_amount = string.Format("{0:0.########}", userSessionModel.OrderAmount);
                var trn_currency = DataUnitOfWork.Currencies.GetAll().Where(x => x.Id == (userSessionModel.Type == (int)Sell ? userSessionModel.CryptoCurrencyId : userSessionModel.CurrencyId)).Select(x => x.Code).FirstOrDefault();  //> 0 ? userSessionModel.CurrencyId : userSessionModel.CryptoCurrencyId;
                                                                                                                                                                                                                                        //generate message which is to be sent to user
                var message = WMC.Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", userSessionModel.Type == (int)Sell ? "SellPhoneNumberVerificationMessage" : "PhoneNumberVerificationMessage");
                //sending verification code user's phone number
                twilio.SendVerificationCode(userSessionModel.PhoneNumber, userSessionModel.FullName, verificationCode, message, userSessionModel.OrderId, trn_amount, trn_currency);
                userSessionModel.PhoneVerificationCodeCounter += 1;
                user.PhoneVerificationCode = verificationCode;
                DataUnitOfWork.Commit();
                AuditLog.log("Updated user PhoneVerificationCode to " + verificationCode + " in SendVerificationCode(), for user Phone number " + user.Phone, (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info, userSessionModel.OrderId);
                return new BetterJsonResult { };
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in SendVerificationCode()\r\n: " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        /// <summary>
        /// method to resend code for phone verification
        /// </summary>
        /// <param name="userIdentity"></param>
        /// <returns></returns>
        public ActionResult ResendVerificationCode(string userIdentity)
        {
            // TODO: code repeatation - move common to another method
            try
            {
                //var user = DataUnitOfWork.Users.Get(q => q.Phone == userSessionModel.PhoneNumber).FirstOrDefault();
                var currentSite = GetCurrentSite();
                TwilioService twilio = (TwilioService)TwilioService.GetDefault(currentSite.Id);
                if (!twilio.IsConfigured())
                {
                    throw new Exception("Unable to find 'TwilioTestOrProd' key in AppSettings.");
                }

                //get user id from order table
                var userSessionModel = ViewBag.UserSessionModel as UserSessionModel;
                var order = DataUnitOfWork.Orders.Get(q => q.CreditCardUserIdentity.ToString() == userIdentity).FirstOrDefault();
                if (order == null)
                    throw new Exception("Unable to find order with useridentity " + userIdentity + ".");
                //get user details
                var user = DataUnitOfWork.Users.Get(q => q.Id == order.UserId).FirstOrDefault();
                var verificationCode = twilio.IsTest() ? 654321 : new Random().Next(100000, 999999);
                var trn_amount = string.Format("{0:0.########}", userSessionModel.OrderAmount);
                var trn_currency = DataUnitOfWork.Currencies.GetAll().Where(x => x.Id == userSessionModel.CurrencyId).Select(x => x.Code).FirstOrDefault();  //> 0 ? userSessionModel.CurrencyId : userSessionModel.CryptoCurrencyId;
                var message = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", userSessionModel.Type == (int)Sell ? "SellPhoneNumberVerificationMessage" : "PhoneNumberVerificationMessage");
                //send verification code using Twilio Service
                twilio.SendVerificationCode(user.Phone, user.Fname, verificationCode, message, userSessionModel.OrderId, trn_amount, trn_currency);
                user.PhoneVerificationCode = verificationCode;
                DataUnitOfWork.Commit();
                AuditLog.log("Updated the Phone verifaction Code " + verificationCode + " for User " + user.Id, (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info);
                return new BetterJsonResult { Data = new object { } };
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;
                AuditLog.log("Error in ResendVerificationCode(" + userIdentity + "):\r\n" + ex.ToMessageAndCompleteStacktrace(),
                   (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "VerificationCodeError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        /// <summary>
        /// method to verify phone number provide by user
        /// </summary>
        /// <param name="verificationCode">code sent to user provided phone number</param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeUser]
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult VerifyPhoneNumber(int? verificationCode)
        {
            var lang = CultureInfo.CurrentCulture.Name;

            try
            {
                var userSessionModel = ViewBag.UserSessionModel as UserSessionModel;
                //get user details
                var user = DataUnitOfWork.Users.Get(q => q.Phone == userSessionModel.PhoneNumber).FirstOrDefault();

                // TODO: null check first
                //Check user entered verification code is correct
                if (user.PhoneVerificationCode == verificationCode && user.PhoneVerificationCode != null && verificationCode != null)
                {
                    //if entered code is correct, assign null to phone verification code 
                    //  user.PhoneVerificationCode = null;
                    // TODO: why commit required here? nothing changed??
                    DataUnitOfWork.Commit();
                    userSessionModel.PhoneNumberVerified = true;
                    return new BetterJsonResult { Data = new { userSessionModel.KycRequirement } };
                }
                else
                {
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    AuditLog.log("Verification code " + verificationCode + " is incorrect for user " + user.Id + ". The correct code is " + user.PhoneVerificationCode + ".", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error, userSessionModel.OrderId);
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "VerificationCodeIncorrect");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in VerifyPhoneNumber(" + verificationCode + "):\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        /// <summary>
        /// method to upload files 
        /// </summary>
        /// <param name="kycFiles"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeUser]
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult KYCFileUpload([ModelBinder(typeof(KYCFileInfoBinder))] KYCFileInfo kycFiles)
        {
            // TODO: remove commented+unwanted code
            try
            {
                if (kycFiles == null)
                {
                    AuditLog.log("KYC files are null in KYCFileUpload().", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    result.AddError("KYC files are null");
                    return result;
                }
                if (!String.IsNullOrEmpty(kycFiles.Error))
                {
                    AuditLog.log(kycFiles.Error, (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    result.AddError(kycFiles.Error);
                    return result;
                }
                var userSessionModel = ViewBag.UserSessionModel as UserSessionModel;
                List<KycFile> files = new List<KycFile>();
                foreach (var kycFile in kycFiles.Files)
                {
                    Stream inputStream = kycFile.InputStream;
                    try
                    {
                        if (string.Equals(kycFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) || string.Equals(kycFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) || string.Equals(kycFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
                        {
                            var bitmap = new System.Drawing.Bitmap(inputStream);
                            inputStream.Position = 0;
                        }
                        KycFile kycfile = KYCFileHandler.AddNewKYC(inputStream, userSessionModel.PhoneNumber, kycFile.FileName, kycFiles.Type);
                        files.Add(kycfile);
                    }
                    catch (Exception ex)
                    {
                        var lang = CultureInfo.CurrentCulture.Name;
                        AuditLog.log("Error for Corrupted KYC file upload" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                        var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ErrorMessage");
                        var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        result.AddErrorKey(errorMesaage, lang);
                        return result;
                    }
                }
                AuditLog.log("Uploaded Kyc file for user Phone number " + userSessionModel.PhoneNumber + " in KYCFileUpload().", (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info, userSessionModel.OrderId);
                // TODO: what are we committing here, any changed done in this context?
                DataUnitOfWork.Commit();
                return new BetterJsonResult { Data = files.Select(q => new { q.Id, q.OriginalFilename }).ToArray() };
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;
                AuditLog.log("Error in KYCFileUpload():\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "FileUploadError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        [HttpPost]
        [AuthorizeUser]
        public ActionResult FaceTecKYCFileUpload(FaceTecSession faceTecSession)
        {
            try
            {
                var mongoDBUnitOfWork = KycDataProvider.Instance;
                //Data from Facetec MongoDB
                var facetecLivenessSession = mongoDBUnitOfWork.GetFaceTecDocWithSessionId(faceTecSession.zoomSessionResult.SessionId);
                var faceTecScanIDSession = mongoDBUnitOfWork.GetFaceTecScanDocWithSessionId(faceTecSession.zoomIDScanResult.SessionId);

                var faceTecSessionDBLivenessLite = facetecLivenessSession.Clone();
                var faceTecSessionDBScanLite = faceTecScanIDSession.Clone();

                var userSessionModel = ViewBag.UserSessionModel as UserSessionModel;

                AuditLog.log("faceTecSessionDBLivenessLite " + JsonConvert.SerializeObject(faceTecSessionDBLivenessLite) + "faceTecSessionDBScanLite " + JsonConvert.SerializeObject(faceTecSessionDBScanLite), (int)Data.Enums.AuditLogStatus.FaceTec, (int)Data.Enums.AuditTrailLevel.Info, userSessionModel.OrderId);

                if (string.IsNullOrEmpty(faceTecScanIDSession?.Data?.IdScanFrontImage)
                    || string.IsNullOrEmpty(facetecLivenessSession?.Data?.AuditTrailImage))
                {
                    AuditLog.log("KYC files are null in FaceTecKYCFileUpload().", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    result.AddError("KYC files are null");
                    return result;
                }

                List<KycFile> files = new List<KycFile>();

                var faceTecScanIDType = faceTecSession.zoomIDScanResult.IdType;

                string faceTecKycFileType = KYCFileTypes.PhotoID.ToString();
                List<FaceTecKYC> kycFiles = new List<FaceTecKYC>
                {
                    new FaceTecKYC{ base64Value = facetecLivenessSession.Data.AuditTrailImage, sessionId = faceTecSession.zoomSessionResult.SessionId, fileType = KYCFileTypes.SelfieID.ToString(), fileName = FacetecImageNames.Selfie },
                    new FaceTecKYC{ base64Value = faceTecScanIDSession.Data.IdScanFrontImage, sessionId = faceTecSession.zoomIDScanResult.SessionId, fileType = faceTecKycFileType, fileName = FacetecImageNames.Front },
                 };
                //consider back side image only if facetec scanIdSession id type is IdCard(not a passport)
                if (faceTecScanIDType == FaceTecIDType.IDCard && !string.IsNullOrEmpty(faceTecScanIDSession?.Data?.IdScanBackImage))
                {
                    kycFiles.Add(
                    new FaceTecKYC { base64Value = faceTecScanIDSession.Data?.IdScanBackImage, sessionId = faceTecSession.zoomIDScanResult.SessionId, fileType = faceTecKycFileType, fileName = FacetecImageNames.Back });
                }
                foreach (var kycFile in kycFiles)
                {
                    string strImg = kycFile.base64Value;
                    byte[] dataBuffer = Convert.FromBase64String(strImg);
                    Stream inputStream = new MemoryStream(dataBuffer);
                    try
                    {
                        string contentType = CheckBase64Type(kycFile.base64Value);
                        if (string.Equals(contentType, "image/jpg", StringComparison.OrdinalIgnoreCase) || string.Equals(contentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) || string.Equals(contentType, "image/png", StringComparison.OrdinalIgnoreCase))
                        {
                            var bitmap = new System.Drawing.Bitmap(inputStream);
                            inputStream.Position = 0;
                        }
                        KycFile kycfile = KYCFileHandler.AddNewKYC(inputStream, userSessionModel.PhoneNumber, kycFile.fileName, kycFile.fileType, kycFile.sessionId, (int)faceTecSession.zoomSessionResult.Status);
                        files.Add(kycfile);
                    }
                    catch (Exception ex)
                    {
                        var lang = CultureInfo.CurrentCulture.Name;
                        AuditLog.log("Error for Corrupted KYC file upload" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                        var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ErrorMessage");
                        var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        result.AddErrorKey(errorMesaage, lang);
                        return result;
                    }
                }

                AuditLog.log("Uploaded Kyc file for user Phone number " + userSessionModel.PhoneNumber + " in KYCFileUpload().", (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info, userSessionModel.OrderId);
                // TODO: what are we committing here, any changed done in this context?
                DataUnitOfWork.Commit();
                return new BetterJsonResult { Data = files.Select(q => new { q.Id, q.OriginalFilename }).ToArray() };
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;
                AuditLog.log("Error in FaceTec():\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "FileUploadError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        public class FaceTecKYC
        {
            public string base64Value { get; set; }
            public string sessionId { get; set; }
            public string fileType { get; set; }
            public string fileName { get; set; }
        }

        public string CheckBase64Type(string base64String)
        {
            string[] strings = base64String.Split(',');
            string extension;
            switch (strings[0])
            {//check image's extension
                case "data:image/jpeg;base64":
                    extension = "image/jpeg";
                    break;
                case "data:image/png;base64":
                    extension = "image/png";
                    break;
                default://should write cases for more images types
                    extension = "image/jpg";
                    break;
            }
            return extension;
        }

        public void Base64toFile(string base64String, string dstFilePath)
        {

            //***Save Base64 Encoded string as Image File***//

            byte[] dataBuffer = Convert.FromBase64String(base64String);
            using (FileStream fileStream = new FileStream(dstFilePath, FileMode.Create, FileAccess.Write))
            {
                if (dataBuffer.Length > 0)
                {
                    fileStream.Write(dataBuffer, 0, dataBuffer.Length);
                }
            }

        }

        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public void SendCreditCardDocumentPushOver(string userIdentity)
        {
            try
            {
                var order = DataUnitOfWork.Orders.Get(q => q.CreditCardUserIdentity.ToString() == userIdentity).FirstOrDefault();
                if (order == null)
                    throw new Exception("Unable to find order with CreditCardUserIdentity " + userIdentity);
                var hostname = Request.Url.Host;
                var currentSite = DataUnitOfWork.Sites.Get(q => q.Url == hostname).FirstOrDefault();
                var testOrProd = "";
                //Appending "TEST:" to Pushover if payment type is Credit card
                if (order.PaymentType == 1)
                    if (!currentSite.Text.Contains("localhost"))
                        if (currentSite.Text.Split('.')[0].ToLower() == "apptest")
                            testOrProd = "TEST: ";
                PushoverHelper.SendNotification(testOrProd + (currentSite.Text.Contains("localhost") ? currentSite.Text : currentSite.Text.Split('.')[1]), order.Number + " CC DOC AWAITS APPROVAL :" + order.Name);
                order.Status = (int)Data.Enums.OrderStatus.ComplianceOfficerApproval;
                DataUnitOfWork.Commit();
                AuditLog.log("Updated Order(" + order.Id + ") status to " + Enum.GetName(typeof(Data.Enums.OrderStatus), order.Status) + " in SendCreditCardDocumentPushOver().", (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;
                AuditLog.log("Error in SendCreditCardDocumentPushOver():\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
            }
        }

        /// <summary>
        /// method to delete uploaded KYC files
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeUser]
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult DeleteKYCFile(int? id)
        {
            try
            {
                var kycFile = DataUnitOfWork.KycFiles.Get(q => q.Id == id).FirstOrDefault();
                if (kycFile != null)
                {
                    DataUnitOfWork.KycFiles.Delete(kycFile);
                    // TODO: save changes??
                    AuditLog.log("KYC File Removed id is " + id, (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info);
                }
                return new BetterJsonResult { Data = new { } };
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;
                AuditLog.log("Error in DeleteKYCFile(" + id + "):\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        // TODO: same as DeleteKYCFile??
        /// <summary>
        /// method to delete uploaded KYC files while credit card verification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteCreditCardKYCFile(int id)
        {
            try
            {
                var kycFile = DataUnitOfWork.KycFiles.Get(q => q.Id == id && q.Type == 3).FirstOrDefault();
                if (kycFile != null)
                {
                    DataUnitOfWork.KycFiles.Delete(kycFile);
                    // TODO: save changes??
                    AuditLog.log("Credit card KYC File Removed id is " + id, (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info);
                }
                return new BetterJsonResult { Data = new { } };
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;
                AuditLog.log("Error in DeleteCreditCardKYCFile(" + id + "):\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        // TODO: lot of reduntent/repeatation of code - on many RATE methods
        /// <summary>
        /// method to get latest BTC rate for currency
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        [AllowCrossSite]
        [EnableThrottling(PerSecond = 20, PerMinute = 50)]
        public string LatestBTCRate(string currency = null, string cryptoCurrency = "BTC")
        {
            var countryCode = "";
            try
            {
                var cultureInfo = CultureInfo.CurrentCulture.Name;
                if (string.IsNullOrEmpty(currency))
                {
                    //countryCode = GetCountryCodeFromUserIP();
                    countryCode = ResolveCountry().Name;
                    currency = DataUnitOfWork.Countries.GetCurrencyCodeByCountryCode(countryCode);
                    cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCountryCode(countryCode);
                }
                else
                    cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCurrency(currency);
                //get BTC exchange rate
                return OpenExchangeRates.GetBTCExchangeRate(currency, HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>, decimal.Parse(((Dictionary<string, decimal?>)HttpContext.Application["LatestBTCEURRate"])[cryptoCurrency].ToString()), cryptoCurrency).ToString("N2", new CultureInfo(cultureInfo)) + " " + currency.ToUpper() + "/" + cryptoCurrency;
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in LatestBTCRate():\r\n currency:'" + currency + "'\r\n countryCode: '" + countryCode + "'" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var response = ControllerContext.HttpContext.Response;
                response.StatusCode = 400;
                response.Write("An error has occured. Please contact the site administrator.");
                return "";
            }
        }

        // TODO: lot of reduntent/repeatation of code - on many RATE methods
        [AllowCrossSite]
        [EnableThrottling(PerSecond = 20, PerMinute = 50)]
        public string LatestBuyBTCRate(string currency = null, string cryptoCurrency = "BTC")
        {
            var countryCode = "";
            try
            {
                // var appSettings = DataUnitOfWork.AppSettings.GetAll();
                var paymentMethodDetails = SettingsManager.GetDefault().Get("SitePaymentMethodDetails").GetJsonData<PaymentBoundaryDetails>();
                decimal spread = paymentMethodDetails.Spread;

                var cultureInfo = CultureInfo.CurrentCulture.Name;
                if (string.IsNullOrEmpty(currency))
                {
                    countryCode = ResolveCountry().Name;
                    currency = DataUnitOfWork.Countries.GetCurrencyCodeByCountryCode(countryCode);
                    cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCountryCode(countryCode);
                }
                else
                    cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCurrency(currency);

                return OpenExchangeRates.GetBTCExchangeRate(currency, HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>, (1 + spread / 100) * decimal.Parse(((Dictionary<string, decimal?>)HttpContext.Application["LatestBTCEURRate"])[cryptoCurrency].ToString()), cryptoCurrency).ToString("N2", new CultureInfo(cultureInfo)) + " " + currency.ToUpper() + "/" + cryptoCurrency;
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in LatestBTCRate():\r\n currency:'" + currency + "'\r\n countryCode: '" + countryCode + "'" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var response = ControllerContext.HttpContext.Response;
                response.StatusCode = 400;
                response.Write("An error has occured. Please contact the site administrator.");
                return "";
            }
        }

        // TODO: lot of reduntent/repeatation of code - on many RATE methods
        [AllowCrossSite]
        [EnableThrottling(PerSecond = 20, PerMinute = 50)]
        public string LatestSellBTCRate(string currency = null, string cryptoCurrency = "BTC")
        {
            var countryCode = "";
            try
            {
                var appSettings = DataUnitOfWork.AppSettings.GetAll();
                var paymentMethodDetails = SettingsManager.GetDefault().Get("SellPaymentMethodDetails").GetJsonData<PaymentBoundaryDetails>();
                decimal spread = paymentMethodDetails.Spread;

                var cultureInfo = CultureInfo.CurrentCulture.Name;
                if (string.IsNullOrEmpty(currency))
                {
                    //countryCode = GetCountryCodeFromUserIP();
                    countryCode = ResolveCountry().Name;
                    currency = DataUnitOfWork.Countries.GetCurrencyCodeByCountryCode(countryCode);
                    cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCountryCode(countryCode);
                }
                else
                    cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCurrency(currency);

                return OpenExchangeRates.GetBTCExchangeRate(currency, HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>, (1 - spread / 100) * decimal.Parse(((Dictionary<string, decimal?>)HttpContext.Application["LatestBTCEURRate"])[cryptoCurrency].ToString()), cryptoCurrency).ToString("N2", new CultureInfo(cultureInfo)) + " " + currency.ToUpper() + "/" + cryptoCurrency;
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in LatestBTCRate():\r\n currency:'" + currency + "'\r\n countryCode: '" + countryCode + "'" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var response = ControllerContext.HttpContext.Response;
                response.StatusCode = 400;
                response.Write("An error has occured. Please contact the site administrator.");
                return "";
            }
        }

        // TODO: lot of reduntent/repeatation of code - on many RATE methods
        /// <summary>
        /// method to get latest BTC rate for currency
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        [EnableThrottling(PerSecond = 20, PerMinute = 50)]
        public JsonResult LatestBTCRate2(string currency = null, string cryptoCurrency = "BTC")
        {
            try
            {
                var cultureInfo = CultureInfo.CurrentCulture.Name;
                //if currency is empty or null get culture info from User IP
                if (string.IsNullOrEmpty(currency))
                {
                    var countryCode = GetCountryCodeFromUserIP();
                    currency = DataUnitOfWork.Countries.GetCurrencyCodeByCountryCode(countryCode);
                    cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCountryCode(countryCode);
                }
                else
                    cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCurrency(currency);
                //get BTC exchange rate
                var rate = OpenExchangeRates.GetBTCExchangeRate(currency, HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>, decimal.Parse(((Dictionary<string, decimal?>)HttpContext.Application["LatestBTCEURRate"])[cryptoCurrency].ToString()), cryptoCurrency);
                var kraken_Rate = ((Dictionary<string, decimal?>)HttpContext.Application["LatestBTCEURRate"])[cryptoCurrency];
                var eur_rate = (HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>)[currency];
                var c_rate = kraken_Rate.Value * eur_rate;
                return new BetterJsonResult { Data = new { Rate = c_rate, FormatedRate = c_rate.ToString("N2", new CultureInfo(cultureInfo)) + " " + currency.ToUpper() + "/" + cryptoCurrency }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                var nullCheckerResult = "";
                nullCheckerResult = NullChecker.GetResult(
                () => { return HttpContext.Application["LatestExchangeRates"]; },
                () => { return HttpContext.Application["LatestBTCEURRate"]; }
                );
                var lang = CultureInfo.CurrentCulture.Name;
                AuditLog.log("Error in LatestBTCRate2(" + currency + "):\r\n" + ex.ToMessageAndCompleteStacktrace() + " NullCheckerResult:" + nullCheckerResult, (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        // TODO: lot of reduntent/repeatation of code - on many RATE methods
        public decimal LatestBTCBuySellRate(string currency = null, int type = 0, string cryptoCurrency = "BTC")
        {
            try
            {
                var cultureInfo = CultureInfo.CurrentCulture.Name;
                //if currency is empty or null get culture info from User IP
                if (string.IsNullOrEmpty(currency))
                {
                    var countryCode = GetCountryCodeFromUserIP();
                    currency = DataUnitOfWork.Countries.GetCurrencyCodeByCountryCode(countryCode);
                    cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCountryCode(countryCode);
                }
                else
                    cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCurrency(currency);

                var currentSite = GetCurrentSite();
                var paymentMethodDetails = SettingsManager.GetDefault().Get((type == (int)Sell ? "SellPaymentMethodDetails" : "SitePaymentMethodDetails")).GetJsonData<PaymentBoundaryDetails>();
                var paymentMethodDetail = paymentMethodDetails;

                decimal spread = paymentMethodDetail.Spread;
                decimal fxMarkup = DataUnitOfWork.Currencies.Get(q => q.Code == currency).FirstOrDefault().FXMarkUp.Value;

                decimal factor = 1;
                if (type == (int)Buy)
                    factor = buyFactorValue(spread, fxMarkup);
                if (type == (int)Sell)
                    factor = sellFactorValue(spread, fxMarkup);

                var kraken_Rate = ((Dictionary<string, decimal?>)HttpContext.Application["LatestBTCEURRate"])[cryptoCurrency];
                var eur_rate = (HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>)[currency];
                //get BTC exchange rate
                var rate = OpenExchangeRates.GetBTCExchangeRate(currency, HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>, decimal.Parse(((Dictionary<string, decimal?>)HttpContext.Application["LatestBTCEURRate"])[cryptoCurrency].ToString()), cryptoCurrency);
                return kraken_Rate.Value * eur_rate * factor;
            }
            catch (Exception ex)
            {
                var nullCheckerResult = "";
                nullCheckerResult = NullChecker.GetResult(
                () => { return HttpContext.Application["LatestExchangeRates"]; },
                () => { return HttpContext.Application["LatestBTCEURRate"]; }
                );

                AuditLog.log("Error in LatestBTCBuySellRate2(" + currency + "):\r\n" + ex.ToMessageAndCompleteStacktrace() + " NullCheckerResult:" + nullCheckerResult,
                    (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                throw ex;
            }
        }

        // TODO: lot of reduntent/repeatation of code - on many RATE methods
        public JsonResult LatestBTCBuySellRate2(string currency = null, int type = 0, string cryptoCurrency = "BTC")
        {
            try
            {
                var rate = LatestCryptocurrencyRate(currency, type, cryptoCurrency);
                return new BetterJsonResult { Data = new { Rate = rate.Item1, FormatedRate = rate.Item2 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        // TODO: lot of reduntent/repeatation of code - on many RATE methods
        public Tuple<decimal, string> LatestCryptocurrencyRate(string currency = null, int type = 0, string cryptoCurrency = "BTC")
        {
            var res = LatestCryptocurrencyRateLocal(currency, type, cryptoCurrency);
            return new Tuple<decimal, string>(res.RateByKraken, res.FormatedRateByKraken);
        }

        // TODO: lot of reduntent/repeatation of code - on many RATE methods
        public Tuple<decimal, string> LatestCryptocurrencyRate2(string currency = null, string forCurrency = null, int type = 0, string cryptoCurrency = "BTC")
        {
            var res = LatestCryptocurrencyRateLocalSell(currency, forCurrency, type, cryptoCurrency);
            return new Tuple<decimal, string>(res.RateByKraken, res.FormatedRateByKraken);
        }

        // TODO: lot of reduntent/repeatation of code - on many RATE methods
        public ExRate LatestCryptocurrencyRateLocalSell(string currency = null, string forCurrency = null, int type = 0, string cryptoCurrency = "BTC")
        {
            try
            {
                var cultureInfo = CultureInfo.CurrentCulture.Name;
                //if currency is empty or null get culture info from User IP
                if (string.IsNullOrEmpty(currency))
                {
                    var countryCode = GetCountryCodeFromUserIP();
                    currency = DataUnitOfWork.Countries.GetCurrencyCodeByCountryCode(countryCode);
                    cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCountryCode(countryCode);
                }
                else
                    cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCurrency(currency);

                var currentSite = GetCurrentSite();
                var paymentMethodDetails = SettingsManager.GetDefault().Get((type == (int)Sell ? "SellPaymentMethodDetails" : "SitePaymentMethodDetails")).GetJsonData<PaymentBoundaryDetails>();
                var paymentMethodDetail = paymentMethodDetails;

                decimal spread = paymentMethodDetail.Spread;
                decimal fxMarkup = DataUnitOfWork.Currencies.Get(q => q.Code == forCurrency).FirstOrDefault().FXMarkUp.Value;

                decimal factor = 1;
                if (type == (int)Buy)
                    factor = buyFactorValue(spread, fxMarkup);
                if (type == (int)Sell)
                    factor = sellFactorValue(spread, fxMarkup);

                var kraken_Rate = ((Dictionary<string, decimal?>)HttpContext.Application["LatestBTCEURRate"])[cryptoCurrency];
                var eur_rate = (HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>)[currency];
                var rate = OpenExchangeRates.GetBTCExchangeRate(currency, HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>, kraken_Rate.Value, cryptoCurrency);
                var c_cate = rate * factor;
                var k_c_cate = kraken_Rate.Value * factor * eur_rate;

                return new ExRate()
                {
                    Rate = c_cate,
                    RateByKraken = k_c_cate,
                    FormatedRateByKraken = k_c_cate.ToString("N2", new CultureInfo(cultureInfo)) + " " + currency.ToUpper() + "/" + cryptoCurrency,
                    FormatedRate = (c_cate).ToString("N2", new CultureInfo(cultureInfo)) + " " + currency.ToUpper() + "/" + cryptoCurrency,
                    EURExchangeRate = eur_rate,
                    BTCExchangeRate = rate,
                    FxMarkup = fxMarkup,
                    Spread = spread,
                    Factor = factor,
                    KrakenRate = kraken_Rate.Value,
                };
            }
            catch (Exception ex)
            {
                var nullCheckerResult = "";
                nullCheckerResult = NullChecker.GetResult(
                () => { return HttpContext.Application["LatestExchangeRates"]; },
                () => { return HttpContext.Application["LatestBTCEURRate"]; }
                );

                AuditLog.log("Error in LatestBTCBuySellRate2(" + currency + "):\r\n" + ex.ToMessageAndCompleteStacktrace() + " NullCheckerResult:" + nullCheckerResult, (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                // var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                throw new Exception("An error has occured. Please contact the site administrator.");
                //result.AddError("An error has occured. Please contact the site administrator.");
                //return null;
            }
        }

        // TODO: lot of reduntent/repeatation of code - on many RATE methods
        public ExRate LatestCryptocurrencyRateLocal(string currency = null, int type = 0, string cryptoCurrency = "BTC")
        {
            try
            {
                var cultureInfo = CultureInfo.CurrentCulture.Name;
                //if currency is empty or null get culture info from User IP
                if (string.IsNullOrEmpty(currency))
                {
                    var countryCode = GetCountryCodeFromUserIP();
                    currency = DataUnitOfWork.Countries.GetCurrencyCodeByCountryCode(countryCode);
                    cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCountryCode(countryCode);
                }
                else
                    cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCurrency(currency);

                var currentSite = GetCurrentSite();
                var paymentMethodDetails = SettingsManager.GetDefault().Get((type == (int)Sell ? "SellPaymentMethodDetails" : "SitePaymentMethodDetails")).GetJsonData<PaymentBoundaryDetails>();
                var paymentMethodDetail = paymentMethodDetails;

                decimal spread = paymentMethodDetail.Spread;
                decimal fxMarkup = DataUnitOfWork.Currencies.Get(q => q.Code == currency).FirstOrDefault().FXMarkUp.Value;

                decimal factor = 1;
                if (type == (int)Buy)
                    factor = buyFactorValue(spread, fxMarkup);
                if (type == (int)Sell)
                    factor = sellFactorValue(spread, fxMarkup);

                var kraken_Rate = ((Dictionary<string, decimal?>)HttpContext.Application["LatestBTCEURRate"])[cryptoCurrency];
                var eur_rate = (HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>)[currency];
                var rate = OpenExchangeRates.GetBTCExchangeRate(currency, HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>, kraken_Rate.Value, cryptoCurrency);
                var c_cate = rate * factor;
                var k_c_cate = kraken_Rate.Value * factor * eur_rate;

                return new ExRate()
                {
                    Rate = c_cate,
                    RateByKraken = k_c_cate,
                    FormatedRateByKraken = k_c_cate.ToString("N2", new CultureInfo(cultureInfo)) + " " + currency.ToUpper() + "/" + cryptoCurrency,
                    FormatedRate = (c_cate).ToString("N2", new CultureInfo(cultureInfo)) + " " + currency.ToUpper() + "/" + cryptoCurrency,
                    EURExchangeRate = eur_rate,
                    BTCExchangeRate = rate,
                    FxMarkup = fxMarkup,
                    Spread = spread,
                    Factor = factor,
                    KrakenRate = kraken_Rate.Value,
                };
            }
            catch (Exception ex)
            {
                var nullCheckerResult = "";
                nullCheckerResult = NullChecker.GetResult(
                () => { return HttpContext.Application["LatestExchangeRates"]; },
                () => { return HttpContext.Application["LatestBTCEURRate"]; }
                );

                AuditLog.log("Error in LatestBTCBuySellRate2(" + currency + "):\r\n" + ex.ToMessageAndCompleteStacktrace() + " NullCheckerResult:" + nullCheckerResult, (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                // var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                throw new Exception("An error has occured. Please contact the site administrator.");
                //result.AddError("An error has occured. Please contact the site administrator.");
                //return null;
            }
        }

        public class ExRate
        {
            public decimal KrakenRate { get; set; }
            public decimal Spread { get; set; }
            public decimal FxMarkup { get; set; }
            public decimal Factor { get; set; }
            public decimal Rate { get; set; }
            public decimal RateByKraken { get; set; }
            public decimal EURExchangeRate { get; set; }
            public decimal BTCExchangeRate { get; set; }
            public string FormatedRate { get; set; }
            public string FormatedRateByKraken { get; set; }
        }

        [AllowCrossSite]
        public JsonResult LatestBTCRates(string currency = null, string cryptoCurrency = "BTC")
        {
            return new BetterJsonResult { Data = new { BuyRate = LatestBTCBuySellRate2(currency, (int)(Buy), cryptoCurrency), SellRate = LatestBTCBuySellRate2(currency, (int)(Sell)) }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private decimal buyFactorValue(decimal spread, decimal fxMarkup)
        {
            decimal factor = 1;
            factor = (1 + (spread / 100) + (fxMarkup / 100));  // factor = (1 + spread / 100) * (1 + fxMarkup / 100)
            return factor;
        }

        private decimal sellFactorValue(decimal spread, decimal fxMarkup)
        {
            decimal factor = 1;
            factor = (1 - (spread / 100) - (fxMarkup / 100)); // factor = (1 - spread / 100) * (1 - fxMarkup / 100);
            return factor;
        }

        /// <summary>
        /// method which returns latest BTC rate for selected currency, min and max size bound for amount and culture code
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="paymentMethod"></param>
        /// <returns></returns>
        [EnableThrottling(PerSecond = 5, PerMinute = 10)]
        public ActionResult CurrencyChanged(string currency, string forSellCurrency, string paymentMethod, int type, string cryptoCurrency)
        {
            try
            {
                var currencyDetails = DataUnitOfWork.Currencies.Get(q => q.Code == currency).FirstOrDefault();
                var cryptoCurrencyDetails = DataUnitOfWork.Currencies.Get(curr => curr.IsActive == true && curr.CurrencyTypeId == (int)Data.Enums.CurrencyTypes.Digital && curr.Code.ToLower() == cryptoCurrency.ToLower()).FirstOrDefault();
                if (currencyDetails == null)
                {
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    AuditLog.log("Purchase currency is null in CurrencyChanged().", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    result.AddError("PurchaseCurrencyEmpty");
                    return result;
                }
                if (cryptoCurrencyDetails == null)
                {
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    AuditLog.log("Purchase crypto currency is null in CurrencyChanged().", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    result.AddError("PurchaseCurrencyEmpty");
                    return result;
                }
                if (string.IsNullOrEmpty(paymentMethod))
                {
                    AuditLog.log("Payment Methods is null in CurrencyChanged().", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    result.AddError("paymentMethodEmpty");
                    return result;
                }
                var currentSite = GetCurrentSite();
                var euroRates = OpenExchangeRates.GetLatestExchangeRates().Rates;
                var euroCurrencyRate = OpenExchangeRates.GetEURExchangeRate(currency, euroRates);
                return new BetterJsonResult
                {
                    Data = new
                    {
                        LatestBTCRate = LatestBTCRate2(currency, cryptoCurrency), //get latest BTC rate for currency
                        LatestSellRate = LatestBTCRate2(forSellCurrency, cryptoCurrency),
                        OrderSizeBoundary = GetOrderSizeBoundary(currentSite.Id, paymentMethod, type == (int)Buy ? currency : forSellCurrency, type), //get order size boundry
                        CultureCode = DataUnitOfWork.Countries.GetCultureCodeByCurrency(currency), //get culture code from Country table
                        EuroBtcRate = decimal.Parse(((Dictionary<string, decimal?>)HttpContext.Application["LatestBTCEURRate"])[cryptoCurrency].ToString()),
                        EuroCurrencyRate = euroCurrencyRate,
                        FxMarkUp = currencyDetails.FXMarkUp,
                        BuyTicker = LatestBTCBuySellRate2(currency, (int)Buy, cryptoCurrency),
                        SellTicker = LatestBTCBuySellRate2(currency, (int)Sell, cryptoCurrency)
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;
                AuditLog.log("Error in CurrencyChanged(" + currency + "):\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        /// <summary>
        /// method to get order size boundary when payment method drop down changed
        /// </summary>
        /// <param name="paymentMethod"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult PaymentMethodChanged(string paymentMethod, string currency, string sellCurrency, int type)
        {
            try
            {
                var currencyDetails = DataUnitOfWork.Currencies.Get(q => q.Code == currency).FirstOrDefault();
                if (type == (int)Sell)
                {
                    var sellCurrencyDetails = DataUnitOfWork.Currencies.Get(q => q.Code == sellCurrency).FirstOrDefault();
                    if (sellCurrencyDetails == null)
                    {
                        var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        AuditLog.log("Purchase Sell Currency is null in PaymentMethodChanged()", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                        result.AddError("PurchaseCurrencyEmpty");
                        return result;
                    }
                }
                if (currencyDetails == null)
                {
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    AuditLog.log("Purchase Currency is null in PaymentMethodChanged()", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    result.AddError("PurchaseCurrencyEmpty");
                    return result;
                }
                if (string.IsNullOrEmpty(paymentMethod))
                {
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    AuditLog.log("Payment Method is null in PaymentMethodChanged()", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    result.AddError("paymentMethodEmpty");
                    return result;
                }
                var currentSite = GetCurrentSite();
                return new BetterJsonResult { Data = new { OrderSizeBoundary = GetOrderSizeBoundary(currentSite.Id, paymentMethod, type == (int)Buy ? currency : sellCurrency, type) }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;
                AuditLog.log("Error in PaymentMethodChanged(" + currency + "):\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        /// <summary>
        /// method to get Phone number style for selected phone code
        /// </summary>
        /// <param name="phonecode"></param>
        /// <returns></returns>
        [EnableThrottling(PerSecond = 5, PerMinute = 10)]
        public JsonResult GetPhoneNumberStyle(string phonecode)
        {
            try
            {
                var code = 0;
                if (int.TryParse(phonecode.Replace("+", ""), out code))
                {
                    //  var definition = new { PhoneNumberStyle = "", CardFee = 0.0};
                    //get phone number style from country table
                    var CountryDetails = DataUnitOfWork.Countries.Get(q => q.PhoneCode == code).Select(q => new { PhoneNumberStyle = q.PhoneNumberStyle, CardFee = q.CardFee }).FirstOrDefault();
                    return new BetterJsonResult { Data = new { PhoneNumberStyle = CountryDetails.PhoneNumberStyle, CardFee = CountryDetails.CardFee }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    AuditLog.log("Phone code '" + phonecode + "' can't be parsed correctly.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    throw new Exception("Phone code '" + phonecode + "' can't be parsed correctly.");
                }
            }
            catch (Exception ex)
            {
                AuditLog.log(string.Format("Error in GetPhoneNumberStyle({0})\r\n{1}", phonecode, ex.ToMessageAndCompleteStacktrace()), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var response = ControllerContext.HttpContext.Response;
                response.StatusCode = 400;
                response.Write("An error has occured. Please contact the site administrator.");
                return new JsonResult();
            }
        }

        /// <summary>
        /// method to validate entered bitcoin address
        /// </summary>
        /// <param name="bitcoinAddress"></param>
        /// <returns></returns>
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public string ValidateBitcoinAddress(string bitcoinAddress, string forDigitalCurrency)
        {
            try
            {
                string retVal = "Invalid";
                if (BitGoUtil.ValidateAddress(bitcoinAddress, forDigitalCurrency))
                {
                    retVal = "Valid";
                }

                return retVal;
            }
            catch (Exception)
            {
                AuditLog.log("Bitcoin Address" + bitcoinAddress + " is invalid.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                return "Invalid";
            }
        }

        /// <summary>
        /// after submitting identity verification redirect to payment procedure page based on paymentGatewayType
        /// </summary>
        /// <param name="withKYC"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeUser]
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult SubmitIdentityVerification(bool withKYC, string lang)
        {
            try
            {
                var hostname = Request.Url.Host;
                var currentSite = DataUnitOfWork.Sites.Get(q => q.Url == hostname).FirstOrDefault();
                SetSiteNameViewBag(hostname);

                // TODO: select fields - see all below DataUnitOfWork selection
                var userSessionModel = ViewBag.UserSessionModel as UserSessionModel;
                var order = DataUnitOfWork.Orders.GetById(userSessionModel.OrderId); //get order details from order table
                var user = DataUnitOfWork.Users.GetById(order.UserId); //get user details from user table
                var currency = DataUnitOfWork.Currencies.GetById(userSessionModel.CurrencyId); //get currency details from currency table
                var cryptoCurrency = DataUnitOfWork.Currencies.GetById(order.CryptoCurrencyId);
                var ci = new CultureInfo(userSessionModel.CultureInfo);
                var googleTagManagerId = DataUnitOfWork.Sites.GetById(currentSite.Id).GoogleTagManagerId;
                var country = DataUnitOfWork.Countries.Get(q => q.Code == order.IpCode).FirstOrDefault();

                if (userSessionModel.KycRequirement != "NONE")
                    if (withKYC == false)
                    {
                        AuditLog.log("Something went wrong, you need to provide KYC files.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                        var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        result.AddErrorKey("KYC files error", ci.ThreeLetterISOLanguageName);
                        return result;
                    }
                //if withKYC is true then send notification using pushover 
                if (withKYC)
                {
                    var testOrProd = "";
                    //Appending "TEST:" to Pushover if payment type is Credit card
                    if (order.PaymentType == 1)
                        if (!currentSite.Text.Contains("localhost"))
                            if (currentSite.Text.Split('.')[0].ToLower() == "apptest")
                                testOrProd = "TEST: ";
                    PushoverHelper.SendNotification(testOrProd + (currentSite.Text.Contains("localhost") ? currentSite.Text : currentSite.Text.Split('.')[1]), order.Number + " KYC AWAITS APPROVAL :" + order.Name);
                }

                //if Phone number is not verified
                if (!userSessionModel.PhoneNumberVerified)
                {
                    AuditLog.log("User " + user.Phone + " trying to access payment procedure before verifying phone number.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "VerifyYouPhoneNumber");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }

                IMessageService twilio = TwilioService.GetDefault(currentSite.Id);
                AuditLog.log("Caller Id response for phone number " + user.Phone + " - " + (twilio.IsConfigured() ? twilio.GetCallerIdentity(user.Phone) : "TwilioNotConfigured"), (int)Data.Enums.AuditLogStatus.Twilio, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                AuditLog.log("Order submitted by user with phone number " + user.Phone + ". User's location details are '" + Session["IPInfoJSONData"] as string + "'.", (int)Data.Enums.AuditLogStatus.IPInfo, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                //generate new Guid and assign it to PaymentValidationKey
                userSessionModel.PaymentValidationKey = Guid.NewGuid();

                //Approve the FaceTec kyc
                var facetecKycFiles = DataUnitOfWork.KycFiles.Get(q => q.UserId == order.UserId && (q.Type == (long)WMC.Data.Enums.KYCFileTypes.PhotoID || q.Type == (long)WMC.Data.Enums.KYCFileTypes.SelfieID) && q.Approved == null);

                foreach (var kycfile in facetecKycFiles)
                {
                    if (kycfile.FaceTecStatus == (int)WMC.Data.Enums.FaceTecStatus.Success && !string.IsNullOrEmpty(kycfile.SessionId) && kycfile.Approved == null)
                    {
                        kycfile.Approved = DateTime.Now;
                        kycfile.ApprovedBy = SystemUsers.FaceTec;
                        DataUnitOfWork.KycFiles.Update(kycfile);
                        DataUnitOfWork.Commit();
                    }
                }

                var paymentGatewayType = order.PaymentGatewayType;
                string paymentOption = null;
                if (order.PaymentType == 1) //"CreditCard"
                {
                    if (paymentGatewayType == "PayLike")
                        paymentOption = "PayLike";
                    else if (paymentGatewayType == "YourPay")
                        paymentOption = "YourPay";
                    else if (paymentGatewayType == "TrustPay")
                        paymentOption = "TrustPay";
                    else
                        paymentOption = "QuickPay";
                }
                else //Bank
                {
                    if (order.Type == (int)Sell)
                        paymentOption = "Sell";
                    else
                        paymentOption = "BankBuy";
                }

                // TODO: can't we move resposibility based on option?
                switch (paymentOption)
                {
                    case "PayLike":
                        {
                            dynamic payLikeDetails = PayLikeService.GetPayLikeDetails(currentSite.Id, userSessionModel.CurrencyId); //get PayLike details
                            return PartialView("PaymentPayLike", new PayLikePaymentModel
                            {
                                MerchantNumber = payLikeDetails.PublicKey.ToString(),
                                //MerchantNumber = payLikeDetails.PayLikePublicKey.ToString(),
                                TxSecret = order.TxSecret,
                                OrderNumber = order.Number,
                                Amount = userSessionModel.OrderAmount.ToString("N2", new CultureInfo(userSessionModel.CultureInfo)),
                                Commission = (userSessionModel.OrderAmount / 100 * userSessionModel.CommissionPercent * DataUnitOfWork.Orders.GetOrderDiscount(userSessionModel.OrderId)).ToString("N2", ci),
                                GoogleTagManagerId = googleTagManagerId,
                                YourPayAmount = userSessionModel.OrderAmount * 100, // Multiply by 100 for YourPay
                                CurrencyCode = currency.Code,
                                SiteName = hostname.Contains("localhost") ? hostname : hostname.Split('.')[1] + "." + hostname.Split('.')[2]
                            });
                        }
                    case "YourPay":
                        {
                            PaymentModel paymentModel = new PaymentModel
                            {
                                MerchantNumber = YourpayService.GetYourPayMerchantNumber(currentSite.Id),
                                OrderNumber = order.Number,
                                Amount = (order.Type == (int)Sell ? "0" : userSessionModel.OrderAmount.ToString("N2", new CultureInfo(userSessionModel.CultureInfo))),
                                Commission = (userSessionModel.OrderAmount / 100 * userSessionModel.CommissionPercent * DataUnitOfWork.Orders.GetOrderDiscount(userSessionModel.OrderId)).ToString("N2", ci),
                                GoogleTagManagerId = googleTagManagerId,
                                Currency = currency.Code,
                                YourPayAmount = (order.Type == (int)Sell ? 0 : userSessionModel.OrderAmount * 100), // Multiply by 100 for YourPay
                                ShopPlatform = "Prestashop",
                                Time = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                                Use3d = "0",
                                TxSecret = order.TxSecret,
                                Cardholder = order.Name,
                                BTCaddress = (order.Type == (int)Sell ? "" : order.CryptoAddress),
                                Phone = user.Phone,
                                Email = user.Email,
                                CurrencyCode = currency.YourPayCurrencyCode,
                                Cartid = Convert.ToInt32(order.Number),
                                Lang = userSessionModel.CultureInfo,
                                CT = "off",
                                Comments = "",
                                YourpayCallBackMethod = "/CryptoCurrencyPaymentInfo",
                                Type = order.Type
                            };
                            return PartialView("Payment", paymentModel);
                        }
                    case "TrustPay":
                        {
                            TrustPayService trustPayService = new TrustPayService();
                            string descriptor = null;
                            if (country.AddTx != null) { descriptor = order.TxSecret; }
                            var preAuthorizationResponse = trustPayService.PreAuthorization(currency.Code, userSessionModel.OrderAmount, order.Number, hostname.Contains("localhost") ? hostname : hostname.Split('.')[1] + "." + hostname.Split('.')[2] + ":" + descriptor, currentSite.Id);
                            AuditLog.log("PreAuthorization response for order with phone number " + user.Phone + ":" + SimpleJson.SerializeObject(preAuthorizationResponse), (int)Data.Enums.AuditLogStatus.TrustPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                            string checkoutId = preAuthorizationResponse.id;
                            return PartialView("PaymentTrustPay", new TrustPayPaymentModel
                            {
                                CheckoutId = checkoutId,
                                Source = "https://" + (trustPayService.GetTrustPayDetails(currentSite.Id).IsProd ? "" : "test.") + string.Format("oppwa.com/v1/paymentWidgets.js?checkoutId={0}", checkoutId),
                                CardHolderName = order.Name,
                                Destination = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Host + ":" + HttpContext.Request.Url.Port
                            });
                        }
                    case "QuickPay":
                        {
                            var quickPayService = new QuickPayService((long)order.SiteId);
                            ///get siteName for text_on_statement Ex: monni.com:<txsecret>  //TODO GET SITE NAME TO METHOD
                            string siteName = DataUnitOfWork.Sites.GetById((long)order.SiteId).Text;
                            string site = siteName.Contains("localhost") ? siteName : siteName.Contains("test") ? siteName.Split('.')[2] + "." + siteName.Split('.')[3] : siteName.Contains("app") ? siteName.Split('.')[1] + "." + siteName.Split('.')[2] : siteName;
                            string text_on_statement = site + ":" + order.TxSecret;
                            var orderNumber = Convert.ToInt64(order.Number); //order_id as orderNumber(order_id length 4 to 20 charaters)
                            //create payment using order_id and currency
                            var createPayment = quickPayService.CreatePayment(order.Id, orderNumber, order.CurrencyCode, text_on_statement);
                            dynamic createPaymentContent = JsonConvert.DeserializeObject(createPayment.Content);
                            long paymentId = createPaymentContent.id;
                            long minorUnits = Convert.ToInt64(Math.Pow(10, currency.MinorUnits));
                            //convert order.Amount in proper format 
                            long amount = Convert.ToInt64(userSessionModel.OrderAmount * minorUnits);
                            //create payment link using payment_id 
                            var createPaymentLink = quickPayService.CreatePaymentLink(paymentId, amount, order.Id);
                            dynamic createPaymentLinkContent = JsonConvert.DeserializeObject(createPaymentLink.Content);
                            string paymentLink = createPaymentLinkContent.url;
                            userSessionModel.PaymentId = paymentId.ToString();
                            QuickPayPaymentModel paymentModel = new QuickPayPaymentModel
                            {
                                paymentLink = paymentLink,
                                paymentId = paymentId,
                                OrderNumber = order.Number,
                                AmountStr = (userSessionModel.OrderAmount).ToString("N2", new CultureInfo(userSessionModel.CultureInfo)) + " " + order.CurrencyCode,
                                Amount = order.Amount.Value
                            };
                            order.ExtRef = paymentId.ToString();
                            DataUnitOfWork.Commit();
                            //return PartialView("PaymentQuickPayForm", paymentModel);
                            return PartialView("PaymentQuickPay", paymentModel);

                        }
                    case "BankBuy":
                        {
                            var site = DataUnitOfWork.Sites.GetById(order.SiteId.Value);
                            //feature lock: Below webconfig are read and checked for only special case handling  
                            string bankBuySetting = null;
                            //ThresholdSettings thresholdSettings = SettingsManager.GetDefault().Get("ThresholdDetails").GetJsonData<ThresholdSettings>();
                            //Threshold config are stopped
                            //string thresholdConfigOverride = thresholdSettings.BankBuyAmountThreshold;
                            //if (!string.IsNullOrEmpty(thresholdConfigOverride))
                            //{
                            //    var amountThreshold = Convert.ToDecimal(thresholdConfigOverride);
                            //    string buyBankcurrency = thresholdSettings.BankBuyCurrency;
                            //    if (order.Amount >= amountThreshold && currency.Code == buyBankcurrency)
                            //    {
                            //        bankBuySetting = JsonConvert.SerializeObject(thresholdSettings.BankBuySettings);
                            //    }
                            //}
                            var bankPaymentSettings = new PurchaseLogic(DataUnitOfWork).GetBankPaymentDetailsForBuyAsLangDictionary(currency.Code, bankBuySetting);
                            //send email to user mail address
                            var emailParams = new Dictionary<string, object> {
                                { "UserFirstName", order.Name },
                                { "OrderAmount", order.Amount.Value.ToString("N2", new CultureInfo(userSessionModel.CultureInfo)) },
                                { "OrderCurrency", currency.Code },
                                { "OrderNumber", order.Number },
                                { "CryptoAddress", order.CryptoAddress },
                            };

                            string bankPaymentDetailHtmlPart = string.Empty;
                            if (bankPaymentSettings != null)
                            {
                                foreach (var item in bankPaymentSettings)
                                {
                                    // TODO: this must be repeated from html template??
                                    // emailParams.Add(item.Key, item.Value);
                                    var strHtml = @"<tr>";
                                    strHtml += @"    <td align=""left"" background=""#m_4798224270076370977_f9f9f9"" style=""word-break:break-word;background:#f9f9f9;font-size:0px;padding:0px 25px 0px 25px;padding-right:25px;padding-left:25px"">";
                                    strHtml += @"        <div style=""color:#000000;font-family:Ubuntu,Helvetica,Arial,sans-serif;font-size:13px;line-height:10px"">";
                                    strHtml += @"            <p><span style=""font-size:14px"">" + WebUtility.HtmlEncode(Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", @item.Key)) + ":</span></p>";
                                    strHtml += @"        </div>";
                                    strHtml += @"    </td>";
                                    strHtml += @"    <td align=""right"" background=""#m_4798224270076370977_f9f9f9"" style=""word-break:break-word;background:#f9f9f9;font-size:0px;padding:0px 25px 0px 25px;padding-right:25px;padding-left:25px"">";
                                    strHtml += @"        <div style=""color:#000000;font-family:Ubuntu,Helvetica,Arial,sans-serif;font-size:13px;line-height:10px"">";
                                    strHtml += @"            <p style=""text-align: right;""><span style=""font-size:14px"">" + WebUtility.HtmlEncode(item.Value) + "</span></p>";
                                    strHtml += @"        </div>";
                                    strHtml += @"    </td>";
                                    strHtml += @"</tr>";
                                    bankPaymentDetailHtmlPart += strHtml;
                                }
                            }
                            EmailHelper.SendEmail(user.Email, "PaymentInstructions", emailParams, site.Text, order.BccAddress, customHtml: bankPaymentDetailHtmlPart);
                            var orderStatus = DataUnitOfWork.OrderStatus.Get(x => x.Id == order.Status).Select(q => q.Text).FirstOrDefault();
                            return PartialView("PaymentInstructions", new PaymentInstructionsModel
                            {
                                User = order.Name,
                                MessageToReciever = order.Number,
                                Amount = order.Amount.Value.ToString("N2", new CultureInfo(userSessionModel.CultureInfo)),
                                Currency = currency.Code,
                                BitcoinAddress = order.CryptoAddress,
                                Bank = "Bank",
                                RegistrationNumber = "Reg No",
                                AccountNumber = "AN",
                                PaymentDetails = bankPaymentSettings,
                                ReferenceId = order.ReferenceId,
                                OrderStatus = orderStatus,
                                OrderDate = order.Quoted,
                                OrderId = order.Id,
                                OrderAmount = order.Amount.Value
                            });
                        }
                    case "Sell":
                        {
                            SellPayment sellPayment = new SellPayment();
                            if (order.PaymentType == 1) //Credit Card
                            {
                                TrustPayService trustPayService = new TrustPayService();
                                string descriptor = null;
                                if (country.AddTx != null) { descriptor = order.TxSecret; }
                                var preAuthorizationResponse = trustPayService.PreAuthorization(currency.Code, userSessionModel.OrderAmount, order.Number, hostname.Contains("localhost") ? hostname : hostname.Split('.')[1] + "." + hostname.Split('.')[2] + ":" + descriptor, currentSite.Id);
                                //var preAuthorizationResponse = trustPayService.PreAuthorization(currency.Code, userSessionModel.OrderAmount, order.Number, hostname.Contains("localhost") ? hostname : hostname.Split('.')[1] + "." + hostname.Split('.')[2] + ":" + order.TxSecret);
                                AuditLog.log("PreAuthorization response for order with phone number " + user.Phone + ":" + SimpleJson.SerializeObject(preAuthorizationResponse), (long)Data.Enums.AuditLogStatus.TrustPay, (long)Data.Enums.AuditTrailLevel.Info, order.Id);
                                string checkoutId = preAuthorizationResponse.id;
                                sellPayment.trustPayPaymentModel = new TrustPayPaymentModel
                                {
                                    CheckoutId = checkoutId,
                                    Source = "https://" + (trustPayService.GetTrustPayDetails(currentSite.Id).IsProd ? "" : "test.") + string.Format("oppwa.com/v1/paymentWidgets.js?checkoutId={0}", checkoutId),
                                    CardHolderName = order.Name,
                                    Destination = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Host + ":" + HttpContext.Request.Url.Port
                                };
                            }
                            else
                            {
                                if ((order.IBAN != "") || (order.AccountNumber != "" && order.Reg != ""))
                                {
                                    userSessionModel.PaymentResponse = "CryptoCurrencyPayment";
                                }
                                else
                                {
                                    sellPayment.orderModel = new OrderModel
                                    {
                                        Reg = order.Reg,
                                        IBAN = order.IBAN,
                                        SwiftCode = order.SwiftBIC,
                                        AccountNumber = order.AccountNumber
                                    };
                                }
                            }

                            // TODO: where this should be fetched and updated :(
                            var bankPaymentSettings = new PurchaseLogic(DataUnitOfWork).GetBankPaymentDetailsForSell(order.CurrencyCode);
                            sellPayment.PaymentDetails = bankPaymentSettings;

                            //QRCode process
                            var appSettings = DataUnitOfWork.AppSettings.Get();
                            // var bitGoAccessCodeJSON = appSettings.FirstOrDefault(q => q.ConfigKey == "BitGoAccessCode").ConfigValue; //Get BitGoAccessCode value
                            BitGoAccessSettings bitGoAccessSettings = SettingsManager.GetDefault().Get("BitGoAccessCode", true).GetJsonData<BitGoAccessSettings>(); //  JsonConvert.DeserializeObject<BitGoAccessSettings>(bitGoAccessCodeJSON);
                            var bitgoClient = new BitGoAccess(bitGoAccessSettings, cryptoCurrency.Code);
                            var sessionJSON = bitgoClient.Session();
                            if (sessionJSON.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                            {
                                AuditLog.log("Error in CryptoCurrencyPaymentInfo(). BitGo session is Unauthorized.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                                return View("Error", new ErrorModel { ErrorText = "Error in processing the order.", ErrorDescription = "Please contact the site administrator." });
                            }
                            dynamic sessionObject = SimpleJson.DeserializeObject(sessionJSON.Content);
                            var bitgoCurrencySetting = DataUnitOfWork.Currencies.Get(cur => cur.Code == cryptoCurrency.Code).FirstOrDefault().BitgoSettings;
                            dynamic bitgoSetting = JsonConvert.DeserializeObject(bitgoCurrencySetting);
                            DateTime dateTimeObj;
                            if (DateTime.TryParse(sessionObject.session.expires.ToString(), out dateTimeObj) && DateTime.Compare(dateTimeObj, DateTime.Now) < 0)
                                throw new Exception("BitGo session is locked.");
                            var wallet = bitgoClient.GetWallet(bitgoSetting.DefaultWalletId.ToString());
                            var cryptoAddress = bitgoClient.CreateWalletAddress(wallet.id);
                            //var cryptoAddress = "18DmrDjMwqHZaSVeWtaz7VjhsEkr5J2D18";
                            order.CryptoAddress = cryptoAddress;
                            var site = DataUnitOfWork.Sites.GetById(order.SiteId.Value);
                            DataUnitOfWork.Commit();

                            var ciQR = new CultureInfo("en-US"); //get culture info

                            string qrCstring = string.Format("bitcoin:{0}?amount={1}", cryptoAddress, order.BTCAmount.Value.ToString("N8", ciQR));

                            string SigBase64 = QRCodeHelper.GenerateBase64QRCode(qrCstring);


                            sellPayment.cryptoCurrencyPaymentInstructionModel = new CryptoCurrencyPaymentInstructionModel
                            {
                                Amount = order.BTCAmount.Value.ToString("N8", ci),
                                BitcoinAddress = order.CryptoAddress,
                                CryptoCurrencyCode = DataUnitOfWork.Currencies.Get(curr => curr.Id == order.CryptoCurrencyId && curr.CurrencyTypeId == (int)Data.Enums.CurrencyTypes.Digital).FirstOrDefault().Code,
                                QRCodeImage = "data:image/png;base64," + SigBase64,
                                OrderNumber = order.Id.ToString(),
                                GoogleTagManagerId = DataUnitOfWork.Sites.GetById(GetCurrentSite().Id).GoogleTagManagerId,
                                Commission = order.CommissionProcent.ToString(),
                                Currency = order.CurrencyCode
                            };
                            return View("SellPaymentNInstruction", sellPayment);

                        }



                    default:
                        {
                            // log payment error in paymentGatewayType
                            AuditLog.log("Invalid paymentGatewayType", (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                            AuditLog.log("Order submitted by user with phone number " + user.Phone + ". User's location details are '" + Session["IPInfoJSONData"] as string + "'.", (int)Data.Enums.AuditLogStatus.IPInfo, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                            break;
                        }
                }
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in SubmitIdentityVerification(): " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        // TODO: Save IBAN and send email??
        [AuthorizeUser]
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult SaveIBAN(string lang, string SwiftCode = null, string IBAN = null, string Reg = null, string Account = null)
        {
            try
            {
                var userSessionModel = ViewBag.UserSessionModel as UserSessionModel;
                var order = DataUnitOfWork.Orders.GetById(userSessionModel.OrderId);
                userSessionModel.PaymentResponse = "CryptoCurrencyPayment";
                var currency = DataUnitOfWork.Currencies.Get(x => x.Id == userSessionModel.CurrencyId).Select(q => q.Code).FirstOrDefault();
                List<string> errors;
                if (PurchaseLogic.ValidateAccountDetailsForSell(currency, SwiftCode, IBAN, Reg, Account, out errors))
                {
                    AuditLog.log("Error in SaveIBAN(): IBAN:" + IBAN + "Reg: " + Reg + "Account: " + Account + errors, (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", errors[0]);
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }
                order.AccountNumber = Account;
                order.Reg = Reg;
                order.IBAN = IBAN;
                order.SwiftBIC = SwiftCode;
                order.Status = (int)Data.Enums.OrderStatus.Quoted;
                DataUnitOfWork.Commit();
                userSessionModel.PaymentValidationKey = Guid.Empty;
                var ci = new CultureInfo(userSessionModel.CultureInfo);
                var site = DataUnitOfWork.Sites.GetById(order.SiteId.Value);
                EmailHelper.SendEmail(order.Email, "CryptoCurrencyPaymentInstructions", new Dictionary<string, object> {
                                { "UserFirstName", order.Name },
                                { "OrderAmount", order.BTCAmount.Value.ToString("N8", ci) },
                                { "OrderNumber", order.Number },
                                { "OrderCurrency", order.CurrencyCode },
                                { "CryptoAddress", order.CryptoAddress },
                            }, site.Text, order.BccAddress);
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in SaveIBAN(): IBAN:" + IBAN + "Reg: " + Reg + "Account: " + Account + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                //result.AddError("Error in identity verification. Please contact the site administrator.");
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "BankDetailsSavingError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
            return new EmptyResult();
        }

        /// <summary>
        /// method provides payment procedure for PayLike service
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        [AuthorizeUser]
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult PayLikeAccept(string tid)
        {
            try
            {
                var lang = CultureInfo.CurrentCulture.Name;
                var hostname = HttpContext.Request.Url.Host;
                var currentSite = DataUnitOfWork.Sites.Get(q => q.Url == hostname).FirstOrDefault();

                var appSettings = DataUnitOfWork.AppSettings.GetAll();
                //dynamic payLikeDetails = JsonConvert.DeserializeObject(appSettings.FirstOrDefault(q => q.ConfigKey == "PayLikeDetails").ConfigValue); //test
                var userSessionModel = ViewBag.UserSessionModel as UserSessionModel;
                dynamic payLikeDetails = PayLikeService.GetPayLikeDetails(currentSite.Id, userSessionModel.CurrencyId); //get PayLike details
                //var paylikeTransactionService = new PaylikeTransactionService(payLikeDetails.AppKey.ToString());
                var paylikeTransactionService = new PaylikeTransactionService(payLikeDetails.AppKey.ToString());
                var paylikeTransaction = paylikeTransactionService.GetTransaction(new GetTransactionRequest { TransactionId = tid }); //get transaction details
                if (!paylikeTransaction.IsError) //if there is no error in transaction
                {
                    var order = DataUnitOfWork.Orders.Get(q => q.Id == userSessionModel.OrderId).FirstOrDefault(); //get order details from Order table
                    var currency = DataUnitOfWork.Currencies.GetById(userSessionModel.CurrencyId); //get currency details from Currency table

                    //generate new Guid as creditCardUserIdentity
                    var creditCardUserIdentity = Guid.NewGuid();
                    order.CreditCardUserIdentity = creditCardUserIdentity;
                    //if transaction amount of Paylike and order amount from Order table doesnot match then change order status to 19
                    if (paylikeTransaction.Content.Amount != userSessionModel.OrderAmount * 100)
                    {
                        order.Status = (int)Data.Enums.OrderStatus.PaymentAborted;
                        DataUnitOfWork.Commit();
                        var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ErrorinPayment");
                        result.AddErrorKey(errorMesaage, lang);
                        return result;
                    }
                    //if transaction OrderNumber of Paylike and OrderNumber from Order table doesnot match then change order status to 19
                    if (paylikeTransaction.Content.Custom["OrderNumber"] != order.Number)
                    {
                        order.Status = (int)Data.Enums.OrderStatus.PaymentAborted;
                        DataUnitOfWork.Commit();
                        var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ErrorinPayment");
                        result.AddErrorKey(errorMesaage, lang);
                        return result;
                    }
                    //if transaction PaymentValidationKey is equal to empty Guid then change order status to 19
                    if (userSessionModel.PaymentValidationKey == Guid.Empty)
                    {
                        order.Status = (int)Data.Enums.OrderStatus.PaymentAborted;
                        DataUnitOfWork.Commit();
                        var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ErrorinPayment");
                        result.AddErrorKey(errorMesaage, lang);
                        return result;
                    }

                    userSessionModel.PaymentValidationKey = Guid.Empty;
                    var ci = new CultureInfo(userSessionModel.CultureInfo);
                    order.CreditCardUserIdentity = Guid.NewGuid();
                    //if KycRequirement is NONE then change order status to Sending else change status to Paid
                    if (userSessionModel.KycRequirement == "NONE")
                        order.Status = (int)Data.Enums.OrderStatus.Sending;
                    else
                        order.Status = (int)Data.Enums.OrderStatus.Paid;
                    string uuid = CommonUUID.UUID();
                    //assign values to Transaction model property
                    //TODO Later for credit cards
                    var transaction = new Transaction
                    {
                        Amount = userSessionModel.OrderAmount,
                        MethodId = 1,
                        Type = 1,
                        ExtRef = tid.ToString(),
                        Currency = userSessionModel.CurrencyId,
                        FromAccount = 1,
                        ToAccount = 8,
                        //Completed = DateTime.Now,
                        Info = paylikeTransaction.Content.Card.Bin + "XXXXXX" + paylikeTransaction.Content.Card.Last4 + "," + userSessionModel.FullName,
                        BatchNumber = uuid
                    };
                    order.CardNumber = paylikeTransaction.Content.Card.Bin + "XXXXXX" + paylikeTransaction.Content.Card.Last4;
                    order.ExtRef = tid.ToString();
                    order.Transactions.Add(transaction);
                    DataUnitOfWork.Commit();
                    AuditLog.log("Updating order by user with phone number " + userSessionModel.PhoneNumber + ".\r\nOrder details :\r\n" +
                        JsonSerializerEx.SerializeObject(order, 1), (int)Data.Enums.AuditLogStatus.PayLike, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                    AuditLog.log("Payment made by user with phone number " + userSessionModel.PhoneNumber + ".\r\nPayLike details for this transaction:\r\n" +
                        JsonConvert.SerializeObject(paylikeTransaction), (int)Data.Enums.AuditLogStatus.PayLike, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                    var site = DataUnitOfWork.Sites.GetById(order.SiteId.Value);
                    //var minersFee = appSettings.FirstOrDefault(q => q.ConfigKey == "MinersFee").ConfigValue;
                    var user = DataUnitOfWork.Users.Get(q => q.Phone == userSessionModel.PhoneNumber).FirstOrDefault();
                    //send email to user email address
                    EmailHelper.SendEmail(user.Email, "PaymentReceived", new Dictionary<string, object> {
                                { "UserFirstName", userSessionModel.FullName },
                                { "OrderAmount", userSessionModel.OrderAmount.ToString("N2", ci) },
                                { "OrderCurrency", currency.PayLikeCurrencyCode },
                                { "OrderCommission", (userSessionModel.OrderAmount / 100 * userSessionModel.CommissionPercent*DataUnitOfWork.Orders.GetOrderDiscount(userSessionModel.OrderId)).ToString("N2", ci) },
                                { "CryptoAddress", userSessionModel.CryptoAddress },
                                { "CardName", userSessionModel.FullName },
                                { "CardNumber", paylikeTransaction.Content.Card.Bin + "XXXXXX" + paylikeTransaction.Content.Card.Last4  },
                                { "OrderNumber", order.Number },
                                { "MinersFee", (order.MinersFee.HasValue ? order.MinersFee.Value.ToString() : "0") + " BTC"}
                            }, site.Text, order.BccAddress);

                    var testOrProd = "";
                    //Appending "TEST:" to Pushover if payment type is Credit card
                    if (order.PaymentType == 1)
                        if (!currentSite.Text.Contains("localhost"))
                            if (currentSite.Text.Split('.')[0].ToLower() == "apptest")
                                testOrProd = "TEST: ";
                    PushoverHelper.SendNotification(testOrProd + (currentSite.Text.Contains("localhost") ? currentSite.Text : currentSite.Text.Split('.')[1]), order.Number + " PAYMENT: " + order.Amount.Value.ToString("N0", new CultureInfo("da-DK")) + " " + currency.Code + ", " + order.Name + ", " + order.CommissionProcent * DataUnitOfWork.Orders.GetOrderDiscount(userSessionModel.OrderId) + " %");

                    paylikeTransaction = paylikeTransactionService.GetTransaction(new GetTransactionRequest { TransactionId = tid });
                    if (paylikeTransaction.IsError) //if transaction has any error then send error message
                    {
                        AuditLog.log("Error in PayLike Receipt(" + tid + ")\r\n: Transaction error.", (int)Data.Enums.AuditLogStatus.PayLike, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                        var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ErrorinPayment");
                        result.AddErrorKey(errorMesaage, lang);
                        return result;
                    }
                    if (ViewBag.UserSessionModel == null) //if UserSessionModel is null then send error message
                    {
                        AuditLog.log("Error in PayLike Receipt(" + tid + ")\r\n: User session unavailable.", (int)Data.Enums.AuditLogStatus.PayLike, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                        var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ErrorinPayment");
                        result.AddErrorKey(errorMesaage, lang);
                        return result;
                    }

                    SetSiteNameViewBag(hostname);
                    userSessionModel.PaymentResponse = "Receipt";
                    return View("YourPayRedirect");
                }
                else //if there is any error in transaction change order status to 19
                {
                    var order = DataUnitOfWork.Orders.GetById(userSessionModel.OrderId);
                    order.Status = (int)Data.Enums.OrderStatus.PaymentAborted;
                    DataUnitOfWork.Commit();
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    AuditLog.log("Error in validating payment process for order with phone number " + userSessionModel.PhoneNumber + ".\r\nPayLike details for this transaction:\r\n" +
                        JsonConvert.SerializeObject(paylikeTransaction), (int)Data.Enums.AuditLogStatus.PaymentError, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                    result.AddError("Error in validating payment process.");
                    return result;
                }
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;
                AuditLog.log("Error in PayLike Accept(" + tid + ")\r\n: " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.PayLike, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PaymentId"></param>
        /// <param name="OrderNumber"></param>
        /// <param name="Amount"></param>
        /// <param name="qp_status_code"></param>
        /// <returns></returns> 
        public ReceiptModel QuickPay(int? paymentId = null, int? orderNumber = null, decimal? orderAmount = 0, int? qp_status_code = null)
        {
            using (var dataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories())))
            {
                //get order and user details from Order and user table
                using (var orderLocker = dataUnitOfWork.Orders.LockAndGet(q => q.Number == orderNumber.ToString() && q.Status == (int)Data.Enums.OrderStatus.Quoted))
                {
                    if (!orderLocker.Usable) throw new Exception($"Order is locked {orderLocker.Domain.Number}");
                    var order = orderLocker.Domain;
                    // TODO: select fields
                    var user = dataUnitOfWork.Users.GetById(order.UserId);
                    var lang = CultureInfo.CurrentCulture.Name;
                    if (order.Amount == orderAmount && order.ExtRef == paymentId.ToString())
                    {
                        QuickPayPaymentModel quickPaymodel = new QuickPayPaymentModel();
                        string strPaymentId = paymentId.ToString();
                        try
                        {

                            var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                            var hostname = HttpContext.Request.Url.Host;
                            var currentSite = dataUnitOfWork.Sites.Get(q => q.Url == hostname).FirstOrDefault();
                            var quickPayService = new QuickPayService(currentSite.Id);
                            var cryptoCurrency = "";
                            var getPayment = quickPayService.GetPayment(strPaymentId);
                            QuickPayResponse getPaymentContent = JsonConvert.DeserializeObject<QuickPayResponse>(getPayment.Content);
                            bool? paymentAccepted = getPaymentContent.Accepted;

                            var authorizedStatusCodes = getPaymentContent.Operations.Where(x => x.TypeMatch(QuickPayResponseConsts.Authorize)).Select(x => new { x.QpStatusCode, x.AqStatusCode }).LastOrDefault();

                            if (paymentAccepted.GetValueOrDefault() && authorizedStatusCodes.QpStatusCode == QuickPayStatusCodes.Success && authorizedStatusCodes.AqStatusCode == QuickPayStatusCodes.Success)
                            {
                                var currency = dataUnitOfWork.Currencies.GetById(order.CurrencyId); //get currency details from Currency table
                                cryptoCurrency = dataUnitOfWork.Currencies.Get(curr => curr.CurrencyTypeId == (int)Data.Enums.CurrencyTypes.Digital && curr.IsActive == true && curr.Id == order.CryptoCurrencyId).FirstOrDefault().Code;
                                //generate new Guid as creditCardUserIdentity
                                var creditCardUserIdentity = Guid.NewGuid();
                                order.CreditCardUserIdentity = creditCardUserIdentity;
                                long minorUnits = Convert.ToInt64(Math.Pow(10, currency.MinorUnits));
                                //if transaction amount of QuickPay and order amount from Order table doesnot match then change order status to 19
                                if (getPaymentContent.Link.Amount != order.Amount * minorUnits)
                                {
                                    order.Status = (int)Data.Enums.OrderStatus.PaymentAborted;
                                    dataUnitOfWork.Commit();
                                    AuditLog.log("Error in validating payment process for order with phone number " + user.Phone + ".\r\nQuickPay details for this transaction:\r\n" +
                                    JsonConvert.SerializeObject(getPayment), (int)Data.Enums.AuditLogStatus.PaymentError, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ErrorinProcess");
                                    result.AddErrorKey(errorMesaage, lang);
                                    return null;
                                }
                                //if transaction OrderNumber of QuickPay and OrderNumber from Order table doesnot match then change order status to 19
                                if (getPaymentContent.OrderId != order.Number)
                                {
                                    order.Status = (int)Data.Enums.OrderStatus.PaymentAborted;
                                    dataUnitOfWork.Commit();
                                    AuditLog.log("Error in validating payment process for order with phone number " + user.Phone + ".\r\nQuickPay details for this transaction:\r\n" +
                                    JsonConvert.SerializeObject(getPayment), (int)Data.Enums.AuditLogStatus.PaymentError, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ErrorinProcess");
                                    result.AddErrorKey(errorMesaage, lang);
                                    return null;
                                }
                                //if transaction PaymentValidationKey is equal to empty Guid then change order status to 19
                                if (order.CreditCardUserIdentity == Guid.Empty)
                                {
                                    order.Status = (int)Data.Enums.OrderStatus.PaymentAborted;
                                    dataUnitOfWork.Commit();
                                    AuditLog.log("Error in validating payment process for order with phone number " + user.Phone + ".\r\nQuickPay details for this transaction:\r\n" +
                                    JsonConvert.SerializeObject(getPayment), (int)Data.Enums.AuditLogStatus.PaymentError, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ErrorinProcess");
                                    result.AddErrorKey(errorMesaage, lang);
                                    return null;
                                }

                                //userSessionModel.PaymentValidationKey = Guid.Empty;
                                var cultureInfo = dataUnitOfWork.Countries.GetCultureCodeByCurrency(order.CurrencyCode);
                                var ci = new CultureInfo(cultureInfo);
                                order.CreditCardUserIdentity = Guid.NewGuid();

                                if (order.Type == (int)Sell)
                                    order.Status = (int)Data.Enums.OrderStatus.Paying;
                                else
                                {
                                    var KycRequirement = dataUnitOfWork.KycFiles.Get(x => x.UserId == order.UserId).FirstOrDefault();
                                    //if KycRequirement is NONE then change order status to Sending else change status to Paid
                                    if (KycRequirement == null)
                                        order.Status = (int)Data.Enums.OrderStatus.Sending;
                                    else
                                        order.Status = (int)Data.Enums.OrderStatus.Paid;
                                }
                                string uuid = CommonUUID.UUID();
                                //assign values to Transaction model property
                                var transaction = new Transaction
                                {
                                    Amount = order.Amount,
                                    MethodId = 1,
                                    Type = 1,
                                    ExtRef = strPaymentId,
                                    Currency = order.CurrencyId,
                                    FromAccount = Logic.Accounting.AccountingUtil.GetAccountId(order.Type, 0, AccountValueFor.FromAccount, 1, ParticularType.NonFee),
                                    ToAccount = Logic.Accounting.AccountingUtil.GetAccountId(order.Type, 0, AccountValueFor.ToAccount, 1, ParticularType.NonFee),
                                    Info = getPaymentContent.Metadata.Bin + "XXXXXX" + getPaymentContent.Metadata.Last4 + "," + user.Fname,
                                    BatchNumber = CommonUUID.UUID()
                                };

                                order.CardNumber = getPaymentContent.Metadata.Bin + "XXXXXX" + getPaymentContent.Metadata.Last4;
                                order.ExtRef = strPaymentId;
                                order.Transactions.Add(transaction);
                                dataUnitOfWork.Commit();

                                AuditLog.log("Updating order by user with phone number " + user.Phone + ".\r\nOrder details :\r\n" +
                                    JsonSerializerEx.SerializeObject(order, 1), (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                AuditLog.log("Payment made by user with phone number " + user.Phone + ".\r\nQuickPay details for this transaction:\r\n" +
                                    JsonConvert.SerializeObject(getPaymentContent), (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                var site = dataUnitOfWork.Sites.GetById(order.SiteId.Value);
                                //send email to user email address
                                EmailHelper.SendEmail(user.Email, "PaymentReceived", new Dictionary<string, object> {
                                { "UserFirstName", user.Fname },
                                { "OrderAmount", order.Amount.Value.ToString("N2", ci) },
                                { "OrderCurrency", order.CurrencyCode},
                                { "OrderCommission", (order.CommissionProcent > 0 ) ? (order.Amount / 100 * order.CommissionProcent).Value.ToString("N2", ci): "0" },
                                { "CardFee",  order.CommissionProcent.HasValue ? order.CommissionProcent.Value.ToString("N2", ci) + "%" : "0" },
                                { "OurFee",  order.OurFee.Value.ToString("N2", ci) + "%" },
                                { "OurFeeValue", (order.Amount / 100 * order.OurFee * dataUnitOfWork.Orders.GetOrderDiscount(order.Id)).Value.ToString("N2", ci) },
                                { "CryptoAddress", order.CryptoAddress },
                                { "CardName", user.Fname },
                                { "CardNumber", getPaymentContent.Metadata.Bin + "XXXXXX" + getPaymentContent.Metadata.Last4 },
                                { "OrderNumber", order.Number },
                                { "MinersFee", (order.MinersFee.HasValue ? order.MinersFee.Value.ToString() : "0") + " BTC"}
                            }, site.Text, order.BccAddress);
                                var testOrProd = "";
                                //Appending "TEST:" to Pushover if payment type is Credit card
                                if (order.PaymentType == 1)
                                    if (!currentSite.Text.Contains("localhost"))
                                        if (currentSite.Text.Split('.')[0].ToLower() == "apptest")
                                            testOrProd = "TEST: ";
                                if (!currentSite.Text.Contains("localhost"))
                                    PushoverHelper.SendNotification(testOrProd + (currentSite.Text.Contains("localhost") ? currentSite.Text : currentSite.Text.Split('.')[1]), order.Number + " PAYMENT: " + order.Amount.Value.ToString("N0", new CultureInfo("da-DK")) + " " + currency.Code + ", " + order.Name + ", " + order.CommissionProcent * dataUnitOfWork.Orders.GetOrderDiscount(order.Id) + " %");
                                var OrderStatus = DataUnitOfWork.OrderStatus.Get(x => x.Id == order.Status).Select(q => q.Text).FirstOrDefault();
                                SetSiteNameViewBag(hostname);
                                return new ReceiptModel(cultureInfo, order.Type)
                                {
                                    Amount = (order.Type == (int)Sell ? order.BTCAmount.Value : order.Amount.Value),
                                    BitcoinAddress = order.CryptoAddress,
                                    CardHolderName = order.Name,
                                    Commission = order.CommissionProcent.HasValue ? (order.Amount.GetValueOrDefault() / 100 * order.CommissionProcent.GetValueOrDefault()) : 0,
                                    OurFeeValue = order.OurFee.HasValue ? (order.Amount / 100 * order.OurFee.GetValueOrDefault() * dataUnitOfWork.Orders.GetOrderDiscount(order.Id)).GetValueOrDefault() : 0,
                                    OurFee = order.OurFee.GetValueOrDefault(),
                                    CardFee = order.CommissionProcent.GetValueOrDefault(),
                                    CreditCardNumber = order.CardNumber,
                                    Currency = (order.Type == (int)Sell ? dataUnitOfWork.Currencies.GetById(order.CryptoCurrencyId).Code : order.CurrencyCode),
                                    OrderNumber = order.Number,
                                    TransactionId = order.ExtRef,
                                    TransactionHash = order.TransactionHash,
                                    Rate = order.Rate.GetValueOrDefault(),
                                    CommissionPercent = order.CommissionProcent.ToString(),
                                    GoogleTagManagerId = dataUnitOfWork.Sites.GetById(currentSite.Id).GoogleTagManagerId,
                                    MinerFee = order.MinersFee.GetValueOrDefault(),
                                    Country = order.CountryCode,
                                    Coupon = order.Coupon,
                                    Type = order.Type,
                                    paymentMethod = dataUnitOfWork.PaymentTypes.GetById(order.PaymentType).Text,
                                    ReferenceId = order.ReferenceId,
                                    OrderStatus = OrderStatus,
                                    OrderDate = order.Quoted,
                                    OrderId = order.Id
                                };
                            }
                            else //if there is any error in transaction change order status to 19
                            {
                                order.Status = (int)Data.Enums.OrderStatus.PaymentAborted;
                                dataUnitOfWork.Commit();
                                AuditLog.log("Error in validating payment process for order with phone number " + user.Phone + ".\r\nQuickPay details for this transaction:\r\n" +
                                    JsonConvert.SerializeObject(getPayment), (int)Data.Enums.AuditLogStatus.PaymentError, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ValidateinPaymentProcess");
                                result.AddErrorKey(errorMesaage, lang);
                                return null;
                            }
                        }
                        catch (Exception ex)
                        {
                            order.Status = (int)Data.Enums.OrderStatus.PaymentAborted;
                            dataUnitOfWork.Commit();
                            AuditLog.log("Error in QuickPay Accept(" + strPaymentId + ")\r\n: " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Error);
                            var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                            var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ErrorValidateinPaymentProcess");
                            result.AddErrorKey(errorMesaage, lang);
                            throw ex;
                        }
                    }
                    else
                    {
                        order.Status = (int)Data.Enums.OrderStatus.PaymentAborted;
                        dataUnitOfWork.Commit();
                        AuditLog.log("Error in validating payment process for order with phone number " + user.Phone + ".\r\nQuickPay details for this transaction:\r\n", (int)Data.Enums.AuditLogStatus.PaymentError, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                        return null;
                    }
                }
            }
        }

        public ActionResult TrustPayAccept(string id)
        {
            try
            {
                var hostname = HttpContext.Request.Url.Host;
                var currentSite = DataUnitOfWork.Sites.Get(q => q.Url == hostname).FirstOrDefault();

                var appSettings = DataUnitOfWork.AppSettings.GetAll();
                //dynamic payLikeDetails = JsonConvert.DeserializeObject(appSettings.FirstOrDefault(q => q.ConfigKey == "PayLikeDetails").ConfigValue); //test
                var userSessionModel = ViewBag.UserSessionModel as UserSessionModel;
                TrustPayService payService = new TrustPayService();
                var currency = DataUnitOfWork.Currencies.GetById(userSessionModel.CurrencyId);
                var checkoutId = payService.PaymentStatus(id, currentSite.Id);
                var order = DataUnitOfWork.Orders.Get(q => q.Id == userSessionModel.OrderId).FirstOrDefault(); //get order details
                order.ExtRef = id;
                DataUnitOfWork.Commit();

                userSessionModel.PaymentResponse = "Receipt";
                return View("YourPayRedirect");
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;

                AuditLog.log("Error in TrustPay Accept(" + id + ")\r\n: " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.PayLike, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ErrorValidateinPaymentProcess");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        /// <summary>
        /// method provides payment procedure for YourPay service
        /// </summary>
        /// <param name="uxtime"></param>
        /// <param name="merchantNumber"></param>
        /// <param name="tid"></param>
        /// <param name="tchecksum"></param>
        /// <param name="checksum"></param>
        /// <param name="orderid"></param>
        /// <param name="shopPlatform"></param>
        /// <param name="amount"></param>
        /// <param name="split"></param>
        /// <param name="date"></param>
        /// <param name="cvc"></param>
        /// <param name="expmonth"></param>
        /// <param name="expyear"></param>
        /// <param name="tcardno"></param>
        /// <param name="time"></param>
        /// <param name="cardid"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        [AuthorizeUser]
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult Accept(string uxtime, string merchantNumber, int tid, string tchecksum, string checksum, string orderid, string shopPlatform, decimal amount, string split, int date, string cvc, string expmonth, string expyear, string tcardno, string time, string cardid, string currency)
        {
            var nullCheckerResult = "";
            var lang = CultureInfo.CurrentCulture.Name;

            try
            {
                var hostname = HttpContext.Request.Url.Host;
                var currentSite = DataUnitOfWork.Sites.Get(q => q.Url == hostname).FirstOrDefault();
                var appSettings = DataUnitOfWork.AppSettings.GetAll();
                var yourPayDetails = YourpayService.GetYourPayDetails(); //get your Pay details
                if (SHA1HashStringForUTF8String(tid + yourPayDetails.IntegrationCode) == tchecksum)
                {
                    var order = DataUnitOfWork.Orders.Get(q => q.Number == orderid.ToString()).FirstOrDefault(); //get order details
                    var userSessionModel = ViewBag.UserSessionModel as UserSessionModel;
                    var userCurrency = DataUnitOfWork.Currencies.Get(q => q.Id == order.CurrencyId).FirstOrDefault();
                    var cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCurrency(userCurrency.Code);
                    var ci = new CultureInfo(cultureInfo); //get culture info
                    var user = DataUnitOfWork.Users.GetById(order.UserId); //get user details
                    if (order.Type == (int)Sell)
                        order.Status = (int)Data.Enums.OrderStatus.Paying;
                    else
                    {
                        order.CreditCardUserIdentity = Guid.NewGuid();
                        if (userSessionModel.KycRequirement == "NONE") //if KycRequirement is NONE change order status to Sending else change to Paid
                            order.Status = (int)Data.Enums.OrderStatus.Sending;
                        else
                            order.Status = (int)Data.Enums.OrderStatus.Paid;
                        //TODO later for credit cards
                        var transaction = new Transaction
                        {
                            Amount = order.Amount,
                            MethodId = 1,
                            Type = 1,
                            ExtRef = tid.ToString(),
                            Currency = order.CurrencyId,
                            FromAccount = 1,
                            ToAccount = 8,
                            //Completed = DateTime.Now,
                            Info = tcardno + "," + order.Name
                        };
                        order.Transactions.Add(transaction);
                        AuditLog.log("Updated Order(" + order.Id + ") transaction " + JsonSerializerEx.SerializeObject(transaction, 1) + " while Processiong Accept().", (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                    }
                    var currencyDetails = DataUnitOfWork.Currencies.Get(q => q.Code.ToLower().Trim() == currency.ToLower().Trim() || q.YourPayCurrencyCode.ToLower().Trim() == currency.ToLower().Trim()).FirstOrDefault();
                    //assign values to Transaction model property

                    order.CurrencyCode = currencyDetails.Code;
                    order.CardNumber = tcardno;
                    order.ExtRef = tid.ToString();
                    DataUnitOfWork.Commit();

                    AuditLog.log("Updating order by user with phone number " + userSessionModel.PhoneNumber + ".\r\nOrder details :\r\n" +
                                            JsonSerializerEx.SerializeObject(order, 1), (int)Data.Enums.AuditLogStatus.PayLike, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                    DataUnitOfWork.AuditTrails.Add(new AuditTrail
                    {
                        Message = "Payment made by user with phone number " + user.Phone + ".\r\nYourPay details for this transaction:\r\n" +
                        "uxtime=" + uxtime + "\r\n" +
                        "merchantNumber=" + merchantNumber + "\r\n" +
                        "tid=" + tid + "\r\n" +
                        "tchecksum=" + tchecksum + "\r\n" +
                        "checksum=" + checksum + "\r\n" +
                        "orderid=" + orderid + "\r\n" +
                        "shopPlatform=" + shopPlatform + "\r\n" +
                        "amount=" + amount + "\r\n" +
                        "split=" + split + "\r\n" +
                        "date=" + date + "\r\n" +
                        "cvc=" + cvc + "\r\n" +
                        "expmonth=" + expmonth + "\r\n" +
                        "expyear=" + expyear + "\r\n" +
                        "tcardno=" + tcardno + "\r\n" +
                        "time=" + time + "\r\n" +
                        "cardid=" + cardid + "\r\n" +
                        "currency=" + currency + "\r\n",
                        Status = 2,
                        Created = DateTime.Now,
                        Order = order
                    });

                    var site = DataUnitOfWork.Sites.GetById(order.SiteId.Value);
                    var minersFee = (order.MinersFee.HasValue ? order.MinersFee.Value.ToString() : "0");

                    nullCheckerResult = NullChecker.GetResult(() => { return user.Email; },
                    () => { return userSessionModel.FullName; },
                    () => { return userSessionModel.OrderAmount.ToString("N2", ci); },
                    () => { return currencyDetails.Code; },
                    () => { return (userSessionModel.OrderAmount / 100 * userSessionModel.CommissionPercent * DataUnitOfWork.Orders.GetOrderDiscount(userSessionModel.OrderId)).ToString("N2", ci); },
                    () => { return userSessionModel.CryptoAddress; },
                    () => { return userSessionModel.FullName; },
                    () => { return tcardno; },
                    () => { return orderid; },
                    () => { return minersFee; },
                    () => { return site.Text; },
                    () => { return order.BccAddress; });

                    if (order.Type == (int)Buy)
                    {
                        //send mail to user id
                        EmailHelper.SendEmail(user.Email, "PaymentReceived", new Dictionary<string, object> {
                            { "UserFirstName", userSessionModel.FullName },
                            { "OrderAmount", userSessionModel.OrderAmount.ToString("N2", ci) },
                            { "OrderCurrency", currencyDetails.Code },
                            { "OrderCommission", (userSessionModel.OrderAmount / 100 * userSessionModel.CommissionPercent*DataUnitOfWork.Orders.GetOrderDiscount(userSessionModel.OrderId)).ToString("N2", ci) },
                            { "CryptoAddress", userSessionModel.CryptoAddress },
                            { "CardName", userSessionModel.FullName },
                            { "CardNumber", tcardno },
                            { "OrderNumber", orderid },
                            { "MinersFee", minersFee + " BTC"}
                        }, site.Text, order.BccAddress);

                        var testOrProd = "";
                        //Appending "TEST:" to Pushover if payment type is Credit card
                        if (order.PaymentType == 1)
                            if (!currentSite.Text.Contains("localhost"))
                                if (currentSite.Text.Split('.')[0].ToLower() == "apptest")
                                    testOrProd = "TEST: ";
                        PushoverHelper.SendNotification(testOrProd + (currentSite.Text.Contains("localhost") ? currentSite.Text : currentSite.Text.Split('.')[1]), order.Number + " PAYMENT: " + order.Amount.Value.ToString("N0", new CultureInfo("da-DK")) + " " + currencyDetails.Code + ", " + order.Name + ", " + order.CommissionProcent * DataUnitOfWork.Orders.GetOrderDiscount(userSessionModel.OrderId) + " %");
                    }

                    ViewBag.GoogleTagManagerId = DataUnitOfWork.Sites.GetById(currentSite.Id).GoogleTagManagerId;
                    //redirect to Receipt action method

                    if (order.Type == (int)Sell)
                    {
                        if (new TrustLogic().IsCardUS(tcardno))
                        {
                            userSessionModel.PaymentResponse = "ErrorUSCard";
                            order.Status = (int)Data.Enums.OrderStatus.Cancel;
                            DataUnitOfWork.Commit();
                        }
                        else
                            userSessionModel.PaymentResponse = "CryptoCurrencyPayment";
                    }
                    else
                        userSessionModel.PaymentResponse = "Receipt";
                    return View("YourPayRedirect");
                }
                else //change order status to 0
                {
                    var order = DataUnitOfWork.Orders.Get(q => q.Number == orderid.ToString()).FirstOrDefault();
                    order.Status = (int)Data.Enums.OrderStatus.Cancel;
                    DataUnitOfWork.Commit();

                    AuditLog.log("Error in validating payment process, rejected by credit card processor", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ValidateinPaymentProcess");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in Accept(" + uxtime + "," + merchantNumber + "," + tid + "," + tchecksum + "," + checksum + "," + orderid + "," + shopPlatform + "," + amount
                    + "," + split + "," + date + "," + cvc + "," + expmonth + "," + expyear + "," + tcardno + "," + time + "," + cardid + "," + currency + ")\r\n:"
                    + ex.ToMessageAndCompleteStacktrace() + "\r\n" + nullCheckerResult, (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ErrorValidateinPaymentProcess");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        [AuthorizeUser]
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult CryptoCurrencyPaymentInfo()
        {
            try
            {
                var userSessionModel = ViewBag.UserSessionModel as UserSessionModel;
                var order = DataUnitOfWork.Orders.GetById(userSessionModel.OrderId); //get order details from order table
                var userCurrency = DataUnitOfWork.Currencies.Get(q => q.Id == order.CurrencyId).FirstOrDefault();
                var cryptoCurrency = DataUnitOfWork.Currencies.Get(curr => curr.CurrencyTypeId == (int)Data.Enums.CurrencyTypes.Digital && curr.IsActive == true && curr.Id == order.CryptoCurrencyId).FirstOrDefault().Code;
                var cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCurrency(userCurrency.Code);
                var ci = new CultureInfo(cultureInfo); //get culture info
                SetSiteNameViewBag(HttpContext.Request.Url.Host);
                switch (userSessionModel.PaymentResponse)
                {
                    case "Receipt":
                        {
                            var hostname = HttpContext.Request.Url.Host;
                            var currentSite = DataUnitOfWork.Sites.Get(q => q.Url == hostname).FirstOrDefault();
                            var euroRates = OpenExchangeRates.GetLatestExchangeRates().Rates;
                            var euroCurrencyRate = OpenExchangeRates.GetEURExchangeRate(order.CurrencyCode, euroRates);

                            string IBANstr = string.Empty;
                            var bankPaymentSettings = new PurchaseLogic(DataUnitOfWork).GetBankPaymentDetailsForSell(order.CurrencyCode);
                            switch (bankPaymentSettings.Value1LabelResourceName)
                            {
                                case "Bank_BICORSWIFT":
                                    bankPaymentSettings.Value1 = order.SwiftBIC;
                                    break;
                                case "Bank_DKK_RegNumber":
                                case "Bank_UKSortCode":
                                case "Bank_BSBCode":
                                default:
                                    bankPaymentSettings.Value1 = order.Reg;
                                    break;
                            }


                            switch (bankPaymentSettings.Value2LabelResourceName)
                            {
                                case "Bank_IBAN":
                                    bankPaymentSettings.Value2 = order.IBAN;
                                    break;
                                case "Bank_AccountNumber":
                                default:
                                    bankPaymentSettings.Value2 = order.AccountNumber;
                                    break;
                            }

                            IBANstr = string.Format("{0}: {1}, ", WebUtility.HtmlEncode(Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", bankPaymentSettings.Value1LabelResourceName)), WebUtility.HtmlEncode(bankPaymentSettings.Value1));
                            IBANstr += string.Format("{0}: {1}", WebUtility.HtmlEncode(Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", bankPaymentSettings.Value2LabelResourceName)), WebUtility.HtmlEncode(bankPaymentSettings.Value2));

                            ReceiptModel receiptModel = new ReceiptModel(userSessionModel.CultureInfo, order.Type);
                            receiptModel.Amount = order.Type == (int)Sell ? order.BTCAmount.GetValueOrDefault() : order.Amount.GetValueOrDefault();
                            receiptModel.PayoutAmountStr = ((order.Amount.GetValueOrDefault() - (order.Amount.GetValueOrDefault() - order.FixedFee.GetValueOrDefault()
                                * euroCurrencyRate) / 100 * order.CommissionProcent.GetValueOrDefault())
                                - order.Amount.GetValueOrDefault() / 100 * order.OurFee.GetValueOrDefault() * DataUnitOfWork.Orders.GetOrderDiscount(userSessionModel.OrderId)
                                - order.FixedFee.GetValueOrDefault() * euroCurrencyRate).ToString("N2", ci);
                            receiptModel.PayoutAmount = ((order.Amount.GetValueOrDefault() - (order.Amount.GetValueOrDefault() - order.FixedFee.GetValueOrDefault()
                                * euroCurrencyRate) / 100 * order.CommissionProcent.GetValueOrDefault())
                                - order.Amount.GetValueOrDefault() / 100 * order.OurFee.GetValueOrDefault() * DataUnitOfWork.Orders.GetOrderDiscount(userSessionModel.OrderId)
                                - order.FixedFee.GetValueOrDefault() * euroCurrencyRate);
                            receiptModel.BitcoinAddress = order.CryptoAddress;
                            receiptModel.CardHolderName = order.Name;
                            receiptModel.Commission = (order.Amount.GetValueOrDefault() - order.FixedFee.GetValueOrDefault() * euroCurrencyRate) / 100 * order.CommissionProcent.GetValueOrDefault();
                            receiptModel.FixedFee = (order.FixedFee.GetValueOrDefault() * euroCurrencyRate);
                            receiptModel.OurFee = (order.Amount.GetValueOrDefault() / 100 * order.OurFee.GetValueOrDefault() * DataUnitOfWork.Orders.GetOrderDiscount(userSessionModel.OrderId));
                            receiptModel.CreditCardNumber = order.CardNumber;
                            receiptModel.IBAN = IBANstr;
                            receiptModel.SellCurrency = order.CurrencyCode;
                            receiptModel.CryptoCurrencyCode = cryptoCurrency;
                            receiptModel.Currency = (order.Type == (int)Sell ? cryptoCurrency : order.CurrencyCode); //
                            receiptModel.OrderNumber = order.Number;
                            receiptModel.TransactionId = order.ExtRef;
                            receiptModel.TransactionHash = order.TransactionHash;
                            receiptModel.RateStr = order.Rate.GetValueOrDefault().ToString("N2", ci);
                            receiptModel.Rate = order.Rate.GetValueOrDefault();
                            receiptModel.CommissionPercent = userSessionModel.CommissionPercent.ToString();
                            receiptModel.GoogleTagManagerId = DataUnitOfWork.Sites.GetById(currentSite.Id).GoogleTagManagerId;
                            var site = DataUnitOfWork.Sites.GetById(order.SiteId.Value);
                            var customCss = string.Empty;
                            var strCss = @"<style type=""text/css"">";
                            if (receiptModel.OurFee == 0)
                            {

                                strCss += @".hideRow1";
                                strCss += @"{";
                                strCss += @"display: none ";
                                strCss += @"}";
                            }
                            if (receiptModel.FixedFee == 0)
                            {
                                strCss += @".hideRow2";
                                strCss += @"{";
                                strCss += @"display: none ";
                                strCss += @"}";

                            }
                            strCss += @"</style>";
                            customCss += strCss;
                            if (order.Type == (int)Sell)
                                EmailHelper.SendEmail(order.Email, ("SellOrderReceipt"),
                                    new Dictionary<string, object>
                                    {
                                    { "UserFirstName", order.Name },
                                    { "OrderNumber", order.Number },
                                    { "Amount", receiptModel.Amount },
                                    { "TransactionExtRef", order.TransactionHash },
                                    { "OrderCurrency", receiptModel.SellCurrency },
                                    { "OrderCommission", receiptModel.Commission},
                                    { "OrderOurFee", receiptModel.OurFee + ' ' + receiptModel.SellCurrency},
                                    { "FixedFee", receiptModel.FixedFee + ' ' + receiptModel.SellCurrency},
                                    { "PayoutAmount", receiptModel.PayoutAmountStr + ' ' + receiptModel.SellCurrency},
                                    { "PayoutDestination",receiptModel.IBAN },
                                    { "OrderRate", receiptModel.RateStr },
                                    { "TxAmount", order.BTCAmount.Value.ToString("N8", ci) },
                                    }, site.Text, order.BccAddress, null, customCss);

                            return View("Receipt", receiptModel);
                        }
                    case "CryptoCurrencyPayment":
                        {
                            var appSettings = DataUnitOfWork.AppSettings.Get();
                            BitGoAccessSettings bitGoAccessSettings = SettingsManager.GetDefault().Get("BitGoAccessCode", true).GetJsonData<BitGoAccessSettings>(); // JsonConvert.DeserializeObject<BitGoAccessSettings>(bitGoAccessCodeJSON);
                            var bitgoClient = new BitGoAccess(bitGoAccessSettings, cryptoCurrency);
                            var sessionJSON = bitgoClient.Session();
                            if (sessionJSON.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                            {
                                AuditLog.log("Error in CryptoCurrencyPaymentInfo(). BitGo session is Unauthorized.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                                return View("Error", new ErrorModel { ErrorText = "Error in processing the order.", ErrorDescription = "Please contact the site administrator." });
                            }
                            dynamic sessionObject = SimpleJson.DeserializeObject(sessionJSON.Content);
                            var bitgoCurrencySetting = DataUnitOfWork.Currencies.Get(cur => cur.Code == cryptoCurrency).FirstOrDefault().BitgoSettings;
                            dynamic bitgoSetting = JsonConvert.DeserializeObject(bitgoCurrencySetting);
                            DateTime dateTimeObj;
                            if (DateTime.TryParse(sessionObject.session.expires.ToString(), out dateTimeObj) && DateTime.Compare(dateTimeObj, DateTime.Now) < 0)
                                throw new Exception("BitGo session is locked.");
                            var wallet = bitgoClient.GetWallet(bitgoSetting.DefaultWalletId.ToString());
                            var cryptoAddress = bitgoClient.CreateWalletAddress(wallet.id);
                            order.CryptoAddress = cryptoAddress;
                            var site = DataUnitOfWork.Sites.GetById(order.SiteId.Value);
                            DataUnitOfWork.Commit();
                            var ciQR = new CultureInfo("en-US"); //get culture info

                            string qrCstring = string.Format("bitcoin:{0}?amount={1}", cryptoAddress, order.BTCAmount.Value.ToString("N8", ciQR));
                            string SigBase64 = QRCodeHelper.GenerateBase64QRCode(qrCstring);

                            EmailHelper.SendEmail(order.Email, "CryptoCurrencyPaymentInstructions", new Dictionary<string, object> {
                                { "UserFirstName", order.Name },
                                { "OrderAmount", order.BTCAmount.ToString() },
                                { "OrderNumber", order.Number },
                                { "OrderCurrency", order.CurrencyCode },
                                { "CryptoAddress", order.CryptoAddress },
                            }, site.Text, order.BccAddress);

                            return View("CryptoCurrencyPaymentInstruction", new CryptoCurrencyPaymentInstructionModel
                            {
                                Amount = order.BTCAmount.ToString(),
                                BitcoinAddress = order.CryptoAddress,
                                CryptoCurrencyCode = DataUnitOfWork.Currencies.Get(curr => curr.Id == order.CryptoCurrencyId && curr.CurrencyTypeId == (int)Data.Enums.CurrencyTypes.Digital).FirstOrDefault().Code,
                                QRCodeImage = "data:image/png;base64," + SigBase64,
                                OrderNumber = order.Id.ToString(),
                                GoogleTagManagerId = DataUnitOfWork.Sites.GetById(GetCurrentSite().Id).GoogleTagManagerId,
                                Commission = order.CommissionProcent.ToString(),
                                Currency = order.CurrencyCode
                            });
                        }
                    case "ErrorUSCard":
                        return View("DenyUsCard", new ReceiptModel(userSessionModel.CultureInfo, order.Type)
                        {
                            OrderNumber = order.Number.ToString(),
                            Amount = (order.Type == (int)Sell ? order.BTCAmount.GetValueOrDefault() : order.Amount.GetValueOrDefault()),
                            Currency = order.Currency.Code,
                            Commission = (order.Type == (int)Sell ?
                                            (order.BTCAmount.GetValueOrDefault() / 100 * order.CommissionProcent.GetValueOrDefault()) :
                                            (order.Amount.GetValueOrDefault() / 100 * order.CommissionProcent.GetValueOrDefault())),
                            OurFee = (order.Type == (int)Sell ?
                                            (order.BTCAmount.GetValueOrDefault() / 100 * order.OurFee.GetValueOrDefault() * DataUnitOfWork.Orders.GetOrderDiscount(userSessionModel.OrderId)) :
                                            (order.Amount.GetValueOrDefault() / 100 * order.OurFee.GetValueOrDefault() * DataUnitOfWork.Orders.GetOrderDiscount(userSessionModel.OrderId))),
                            GoogleTagManagerId = DataUnitOfWork.Sites.GetById(GetCurrentSite().Id).GoogleTagManagerId
                        });
                    default:
                        return View("Error", new ErrorModel { ErrorText = "Error in processing the order.", ErrorDescription = "Please contact the site administrator." });
                }
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;
                AuditLog.log("Error in CryptoCurrencyPaymentInfo():" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "ErrorValidateinPaymentProcess");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        /// <summary>
        /// method which returns Reciept page
        /// </summary>
        /// <param name="receiptModel"></param>
        /// <returns></returns>
        [AuthorizeUser]
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult Receipt(ReceiptModel receiptModel = null)
        {
            try
            {
                var hostname = Request.Url.Host;
                SetSiteNameViewBag(hostname);
                if (receiptModel == null)
                {
                    var userSessionModel = ViewBag.UserSessionModel as UserSessionModel;
                    // TODOL select mandatory
                    var order = DataUnitOfWork.Orders.Get(q => q.Id == userSessionModel.OrderId).FirstOrDefault(); //get order details from Order table

                    hostname = Request.Url.Host;
                    SetSiteNameViewBag(hostname);

                    var currentSite = GetCurrentSite();

                    // var ci = new CultureInfo(userSessionModel.CultureInfo);
                    receiptModel = new ReceiptModel(userSessionModel.CultureInfo, order.Type)
                    {
                        Amount = (order.Type == (int)Sell ? order.BTCAmount.GetValueOrDefault() : order.Amount.GetValueOrDefault()),
                        BitcoinAddress = order.CryptoAddress,
                        CardHolderName = order.Name,
                        Commission = (order.Type == (int)Sell ?
                                        (order.BTCAmount.GetValueOrDefault() / 100 * order.CommissionProcent.GetValueOrDefault()) :
                                        (order.Amount.GetValueOrDefault() / 100 * order.CommissionProcent.GetValueOrDefault())),
                        OurFee = (order.Type == (int)Sell ?
                                        (order.BTCAmount.GetValueOrDefault() / 100 * order.OurFee.GetValueOrDefault() * DataUnitOfWork.Orders.GetOrderDiscount(userSessionModel.OrderId)) :
                                        (order.Amount.GetValueOrDefault() / 100 * order.OurFee.GetValueOrDefault() * DataUnitOfWork.Orders.GetOrderDiscount(userSessionModel.OrderId))),
                        CreditCardNumber = order.CardNumber,
                        Currency = (order.Type == (int)Sell ? DataUnitOfWork.Currencies.Get(curr => curr.Id == order.CryptoCurrencyId && curr.CurrencyTypeId == (int)Data.Enums.CurrencyTypes.Digital).FirstOrDefault().Code : order.CurrencyCode),
                        CryptoCurrencyCode = DataUnitOfWork.Currencies.Get(curr => curr.Id == order.CryptoCurrencyId && curr.CurrencyTypeId == (int)Data.Enums.CurrencyTypes.Digital).FirstOrDefault().Code,
                        OrderNumber = order.Number.ToString(),
                        //TransactionId = tid.ToString(),
                        Rate = order.Rate.GetValueOrDefault(),
                        CommissionPercent = (userSessionModel.CommissionPercent * DataUnitOfWork.Orders.GetOrderDiscount(userSessionModel.OrderId)).ToString(),
                        GoogleTagManagerId = DataUnitOfWork.Sites.GetById(currentSite.Id).GoogleTagManagerId,
                    };
                }
                return View(receiptModel);
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;

                AuditLog.log("Error in Receipt(): \r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        public string CheckNewUserLimits(decimal buyAmount, long userCurrencyId, long userId)
        {
            string limitError = string.Empty;
            // TODO: select required fields 
            var user = DataUnitOfWork.Users.GetById(userId);
            var TransactionLimitsJSON = user.TransactionLimitsDetails;
            dynamic TransactionLimits = JsonConvert.DeserializeObject(TransactionLimitsJSON);
            var previousDuration = DateTime.Now.AddDays(-30);

            // TODO: select is must - only few fields are required
            //get previous orders
            var previousCompletedOrders = DataUnitOfWork.Orders.GetAll().Where(q => q.UserId == userId &&
                                                         (q.Status == (int)Data.Enums.OrderStatus.Paid || q.Status == (int)Data.Enums.OrderStatus.KYCApprovalPending ||
                                                          q.Status == (int)Data.Enums.OrderStatus.AMLApprovalPending || q.Status == (int)Data.Enums.OrderStatus.KYCApproved ||
                                                          q.Status == (int)Data.Enums.OrderStatus.AMLApproved || q.Status == (int)Data.Enums.OrderStatus.Sending ||
                                                          q.Status == (int)Data.Enums.OrderStatus.PayoutAwaitsApproval || q.Status == (int)Data.Enums.OrderStatus.Sent ||
                                                          q.Status == (int)Data.Enums.OrderStatus.ReleasingPayment || q.Status == (int)Data.Enums.OrderStatus.ReleasedPayment ||
                                                          q.Status == (int)Data.Enums.OrderStatus.Completed || q.Status == (int)Data.Enums.OrderStatus.PayoutApproved ||
                                                          q.Status == (int)Data.Enums.OrderStatus.ComplianceOfficerApproval || q.Status == (int)Data.Enums.OrderStatus.CustomerResponsePending) &&
                                                         q.Quoted > previousDuration).Select(x => new { x.Amount, x.Quoted, x.CurrencyId }).ToList();

            //Count the num of transaction per day 
            previousDuration = DateTime.Now.AddDays(-1);
            decimal euroCurrencyRate = 0m;
            var previousOrderAmount = 0M;
            if ((previousCompletedOrders.Where(q => q.Quoted > previousDuration).Count()) >= TransactionLimits.DayTransactionLimit.Value)
            {
                limitError = "DayTransactionLimitExceeded";
                goto LRET;
            }

            //check if the current txn is above  TransactionLimits.PerTransactionAmountLimit
            var currency = DataUnitOfWork.Currencies.GetById(userCurrencyId);
            var euroRates = HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>;
            euroCurrencyRate = OpenExchangeRates.GetEURExchangeRate(currency.Code, euroRates);
            if (buyAmount / euroCurrencyRate > TransactionLimits.PerTransactionAmountLimit.Value)
            {
                limitError = "PerTransactionAmountLimitExceeded";
                goto LRET;
            }


            ////check if the current txn is above DayTransactionAmountLimit
            var previous1dayCompletedOrders = previousCompletedOrders.Where(q => q.Quoted >= previousDuration).ToList();
            if (previous1dayCompletedOrders.Any())
                foreach (var previousOrder in previous1dayCompletedOrders)
                    previousOrderAmount += previousOrder.Amount.Value / OpenExchangeRates.GetEURExchangeRate(DataUnitOfWork.Currencies.GetById(previousOrder.CurrencyId).Code, HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>);

            if ((previousOrderAmount + (buyAmount / euroCurrencyRate)) > TransactionLimits.DayTransactionAmountLimit.Value)
            {
                limitError = "DayTransactionAmountLimitExceeded";
                goto LRET;
            }

            //check if the current txn is above MonthTransactionAmountLimit
            previousOrderAmount = 0M;
            if (previousCompletedOrders.Any())
                foreach (var previousOrder in previousCompletedOrders)
                    previousOrderAmount += previousOrder.Amount.Value / OpenExchangeRates.GetEURExchangeRate(DataUnitOfWork.Currencies.GetById(previousOrder.CurrencyId).Code, HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>);

            if ((previousOrderAmount + (buyAmount / euroCurrencyRate)) > TransactionLimits.MonthTransactionAmountLimit.Value)
            {
                limitError = "MonthTransactionAmountLimitExceeded";
                goto LRET;
            }

        LRET:
            if (!string.IsNullOrEmpty(limitError))
            {
                AuditLog.log($"CheckNewUserLimits :{limitError} user({user.Id}: {user.Phone}), Buy:{buyAmount}, EUR-r:{euroCurrencyRate}, total:{previousOrderAmount}, Buy/EUR-r:{buyAmount / euroCurrencyRate} limits: {TransactionLimitsJSON}",
                    (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Warn);
            }

            return limitError;
        }


        public string CheckNewUserCrdeitCardLimits(decimal buyAmount, long userCurrencyId, long userId)
        {
            var user = DataUnitOfWork.Users.GetById(userId);
            var CreditCardLimitsJSON = user.CreditCardLimitsDetails;
            dynamic CrediCardLimits = JsonConvert.DeserializeObject(CreditCardLimitsJSON);
            var previousDuration = DateTime.Now.AddDays(-30);

            var previousCompletedOrders = DataUnitOfWork.Orders.GetAll().Where(q => q.UserId == userId &&
                                                         q.PaymentType == (int)Data.Enums.OrderPaymentType.CreditCard &&
                                                         (q.Status == (int)Data.Enums.OrderStatus.Paid || q.Status == (int)Data.Enums.OrderStatus.KYCApprovalPending ||
                                                          q.Status == (int)Data.Enums.OrderStatus.AMLApprovalPending || q.Status == (int)Data.Enums.OrderStatus.KYCApproved ||
                                                          q.Status == (int)Data.Enums.OrderStatus.AMLApproved || q.Status == (int)Data.Enums.OrderStatus.Sending ||
                                                          q.Status == (int)Data.Enums.OrderStatus.PayoutAwaitsApproval || q.Status == (int)Data.Enums.OrderStatus.Sent ||
                                                          q.Status == (int)Data.Enums.OrderStatus.ReleasingPayment || q.Status == (int)Data.Enums.OrderStatus.ReleasedPayment ||
                                                          q.Status == (int)Data.Enums.OrderStatus.Completed || q.Status == (int)Data.Enums.OrderStatus.PayoutApproved ||
                                                          q.Status == (int)Data.Enums.OrderStatus.ComplianceOfficerApproval || q.Status == (int)Data.Enums.OrderStatus.CustomerResponsePending) &&
                                                         q.Quoted > previousDuration).Select(x => new { x.Amount, x.Quoted, x.CurrencyId }).ToList();

            //Count the num of transaction per day 
            previousDuration = DateTime.Now.AddDays(-1);

            //check if the current txn is above  CrediCardLimits.PerTransactionAmountLimit
            var currency = DataUnitOfWork.Currencies.GetById(userCurrencyId);
            var euroRates = HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>;
            var euroCurrencyRate = OpenExchangeRates.GetEURExchangeRate(currency.Code, euroRates);
            if (buyAmount / euroCurrencyRate > CrediCardLimits.PerTransactionAmountLimit.Value) return "PerTransactionAmountLimitExceeded";


            ////check if the current txn is above DayTransactionAmountLimit
            var previous1dayCompletedOrders = previousCompletedOrders.Where(q => q.Quoted >= previousDuration).ToList();
            var previousOrderAmount = 0M;
            if (previous1dayCompletedOrders.Any())
                foreach (var previousOrder in previous1dayCompletedOrders)
                    previousOrderAmount += previousOrder.Amount.Value / OpenExchangeRates.GetEURExchangeRate(DataUnitOfWork.Currencies.GetById(previousOrder.CurrencyId).Code, HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>);

            if ((previousOrderAmount + (buyAmount / euroCurrencyRate)) > CrediCardLimits.DayTransactionAmountLimit.Value) return "DayTransactionAmountLimitExceeded";

            //check if the current txn is above MonthTransactionAmountLimit
            previousOrderAmount = 0M;
            if (previousCompletedOrders.Any())
                foreach (var previousOrder in previousCompletedOrders)
                    previousOrderAmount += previousOrder.Amount.Value / OpenExchangeRates.GetEURExchangeRate(DataUnitOfWork.Currencies.GetById(previousOrder.CurrencyId).Code, HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>);

            if ((previousOrderAmount + (buyAmount / euroCurrencyRate)) > CrediCardLimits.MonthTransactionAmountLimit.Value) return "MonthTransactionAmountLimitExceeded";

            return "";
        }

        [HttpGet]
        public ActionResult GetSellBankValidationSetting(string currencyCode)
        {
            var bankPaymentDataConfigs = SettingsManager.GetDefault().Get("SellBankPaymentSettings").GetJsonData<SellPaymentConfig[]>();
            var bankPaymentDataConfig = bankPaymentDataConfigs.FirstOrDefault(x => x.Currency.Equals(currencyCode, StringComparison.InvariantCultureIgnoreCase));
            List<SellBankValidationSetting> sellBankValidationSettings;
            if (bankPaymentDataConfig != null)
                sellBankValidationSettings = bankPaymentDataConfig.ValidationSetting;
            else
                sellBankValidationSettings = new List<SellBankValidationSetting>();
            return (new BetterJsonResult { Data = sellBankValidationSettings, JsonRequestBehavior = JsonRequestBehavior.AllowGet });
        }

        [HttpPost]
        public ActionResult QuickPayAuditLog(string orderNumber, string status, object message = null, object data = null)
        {
            try
            {
                long auditTrailLevel = Convert.ToInt64(status);

                var orderId = DataUnitOfWork.Orders.Get(x => x.Number == orderNumber).Select(x => x.Id).FirstOrDefault();
                AuditLog.log("QuickPay message: " + JsonConvert.SerializeObject(message) + ",data:" + JsonConvert.SerializeObject(data), (int)Data.Enums.AuditLogStatus.QuickPay, auditTrailLevel, orderId);

                return new BetterJsonResult { Data = new { } };
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;

                AuditLog.log("Error in Quickpay Auditlog creation:\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        //[HttpPost]
        //public ActionResult KYCFaceTecLogs(string orderNumber, string status)
        //{
        //    try
        //    {
        //        long auditTrailLevel = Convert.ToInt64(status);

        //        var userId = DataUnitOfWork.Orders.Get(x => x.Number == orderNumber).Select(x => x.UserId).FirstOrDefault();
        //        var kycdata = 
        //        DataUnitOfWork.KycFiles.Add();
        //        AuditLog.log("QuickPay message: " + JsonConvert.SerializeObject(message) + ",data:" + JsonConvert.SerializeObject(data), (int)Data.Enums.AuditLogStatus.QuickPay, auditTrailLevel, orderId);

        //        return new BetterJsonResult { Data = new { } };
        //    }
        //    catch (Exception ex)
        //    {
        //        AuditLog.log("Error in Quickpay Auditlog creation:\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Error);
        //        var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //        result.AddError("An error has occured. Please contact the site administrator.");
        //        return result;
        //    }
        //}

        [AuthorizeUser]
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult PaymentInstructions(PaymentInstructionsModel paymentInstructionsModel)
        {
            return View(paymentInstructionsModel);
        }

        /// <summary>
        /// Method to verify Txsecret
        /// </summary>
        /// <param name="useridentity"></param>
        /// <param name="txSecret"></param>
        /// <returns></returns>
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public ActionResult TxSecretRequest(string useridentity, string txSecret)
        {
            try
            {
                var hostname = HttpContext.Request.Url.Host;
                SetSiteNameViewBag(hostname);
                //if user identity value is null show error page
                if (useridentity == null)
                {
                    AuditLog.log("Useridentity is null.", (int)Data.Enums.AuditLogStatus.TxSercret, (int)Data.Enums.AuditTrailLevel.Info);
                    return View("Error", new ErrorModel { ErrorText = "Error accepting TX Secret", ErrorDescription = "Useridentity is null." });
                }
                //get order details
                var order = DataUnitOfWork.Orders.GetOrderByUserIdentity(Guid.Parse(useridentity));

                if (!(order.Status == (int)Data.Enums.OrderStatus.ComplianceOfficerApproval ||
                        order.Status == (int)Data.Enums.OrderStatus.CustomerResponsePending))
                {
                    AuditLog.log("No customer response pending for order with user identity " + useridentity + ". Order status:" + (int)Data.Enums.OrderStatus.ComplianceOfficerApproval + ".", (int)Data.Enums.AuditLogStatus.TxSercret, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                    var result = new BetterJsonResult { ErrorCode = "NoCustomerResponsePendingForOrder", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    result.AddError(Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "NoCustomerResponsePendingForOrder"));
                    return result;
                }

                //if order is null show error page
                if (order == null)
                {
                    ViewBag.OrderIsNUll = "There is no order for user identity " + useridentity + ".";
                    AuditLog.log("For Useridentity " + useridentity + "order is null in TxSecretRequest().", (int)Data.Enums.AuditLogStatus.TxSercret, (int)Data.Enums.AuditTrailLevel.Info);
                    return View("Error", new ErrorModel { ErrorText = "Error accepting TX Secret", ErrorDescription = "There is no order for user identity " + useridentity + "." });
                }
                else
                {
                    //get currency code
                    var currency = DataUnitOfWork.Currencies.Get(q => q.Id == order.CurrencyId).Select(q => q.Code).FirstOrDefault();
                    //get culture info from countries
                    var cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCurrency(currency);
                    var ci = new CultureInfo(cultureInfo);
                    var txSecretAttempt = Convert.ToInt32(SettingsManager.GetDefault().Get("TxSecrectVerificationAttempts").Value);

                    //if txsecret is empty then redirect to CreditCardVerification page with order details
                    if (string.IsNullOrEmpty(txSecret))
                    {
                        bool verified;
                        if (order.Note == null)
                            verified = false;
                        else
                            verified = order.Note.Contains("Tx provided") ? true : false;
                        var message = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "SusseccfullVerifyModalContentText");
                        var currentSite = GetCurrentSite();
                        ViewBag.GoogleTagManagerId = DataUnitOfWork.Sites.GetById(currentSite.Id).GoogleTagManagerId;

                        var oldestOrderDate = DataUnitOfWork.Orders.GetOldestOrderQuotedDate(order.CardNumber);
                        return View("CreditCardVerification", new TxSecretModel
                        {
                            UserIdentity = useridentity,
                            Name = order.Name,
                            OrderNumber = order.Number,
                            OrderDate = oldestOrderDate.ToString("d", ci),
                            Amount = order.Amount.Value.ToString("N2", ci),
                            Currency = currency,
                            CardNumber = order.CardNumber,
                            TxSecreteAttempts = order.TxSecrectVerificationAttempts.HasValue ? order.TxSecrectVerificationAttempts.Value : 0,
                            TxSecreteAttemptsFromDB = txSecretAttempt,
                            IsVerified = verified,
                            Commission = (order.CommissionProcent).Value,
                            TxSecreteMessage = message
                        });
                    }
                    else
                    {
                        //get orderlist for current user 
                        var orderList = DataUnitOfWork.Orders.GetOrderByUserId(order.UserId);
                        //if the orderlist contains txsecret then show successful verification message
                        if (orderList.Any(q => q.TxSecret == txSecret))
                        {
                            string orderNote = order.Note;
                            if (!string.IsNullOrEmpty(orderNote))//If some note already exists then concat the new note
                            {
                                order.Note = orderNote + "| Tx provided";
                            }
                            else
                            {
                                order.Note = "Tx provided";
                            }
                            order.Status = (int)Data.Enums.OrderStatus.PayoutAwaitsApproval;
                            order.CardApproved = DateTime.Now;
                            DataUnitOfWork.Commit();
                            AuditLog.log("Order status changed to Payout Awaits Approval.", (int)Data.Enums.AuditLogStatus.TxSercret, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                            var message = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "SusseccfullVerifyModalContentText");
                            return new BetterJsonResult { Data = message, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        }
                        else
                        {
                            //if txsecret attempt from order table greater than or equal to txSecreteAttempt from appSetting table, show attempt exceed limit message
                            if (order.TxSecrectVerificationAttempts >= txSecretAttempt)
                            {
                                AuditLog.log("User " + useridentity + " exceeded the credit card verification attempt with txsecret " + txSecret + ".", (int)Data.Enums.AuditLogStatus.TxSercret, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                var result = new BetterJsonResult { ErrorCode = "TxAttemptExceedLimit", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                                result.AddError(Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "TxAttemptExceedLimit"));
                                return result;
                            }
                            else
                            {
                                //increase TxSecrectVerificationAttempts column value in order table by 1 and show Incorrect txsecret code error message                                                       
                                order.TxSecrectVerificationAttempts = order.TxSecrectVerificationAttempts.HasValue ? order.TxSecrectVerificationAttempts + 1 : 1;
                                DataUnitOfWork.Commit();
                                AuditLog.log("User " + useridentity + " entered incorrect Txsecret code " + txSecret + ".", (int)Data.Enums.AuditLogStatus.TxSercret, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                var result = new BetterJsonResult { ErrorCode = "IncorrectTxsecretCode", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                                result.AddError(Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "IncorrectTxsecretCode"));
                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var lang = CultureInfo.CurrentCulture.Name;
                AuditLog.log(string.Format("Error in TxSecretRequest({0}) for user identity({1}):\r\n{2}", txSecret, useridentity, ex.ToMessageAndCompleteStacktrace()), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "CancelOrder");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        /// <summary>
        /// Method to cancel order
        /// </summary>
        /// <param name="useridentity"></param>
        /// <returns></returns>
        public ActionResult CancelOrder(string useridentity)
        {
            var lang = CultureInfo.CurrentCulture.Name;
            try
            {
                if (string.IsNullOrEmpty(useridentity))
                {
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    AuditLog.log("User identity is Null in CancelOrder().", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    result.AddErrorKey("IdentityEmpty", lang);
                    return result;
                }
                //get order details
                var order = DataUnitOfWork.Orders.Get(q => q.CreditCardUserIdentity.ToString() == useridentity).FirstOrDefault();
                if (order == null)
                    throw new Exception("Unable to find order with CreditCardUserIdentity " + useridentity);
                order.Status = (int)Data.Enums.OrderStatus.Cancel; //change order status to 0
                DataUnitOfWork.Commit();
                AuditLog.log("Order status set to Cancel, Order #" + order.Number + " with CreditCardUserIdentity " + useridentity + ".", (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                return new BetterJsonResult { };
            }
            catch (Exception ex)
            {
                AuditLog.log(string.Format("Error in CancelOrder({0}):\r\n{1}", useridentity, ex.ToMessageAndCompleteStacktrace()), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "CancelOrder");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        /// <summary>
        /// method to verify phone number
        /// </summary>
        /// <param name="userIdentity"></param>
        /// <param name="verificationCode"></param>
        /// <returns></returns>
        public ActionResult ComplaintsVerifyPhoneNumber(string userIdentity, int verificationCode)
        {
            var lang = CultureInfo.CurrentCulture.Name;
            try
            {
                if (string.IsNullOrEmpty(userIdentity))
                {
                    AuditLog.log("User identity is null", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    result.AddError("IdentityNull");
                    return result;
                }
                var userID = DataUnitOfWork.Orders.Get(q => q.CreditCardUserIdentity.ToString() == userIdentity).Select(q => q.UserId).FirstOrDefault();
                var user = DataUnitOfWork.Users.Get(q => q.Id == userID).FirstOrDefault();
                //Check user entered verification code is correct
                if (user.PhoneVerificationCode == verificationCode)
                {
                    //if entered code is correct, assign null to phone verification code 
                    user.PhoneVerificationCode = null;
                    DataUnitOfWork.Commit();
                    AuditLog.log("Phone Verification Code set to Null for User " + user.Id, (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info);
                    return new BetterJsonResult { Data = new { } };
                }
                else
                {
                    var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    AuditLog.log("Phone Verification Code  is Incorrect for User " + user.Id, (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "VerificationCodeIncorrect");
                    result.AddErrorKey(errorMesaage, lang);
                    return result;
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in VerifyPhoneNumber(" + verificationCode + "):\r\n" + ex.ToMessageAndCompleteStacktrace(),
                    (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                var errorMesaage = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", "UnknownError");
                result.AddErrorKey(errorMesaage, lang);
                return result;
            }
        }

        [AuthorizeUser]
        [EnableThrottling(PerSecond = 2, PerMinute = 50)]
        public bool CheckIfPaid()
        {
            try
            {
                var userSessionModel = ViewBag.UserSessionModel as UserSessionModel;
                var order = DataUnitOfWork.Orders.GetById(userSessionModel.OrderId); //get order details from order table
                if (order.Status != (int)Data.Enums.OrderStatus.Quoted && order.Status != (int)Data.Enums.OrderStatus.Paying)
                {
                    userSessionModel.PaymentResponse = "Receipt";
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in CheckIfPaid():\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                return false;
            }
        }

        [HttpGet]
        public ActionResult GetSellMessageContent(decimal amount, string digitalCurrency, string sellCurrency)
        {
            String message = "";
            String messageResource = "";
            Currency digitalCurrencyObj = DataUnitOfWork.Currencies.GetAll().Where(currency => currency.Code == digitalCurrency).FirstOrDefault();
            BitGoCurrencySettings bitGoCurrencySettings = JsonConvert.DeserializeObject<BitGoCurrencySettings>(digitalCurrencyObj.BitgoSettings);
            decimal euroUsdRate = Convert.ToDecimal((HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>)["EUR"]);
            decimal sellCurrencyUsdRate = Convert.ToDecimal((HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>)[sellCurrency]);
            decimal amountInEur = amount * (euroUsdRate / sellCurrencyUsdRate);
            foreach (var item in bitGoCurrencySettings.SellMessageLangRes)
            {
                if (amountInEur >= item.MinAmountInEUR && amountInEur < item.MaxAmountInEUR)
                {
                    messageResource = item.LanguageResource;
                    break;
                }
            }
            message = ResourceExtensions.Resource(HttpContext, "WMCResources", messageResource);
            return (new BetterJsonResult { Data = new { message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet });
        }

        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public string IsAlive()
        {
            return "is alive";
        }

        public static bool Validate(string mainresponse, string privatekey)
        {
            try
            {
                // TODO: url from web.config/config?
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=" + privatekey + "&response=" + mainresponse);
                WebResponse response = req.GetResponse();
                using (StreamReader readStream = new StreamReader(response.GetResponseStream()))
                {
                    string jsonResponse = readStream.ReadToEnd();
                    JsonResponseObject jobj = JsonConvert.DeserializeObject<JsonResponseObject>(jsonResponse);
                    return jobj.success;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public class JsonResponseObject
        {
            public bool success { get; set; }
            [JsonProperty("error-codes")]
            public List<string> errorcodes { get; set; }
            public decimal score { get; set; }
        }
        #endregion

        #region Private Functions

        private void CreateUserSession(decimal buyAmount, string paymentMethod, string btcAddress, string fullname, string email, string phoneNumber, Currency userCurrency, string kycRequirement, decimal commission, decimal? ourFee, decimal latestBTCRate, long orderId, long type, string couponCode, decimal? fixedFee, Currency cryptoCurrency)
        {
            try
            {
                var authTicket = new FormsAuthenticationTicket(1, phoneNumber, DateTime.Now, DateTime.Now.AddMinutes(Session.Timeout), true, phoneNumber);
                var encKey = FormsAuthentication.Encrypt(authTicket);
                Response.Cookies.Add(new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, encKey));
                Session.Add(encKey, new UserSessionModel
                {
                    PhoneNumber = phoneNumber,
                    FullName = fullname,
                    Email = email,
                    IPAddress = GetLanIPAddress(),
                    CryptoAddress = btcAddress,
                    OrderAmount = buyAmount,
                    CurrencyId = userCurrency.Id,
                    CryptoCurrencyId = cryptoCurrency.Id,
                    PaymentMethod = paymentMethod,
                    CommissionPercent = commission,
                    OurFee = ourFee,
                    Btc2LocalCurrencyRate = latestBTCRate,
                    KycRequirement = kycRequirement,
                    PhoneNumberVerified = false,
                    CultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCurrency(userCurrency.Code),
                    OrderId = orderId,
                    Type = type,
                    CouponCode = couponCode,
                    CouponDiscount = ((couponCode != null) ? (DataUnitOfWork.Coupons.Get(q => q.CouponCode == couponCode).FirstOrDefault().Discount) : 0.0M),
                    FixedFee = fixedFee
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error in creating user session object.", ex);
            }
        }

        private long CreateNewOrder(decimal amount, string paymentMethod, string btcAddress, string fullname, string email, string bccAddress, string partnerId, string requsetObject, string referrer,
            long currentSiteId, long userCurrencyId, long userId, decimal commission, decimal? ourFee, string paymentGatewayType, string countryCode, long orderNumber, string ipinfo, decimal latestBTCRate,
            BitGoCurrencySettings minersFeeAppSettings, long type, string couponCode, decimal? fixedFee, decimal? discountAmount, string Reg, string IBAN, string SwiftCode, string AccountNumber, decimal fxMarkUp,
            decimal spread, long cryptoCurrencyId, string referenceId = "", string merchantCode = "")
        {
            var order = new Order();
            try
            {
                order.Amount = type == (int)Sell ? 0 : amount;
                order.BTCAmount = type == (int)Sell ? amount : 0;
                order.CommissionProcent = commission;
                order.OurFee = ourFee;
                order.TermsIsAgreed = DateTime.Now;
                order.CryptoAddress = btcAddress;
                order.Name = fullname;
                order.Email = email;
                order.IP = GetLanIPAddress();
                //CurrencyCode may not be needed in Currency is added
                order.CurrencyId = userCurrencyId;
                order.Status = (int)Data.Enums.OrderStatus.Quoted;
                order.SiteId = currentSiteId;
                order.Quoted = DateTime.Now;
                order.Type = type;
                order.UserId = userId;
                order.CountryCode = countryCode;
                order.Rate = latestBTCRate;
                order.QuoteSource = "Kraken";
                order.RequestInfo = requsetObject;
                order.Number = orderNumber.ToString();
                order.IpCode = ipinfo;
                order.Referrer = referrer;
                order.Origin = HttpContext.Request.Headers["origin"];
                order.MinersFee = minersFeeAppSettings.MinersFee.fee;
                order.BccAddress = bccAddress;
                order.PartnerId = partnerId;
                order.CouponId = (couponCode != null) ? DataUnitOfWork.Coupons.Get(q => q.CouponCode == couponCode).FirstOrDefault().Id : (long?)null;
                order.FixedFee = fixedFee.HasValue ? fixedFee : 0;
                order.Reg = Reg;
                order.IBAN = IBAN;
                order.SwiftBIC = SwiftCode;
                order.AccountNumber = AccountNumber;
                order.DiscountAmount = discountAmount;
                order.CurrencyCode = DataUnitOfWork.Currencies.GetById(userCurrencyId).Code;
                order.FxMarkUp = fxMarkUp;
                order.Spread = spread;
                order.CryptoCurrencyId = cryptoCurrencyId;
                order.ReferenceId = referenceId;
                order.MerchantCode = merchantCode;
                if (paymentMethod == "CreditCard")
                {
                    order.TxSecret = new Random().Next(1000, 9999).ToString();
                    order.PaymentType = 1; //Credit Card
                    order.PaymentGatewayType = paymentGatewayType; // set PaymentGatewayType
                }
                else
                {
                    order.PaymentType = 2; //Bank
                    if (order.Type == (int)Buy)
                    {
                        order.Transactions.Add(new Transaction
                        {
                            Amount = amount,
                            MethodId = 6, //Bank
                            Type = 1, //Incomming
                            Currency = userCurrencyId,
                            FromAccount = Logic.Accounting.AccountingUtil.GetAccountId(order.Type, order.CurrencyId, AccountValueFor.FromAccount, 1, ParticularType.NonFee),
                            ToAccount = Logic.Accounting.AccountingUtil.GetAccountId(order.Type, order.CurrencyId, AccountValueFor.ToAccount, 1, ParticularType.NonFee),
                            BatchNumber = CommonUUID.UUID()
                        });
                    }
                }
                DataUnitOfWork.Orders.Add(order); //add new order to Order table
                DataUnitOfWork.Commit();
                var returnUrl = Session["ReturnUrl"] as string;
                var referrerUrl = Session["ReferrerUrl"] as string;
                AuditLog.log(string.Format("Created new order({0}){1}{2} {3}is ", order.Id,
                    string.IsNullOrEmpty(returnUrl) ? "" : "  returnUrl(" + returnUrl + ")",
                    string.IsNullOrEmpty(referrerUrl) ? "" : "  refererUrl(" + referrerUrl + ")",
                    JsonSerializerEx.SerializeObject(order, 1)), (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in placing order.", ex);
            }
            return order.Id;
        }

        /// <summary>
        /// method to set sitename in view bag
        /// </summary>
        /// <param name="hostname"></param>
        private void SetSiteNameViewBag(string hostname)
        {
            if (hostname.Contains("localhost")) //for debugging
            {
                ViewBag.SiteName = hostname;
                ViewBag.fSiteName = hostname;
            }
            else
            {
                // test.app.hafniatrading.com
                ViewBag.SiteName = GetSiteName(hostname); // hostname.Split('.')[1];
                ViewBag.fSiteName = GetFSiteName(hostname); // hostname.Split('.')[1] + "." + hostname.Split('.')[2];
            }
        }

        private string GetSiteName(string hostname)
        {
            var hostNameParts = hostname.Split('.');
            return hostNameParts[hostNameParts.Length - 2];
        }

        private string GetFSiteName(string hostname)
        {
            var hostNameParts = hostname.Split('.');
            return hostNameParts[hostNameParts.Length - 2] + "." + hostNameParts[hostNameParts.Length - 1];
        }

        private void SetTCUrl()
        {
            var userLanguages = new List<string>();
            try
            {
                var language = "en";
                if (HttpContext.Request.UserLanguages != null)
                {
                    userLanguages = new List<string>(HttpContext.Request.UserLanguages);
                    if (userLanguages.Count > 0)
                        language = userLanguages[0].Split('-')[0];
                }

                var url = "/Sites/" + ViewBag.SiteName + "/TC/TC_" + language + "_" + ViewBag.SiteName + ".html";
                var urlPP = "/Sites/" + ViewBag.SiteName + "/PrivacyPolicy/PrivacyPolicy_" + language + "_" + ViewBag.SiteName + ".html";
                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(url)) && System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(urlPP)))
                {
                    ViewBag.TC_url = url;
                    ViewBag.PP_url = urlPP;
                }

                else
                {
                    // TODO: Default site?
                    ViewBag.TC_url = "/Sites/monni/TC/TC_en_monni.html";
                    ViewBag.PP_url = "/Sites/monni/PrivacyPolicy/PrivacyPolicy_en_monni.html";
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in setTCUrl(). Count of UserLanguages " + userLanguages.Count + ".\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        /// <summary>
        /// Get current site details
        /// </summary>
        /// <returns></returns>
        private Site GetCurrentSite()
        {
            try
            {
                var hostname = HttpContext.Request.Url.Host;
                var currentSite = DataUnitOfWork.Sites.Get(q => q.Url == hostname).FirstOrDefault();
                // TODO: doing multiple responsibility
                SetSiteNameViewBag(hostname);
                return currentSite;
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in getCurrentSite():\r\n" + ex.ToMessageAndCompleteStacktrace(),
                    (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                throw ex;
            }
        }

        private string GetKYCRequirement(User user, decimal buyAmount, string purchaseCurrency, decimal eurRate, bool doLog, string paymentMethod)
        {
            int tierLevel = 0;
            string kycRequirement = "NONE";
            try
            {
                //Calculate tier level
                decimal totalTransactionAmount = GetUserTier(user, buyAmount, eurRate, ref tierLevel);

                //get kyc file types from kyc files where kyc files are approved for user
                var kycFileTypes = DataUnitOfWork.KycFiles.GetAll().Where(q => q.UserId == user.Id && q.Approved.HasValue).Select(q => q.KycType.Text).ToList();
                var kycNotApprovedCount = DataUnitOfWork.KycFiles.GetAll().Where(q => q.UserId == user.Id && (q.KycType.Text == "ProofOfRecidency" || q.KycType.Text == "PhotoID")).Count(q => !q.Approved.HasValue);
                var kycNotApprovedCountForCC = DataUnitOfWork.KycFiles.GetAll().Where(q => q.UserId == user.Id && (q.KycType.Text == "SelfieID" || q.KycType.Text == "PhotoID")).Count(q => !q.Approved.HasValue);

                //get kyc file types which contains PhotoID
                var photoIDUploaded = kycFileTypes.Contains("PhotoID");
                var proofOfRecidencyUploaded = kycFileTypes.Contains("ProofOfRecidency"); //get kyc file types which contains ProofOfRecidency
                var selfieIDUploaded = kycFileTypes.Contains("SelfieID");//
                //CustomerTier customerTier = (CustomerTier)user.Tier;
                //set kyc requirement value based on tier level
                if (paymentMethod == "CreditCard")
                {
                    if (!photoIDUploaded)
                        kycRequirement = "PhotoID";
                    if (!selfieIDUploaded)
                        kycRequirement = "SelfieID";
                    if (!photoIDUploaded && !selfieIDUploaded)
                        kycRequirement = "PhotoID&SelfieID";
                    if (kycNotApprovedCountForCC > 0) //even though notApprovedcount is greater than 0 check the individual id's
                    //kycRequirement = "PhotoID&SelfieID";
                    {
                        if (!selfieIDUploaded)
                            kycRequirement = "SelfieID";
                        if (!photoIDUploaded)
                            kycRequirement = "PhotoID";
                        if (!photoIDUploaded && !selfieIDUploaded)
                            kycRequirement = "PhotoID&SelfieID";
                    }
                }
                else
                {
                    if (tierLevel == 1)
                    {
                        if (kycNotApprovedCount > 0)
                            tierLevel = 3;
                    }
                    if (tierLevel == 2)
                    {
                        if (!photoIDUploaded)
                            kycRequirement = "PhotoID";
                        if (kycNotApprovedCount > 0)
                        {
                            if (!proofOfRecidencyUploaded)
                                kycRequirement = "ProofOfRecidency";
                            if (!photoIDUploaded)
                                kycRequirement = "PhotoID";
                            if (!photoIDUploaded && !proofOfRecidencyUploaded)
                                kycRequirement = "PhotoID&ProofOfRecidency";
                        }
                    }
                    if (tierLevel == 3)
                    {
                        if (!photoIDUploaded && proofOfRecidencyUploaded)
                            kycRequirement = "PhotoID";
                        if (photoIDUploaded && !proofOfRecidencyUploaded)
                            kycRequirement = "ProofOfRecidency";
                        if (!photoIDUploaded && !proofOfRecidencyUploaded)
                            kycRequirement = "PhotoID&ProofOfRecidency";
                        if (kycNotApprovedCount > 0)
                        {
                            if (!proofOfRecidencyUploaded)
                                kycRequirement = "ProofOfRecidency";
                            if (!photoIDUploaded)
                                kycRequirement = "PhotoID";
                            if (!photoIDUploaded && !proofOfRecidencyUploaded)
                                kycRequirement = "PhotoID&ProofOfRecidency";
                        }
                    }
                }
                if (doLog)
                    AuditLog.log("Calculated GetKYCRequirement(" + user.Phone + "," + buyAmount + "," + purchaseCurrency + ")" +
                        //"\r\nPreviousOrders:" + string.Join(",", previousCompletedOrders.Select(q => q.Number).ToArray()) +
                        "\r\nTotalTransactionAmount:" + totalTransactionAmount +
                        "\r\nTierLevel:" + tierLevel +
                        "\r\nKycNotApprovedCount:" + kycNotApprovedCount +
                        "\r\nPhotoIDUploaded:" + photoIDUploaded +
                        "\r\nProofOfRecidencyUploaded:" + proofOfRecidencyUploaded +
                        "\r\nEurRate:" + eurRate +
                        "\r\nKycRequirement:" + kycRequirement,
                        (int)Data.Enums.AuditLogStatus.UserLogin, (int)Data.Enums.AuditTrailLevel.Info);
                return kycRequirement;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetKYCRequirement(" + user.Phone + "," + buyAmount + "," + purchaseCurrency + ").\r\n" + ex.ToMessageAndCompleteStacktrace());
            }
        }

        private decimal GetUserTier(User user, decimal buyAmount, decimal eurRate, ref int tierLevel)
        {
            Expression<Func<decimal, bool>> tierOne = (transactionAmount) => transactionAmount <= 500;
            Expression<Func<decimal, bool>> tierTwo = (transactionAmount) => transactionAmount > 500 && transactionAmount <= 7500;
            Expression<Func<decimal, bool>> tierThree = (transactionAmount) => transactionAmount > 7500;

            //get previous duration(30 days before)
            //get previous orders
            var previousCompletedOrders = DataUnitOfWork.Orders.GetAll().Where(q => q.UserId == user.Id &&
                                                         (q.Status == (int)Data.Enums.OrderStatus.Paid || q.Status == (int)Data.Enums.OrderStatus.KYCApprovalPending ||
                                                          q.Status == (int)Data.Enums.OrderStatus.AMLApprovalPending || q.Status == (int)Data.Enums.OrderStatus.KYCApproved ||
                                                          q.Status == (int)Data.Enums.OrderStatus.AMLApproved || q.Status == (int)Data.Enums.OrderStatus.Sending ||
                                                          q.Status == (int)Data.Enums.OrderStatus.PayoutAwaitsApproval || q.Status == (int)Data.Enums.OrderStatus.Sent ||
                                                          q.Status == (int)Data.Enums.OrderStatus.ReleasingPayment || q.Status == (int)Data.Enums.OrderStatus.ReleasedPayment ||
                                                          q.Status == (int)Data.Enums.OrderStatus.Completed || q.Status == (int)Data.Enums.OrderStatus.PayoutApproved ||
                                                          q.Status == (int)Data.Enums.OrderStatus.ComplianceOfficerApproval || q.Status == (int)Data.Enums.OrderStatus.CustomerResponsePending))
                                                          .Select(q => new { Amount = q.Amount.Value, Currency = q.Currency.Code, q.CurrencyId }).ToList();

            ////if there are any previous orders then get previous order amount
            var previousOrderAmount = 0M;
            if (previousCompletedOrders.Any())
                foreach (var previousOrder in previousCompletedOrders)
                    previousOrderAmount += previousOrder.Amount / OpenExchangeRates.GetEURExchangeRate(previousOrder.Currency, HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>);

            //get total transaction amount
            var totalTransactionAmount = previousOrderAmount + buyAmount / eurRate;

            //get tier level based on which tier, total transaction amount belongs to
            if (tierOne.Compile()(totalTransactionAmount))
                tierLevel = 1;
            else if (tierTwo.Compile()(totalTransactionAmount))
                tierLevel = 2;
            else if (tierThree.Compile()(totalTransactionAmount))
                tierLevel = 3;
            return totalTransactionAmount;
        }


        public void UpdateUserTier(User user, decimal buyAmount, decimal eurRate)
        {
            var customerTier = user.Tier;
            var kycCount = DataUnitOfWork.KycFiles.GetAll().Where(q => q.UserId == user.Id && q.Approved.HasValue).Count();
            int tierLevel = 0;
            this.GetUserTier(user, buyAmount, eurRate, ref tierLevel);
            if (tierLevel == 2)
            {
                if (kycCount > 0 && user.Tier != CustomerTier.Tier2)
                {
                    //customerTier = CustomerTier.Tier2;
                    customerTier = CustomerTier.Tier2;
                }
                else if (kycCount == 0 && user.Tier != CustomerTier.Tier2Pending)
                {
                    customerTier = CustomerTier.Tier2Pending;
                }
            }
            else if (tierLevel == 3)
            {
                if (kycCount >= 0 && user.Tier != CustomerTier.Tier3)
                {
                    customerTier = CustomerTier.Tier3Pending;
                }
            }
            if (user.Tier != customerTier)
            {
                // TODO: partial update
                var dc = new MonniData();
                var resVal = dc.Users.FirstOrDefault(q => q.Id == user.Id);
                resVal.Tier = customerTier;
                dc.SaveChanges();
            }

        }

        private PaymentBoundaryDetails GetPaymentMethodDetail(Site currentSite, int type, string paymentMethod = PaymentMethods.CC)
        {
            PaymentBoundaryDetails currentPaymentMethodDetail;
            try
            {
                var overwriteCardFee = SettingsManager.GetDefault().Get("OverwriteCardFee").Value;
                var sitePaymentMethodDetails = SettingsManager.GetDefault().Get(type == (int)Sell ? "SellPaymentMethodDetails" : "SitePaymentMethodDetails").GetJsonData<PaymentBoundaryDetails>();
                var paymentMethodDetail = sitePaymentMethodDetails;
                if (paymentMethodDetail == null)
                    throw new Exception("SitePaymentMethodDetails not defigned for site " + currentSite.Text); // + " -" + currentSite.Id.ToString());

                if (overwriteCardFee != null && !string.IsNullOrEmpty(overwriteCardFee) && paymentMethod.Equals(PaymentMethods.CC) && type == (int)Buy)
                {
                    var overwriteCardFeeValue = 0.0m;
                    decimal.TryParse(overwriteCardFee, NumberStyles.Number, new CultureInfo("en-US"), out overwriteCardFeeValue);
                    if (overwriteCardFeeValue > 0.0m)
                        paymentMethodDetail.CCCommission.Fee = overwriteCardFee;
                }
                // paymentMethodDetail.BankCommission.DisplayName = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", paymentMethod);
                currentPaymentMethodDetail = paymentMethodDetail;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in reading SitePaymentMethodDetails from AppSettings.", ex.ToMessageAndCompleteStacktrace()), ex);
            }
            return currentPaymentMethodDetail;
        }

        private decimal GetCommissionPercent(string paymentMethod, Site currentSite, string userPaymentMethodDetails, string overwriteCardFee, int type)
        {
            try
            {
                decimal commission = 0.0M;
                if (string.IsNullOrEmpty(overwriteCardFee))
                    throw new Exception("OverwriteCardFee value is null or empty.");

                var overwriteCardFeeValue = 0.0M;
                decimal.TryParse(overwriteCardFee, NumberStyles.Number, new CultureInfo("en-US"), out overwriteCardFeeValue);
                //get payment method details
                var paymentMethodDetails = JsonConvert.DeserializeObject<PaymentBoundaryDetails>(userPaymentMethodDetails);
                var paymentMethodDetail = paymentMethodDetails;
                if (paymentMethodDetail == null)
                    throw new Exception("Unable to find PaymentMethodDetail.");

                //foreach (var method in paymentMethodDetail.Methods.Where(q => q.Name == paymentMethod))
                // {

                if (paymentMethod.Equals(PaymentMethods.CC))
                {
                    if (overwriteCardFeeValue != 0.0M)
                        commission = overwriteCardFeeValue;
                    else
                    {
                        if (!decimal.TryParse(paymentMethodDetail.CCCommission.Fee, NumberStyles.Number, new CultureInfo("en-US"), out commission))
                            throw new Exception("Unable to extract PaymentMethod fee '" + paymentMethodDetail.CCCommission.Fee + "' from AppSettings.");
                    }
                }
                else
                {
                    if (!decimal.TryParse(paymentMethodDetail.BankCommission.Fee, NumberStyles.Number, new CultureInfo("en-US"), out commission))
                        throw new Exception("Unable to extract PaymentMethod fee '" + paymentMethodDetail.BankCommission.Fee + "' from AppSettings.");
                }
                //}

                if (paymentMethod == PaymentMethods.CC)
                    if (commission == 0.0M)
                        throw new Exception("Commission is 0.0.");

                AuditLog.log("Calculated GetCommissionPercent(" + paymentMethod + "," + currentSite.Text + "," + userPaymentMethodDetails + "," + overwriteCardFee + "," + type + ").\r\n" +
                    "Commission:" + commission, (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Info);

                return commission;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetCommissionPercent(" + paymentMethod + "," + currentSite.Text + "," + userPaymentMethodDetails + "," + overwriteCardFee + "," + type + ").\r\n" + ex.ToMessageAndCompleteStacktrace());
            }
        }

        private decimal? GetOurFeePercent(string paymentMethod, Site currentSite, string userPaymentMethodDetails, string overwriteCardFee, int type)
        {
            try
            {
                decimal? ourFee = 0.0M;
                //get payment method details
                var paymentMethodDetails = JsonConvert.DeserializeObject<PaymentBoundaryDetails>(userPaymentMethodDetails);
                var paymentMethodDetail = paymentMethodDetails;
                if (paymentMethodDetail == null)
                    throw new Exception("Unable to find PaymentMethodDetail.");

                //foreach (var method in paymentMethodDetail.Methods.Where(q => q.Name == paymentMethod))
                if (paymentMethod.Equals(PaymentMethods.Bank))
                {
                    ourFee = paymentMethodDetail.BankCommission.Commission;
                }
                else if (paymentMethod.Equals(PaymentMethods.CC))
                {
                    ourFee = paymentMethodDetail.CCCommission.Commission;
                }
                return ourFee;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetOurFeePercent(" + paymentMethod + "," + currentSite.Text + "," + userPaymentMethodDetails + "," + overwriteCardFee + "," + type + ").\r\n" + ex.ToMessageAndCompleteStacktrace());
            }
        }

        /// <summary>
        /// get payment gateway type 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="paymentGatewayType"></param>
        /// <returns></returns>
        private string CalculatePaymentGatewayType(long siteId, string paymentGatewayType, string currencyCode, bool isUserTrusted)
        {
            if (paymentGatewayType.ToLower() == "auto")
            {
                if (siteId == 2 && currencyCode.ToLower().Trim() == "dkk")
                    paymentGatewayType = "PayLike";
                else
                    paymentGatewayType = "YourPay";
            }
            return paymentGatewayType;
        }

        private List<Tuple<string, int>> GetCurrenciesWithType(Site currentSite)
        {
            //List<Tuple<string, int>> currencies;
            var currencies = new List<Tuple<string, int>>();

            try
            {
                if (currentSite != null && currentSite.CurrencyId.HasValue)
                    DataUnitOfWork.Currencies.Get(
                        q => q.CurrencyType.Text == "Fiat"
                         && !string.IsNullOrEmpty(q.Code)
                         || q.Id == currentSite.CurrencyId).OrderBy(q => q.Code)
                        .Select(q => new { q.Code, q.PaymentTypeAcceptance }).ToList()
                        .ForEach(q => currencies.Add(new Tuple<string, int>(q.Code, Convert.ToInt16(q.PaymentTypeAcceptance))));
                else
                {
                    DataUnitOfWork.Currencies.Get(
                        q => q.CurrencyType.Text == "Fiat"
                          && !string.IsNullOrEmpty(q.Code)
                          || q.Id == currentSite.CurrencyId).OrderBy(q => q.Code)
                        .Select(q => new { q.Code, q.PaymentTypeAcceptance }).ToList()
                        .ForEach(q => currencies.Add(new Tuple<string, int>(q.Code, Convert.ToInt16(q.PaymentTypeAcceptance))));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in getting currencies from Currencies table.", ex));
            }
            return currencies;
        }

        private List<string> GetCurrencies(Site currentSite)
        {
            List<string> currencies;
            try
            {
                if (currentSite != null && currentSite.CurrencyId.HasValue)
                    currencies = DataUnitOfWork.Currencies.Get(q => q.CurrencyType.Text == "Fiat" && !string.IsNullOrEmpty(q.Code)).Where(q => q.Id == currentSite.CurrencyId).Select(q => q.Code).ToList();
                else
                {
                    currencies = DataUnitOfWork.Currencies.Get(q => q.CurrencyType.Text == "Fiat" && !string.IsNullOrEmpty(q.Code)).Select(q => q.Code).ToList();
                    //if (!currencies.Contains("USD"))
                    //    currencies.Add("USD");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in getting currencies from Currencies table.", ex));
            }

            return currencies;
        }

        private RequestObjectModel GetRequestObjectModel()
        {
            try
            {
                RequestObjectModel requestObjectModel = new RequestObjectModel() { header = new Header() };
                Request.InputStream.Position = 0;
                using (StreamReader reader = new StreamReader(Request.InputStream))
                {
                    for (int i = 0; i < Request.Headers.Count; i++)
                    {
                        string key = Request.Headers.AllKeys[i];
                        requestObjectModel.header.RequestHeader.Add(new KeyValuePair<string, string>(key, Request.Headers[key]));
                    }
                    string requestFromPost = reader.ReadToEnd();
                    requestObjectModel.body = requestFromPost;
                }

                return requestObjectModel;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetRequestObjectModel().", ex);
            }
        }

        private User GetUser(string fullname, string email, string phoneNumber, string paymentMethodDetailsJSON, string sellpaymentMethodDetailsJSON, string transactionLimitsDetailsJSON, string CreditCardLimitsDetailsJSON, Country userCountry, bool recieveNewsletters)
        {
            try
            {
                var user = DataUnitOfWork.Users.Get(q => q.Phone == phoneNumber).FirstOrDefault();
                //if user is null create new user else update existing user
                if (user == null)
                {
                    var newUser = new User
                    {
                        Fname = fullname,
                        Email = email,
                        Phone = phoneNumber,
                        Created = DateTime.Now,
                        RoleId = 1, //Client
                        UserType = 1, //c2b
                        CountryId = userCountry == null ? (long?)null : userCountry.Id,
                        PaymentMethodDetails = paymentMethodDetailsJSON,
                        TransactionLimitsDetails = transactionLimitsDetailsJSON,
                        Newsletter = recieveNewsletters,
                        SellPaymentMethodDetails = sellpaymentMethodDetailsJSON,
                        CreditCardLimitsDetails = CreditCardLimitsDetailsJSON
                    };
                    DataUnitOfWork.Users.Add(newUser);
                    AuditLog.log(string.Format("Logged in by new user with phone number {0}.", phoneNumber), (int)Data.Enums.AuditLogStatus.UserLogin, (int)Data.Enums.AuditTrailLevel.Info);
                }
                else
                {
                    user.Fname = fullname;
                    user.Email = email;
                    user.Newsletter = recieveNewsletters;
                    user.CountryId = userCountry == null ? (long?)null : userCountry.Id;
                    //TODO if user.PaymentMethodDetails is null then update the paymentMethodDetails
                    if (user.PaymentMethodDetails == null)
                    {
                        user.PaymentMethodDetails = paymentMethodDetailsJSON;
                    }
                    if (user.SellPaymentMethodDetails == null)
                    {
                        user.SellPaymentMethodDetails = sellpaymentMethodDetailsJSON;
                    }

                    if (user.TransactionLimitsDetails == null)
                    {
                        user.TransactionLimitsDetails = transactionLimitsDetailsJSON;
                    }

                    if (user.CreditCardLimitsDetails == null)
                    {
                        user.CreditCardLimitsDetails = CreditCardLimitsDetailsJSON;
                    }


                    AuditLog.log(string.Format("Logged in by existing user with phone number {0}.", phoneNumber), (int)Data.Enums.AuditLogStatus.UserLogin, (int)Data.Enums.AuditTrailLevel.Info);
                }
                //commit the changes done
                DataUnitOfWork.Commit();
                return DataUnitOfWork.Users.GetUserByPhoneNumber(phoneNumber);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in updating user data.", ex);
            }
        }

        /// <summary>
        /// method to convert stream to byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private byte[] StreamToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// generate unique order number for orders
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>

        private static volatile object syncRoot = new object();
        private long getUniqueOrderNumber()
        {
            long new_OrderId = 0;
            lock (syncRoot)
            {
                var dataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
                List<long> triedIds = new List<long>();
                bool idExists = true;
                do
                {
                    List<long> newIds = new List<long>();
                    do
                    {
                        new_OrderId = new Random().Next(100000, 999999);
                    }
                    while (triedIds.Contains(new_OrderId));

                    triedIds.Add(new_OrderId);

                    //if newly generated OrderId is already present in Order table then again generate new random number
                    idExists = dataUnitOfWork.Orders.Get(q => q.Number == new_OrderId.ToString()).Select(x => x.Number).Any();
                }
                while (idExists);
            }

            return new_OrderId;
        }

        /// <summary>
        /// method to get country code user IP
        /// </summary>
        /// <returns></returns>
        private string GetCountryCodeFromUserIP()
        {
            try
            {
                if (Session["IPInfoJSONData"] == null || string.IsNullOrEmpty(Session["IPInfoJSONData"] as string) || Session["IPInfoJSONData"].ToString() == "null" || Session["IPInfoJSONData"].ToString() == "DK")
                {
                    //AuditLog.log("Reset IPInfo Session Data. IPInfoJSONData:" + Session["IPInfoJSONData"], 6);
                    var ipinfo = IPInfoService.GetIPInfo(GetLanIPAddress());
                    if (string.IsNullOrEmpty(ipinfo))
                        Session["IPInfoJSONData"] = ResolveCountry().Name;
                    return Session["IPInfoJSONData"] as string;
                }
                else
                    return Session["IPInfoJSONData"] as string;
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in GetCountryCodeFromUserIP()\r\nIPInfoJSONData Session: '" + Session["IPInfoJSONData"].ToString() + "' \r\n" + ex.ToMessageAndCompleteStacktrace() +
                    "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                var result = new BetterJsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                result.AddError(ex.Message);
                return ex.Message + "\n" + ex.StackTrace;
            }
        }

        /// <summary>
        /// method to get culture info
        /// </summary>
        /// <returns></returns>
        private CultureInfo ResolveCulture()
        {
            string[] languages = HttpContext.Request.UserLanguages;

            if (languages == null || languages.Length == 0)
            {
                AuditLog.log("There is no Languages in ResolveCulture(). ", (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info);
                return null;
            }

            try
            {
                string language = languages[0].ToLowerInvariant().Trim(); //get user language
                return CultureInfo.CreateSpecificCulture(language);
            }
            catch (ArgumentException)
            {
                AuditLog.log("Error while Creating specific Culture based on Language in ResolveCulture().", (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info);
                return null;
            }
        }

        /// <summary>
        /// method to get culture identifier
        /// </summary>
        /// <returns></returns>
        private RegionInfo ResolveCountry()
        {
            //get culture info
            CultureInfo culture = ResolveCulture();
            if (culture != null)
            {
                return new RegionInfo(culture.LCID); //culture identifier
            }
            else
            {
                AuditLog.log("There is no Culture in ResolveCountry().", (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info);
                return null;
            }
        }

        /// <summary>
        /// method to get IP
        /// </summary>
        /// <returns></returns>
        private string GetLanIPAddress()
        {
            string ip = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            return string.IsNullOrEmpty(ip) ? HttpContext.Request.ServerVariables["REMOTE_ADDR"] : ip;
        }

        /// <summary>
        /// method to get minimum and maximum size boundry for particular currency
        /// </summary>
        /// <param name="siteId">Site Id</param>
        /// <param name="paymentMethod">Payment method selected by user</param>
        /// <param name="currency">currency selected by user</param>
        /// <returns></returns>
        private Models.OrderSizeBoundary GetOrderSizeBoundary(long siteId, string paymentMethod, string currency, int type)
        {
            try
            {
                var paymentMethodDetailJson = SettingsManager.GetDefault().Get((type == (int)Sell ? "SellPaymentMethodDetails" : "SitePaymentMethodDetails")).GetJsonData<PaymentBoundaryDetails>();
                if (paymentMethodDetailJson == null)
                    AuditLog.log("SitePaymentMethodDetails is not defined in the database.", (int)Data.Enums.AuditLogStatus.ApplicationError,
                        (int)Data.Enums.AuditTrailLevel.Debug);
                return GetOrderSizeBoundary(siteId, paymentMethod, currency, type, paymentMethodDetailJson).Item1;

            }
            catch (Exception ex)
            {
                throw new Exception("Error in reading OrderSizeBoundary(" + siteId + "," + paymentMethod + "," + currency + ").", ex);
            }
        }

        private Tuple<Models.OrderSizeBoundary, decimal?> GetOrderSizeBoundary(long siteId, string paymentMethod, string currency, int type, PaymentBoundaryDetails paymentMethodDetailJson)
        {
            var orderSizeBoundary = new Models.OrderSizeBoundary();
            decimal? commission = 0.0M;
            //foreach (var paymentMethodDetail in paymentMethodDetailJson)
            //{
            // Sell -> Direction value is false -> it executes the paymentMethodDetail.Methods loop
            // Buy  -> Direction value is true - continue; will be executed 
            //   until the it matches with the siteid. When found it executes the paymentMethodDetail.Methods loop 
            //if (type == (int)Buy && paymentMethodDetail.SiteId != siteId)
            //    continue;

            // foreach (var method in paymentMethodDetail.Methods)
            if (paymentMethod.Equals(PaymentMethods.Bank))
            {
                orderSizeBoundary = paymentMethodDetailJson.Boundaries[currency];
                commission = paymentMethodDetailJson.BankCommission.Commission;
            }
            else if (paymentMethod.Equals(PaymentMethods.CC))
            {
                orderSizeBoundary = paymentMethodDetailJson.CCOrderSizeBoundary;
                commission = paymentMethodDetailJson.CCCommission.Commission;
            }

            //}
            var latestExchangeRates = HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>;
            var rate = OpenExchangeRates.GetEURExchangeRate(currency, latestExchangeRates);
            var cultureInfo = ResolveCulture();
            var ci = new CultureInfo(cultureInfo.Name); //get culture info
            var numberFormat = "N2";
            //TODO:currency must be checked from currency table wether it is digital currency
            if (currency == "BTC")
            {
                numberFormat = "N8";
            }
            //var euroRates = OpenExchangeRates.GetLatestExchangeRates().Rates;
            //var euroExchangeRate = latestExchangeRates.Where(q => q.Key == currency).Select(q => q.Value).FirstOrDefault();
            return new Tuple<OrderSizeBoundary, decimal?>(new Models.OrderSizeBoundary(ci.ToString(), numberFormat)
            {
                Max = orderSizeBoundary.Max * rate,
                Min = orderSizeBoundary.Min * rate
            }, commission
            );
        }

        private static string SHA1HashStringForUTF8String(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);

            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);

            return HexStringFromBytes(hashBytes);
        }

        private static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
        private bool IsValidateFullName(string name)
        {
            return !((name.Split(' ').Length < 2) || (name.Split(' ')[0].Length < 2) || (name.Split(' ')[1].Length < 2));
        }

        private Models.OrderSizeBoundary GetUserOrderSizeBoundary(string currency, string forsellCurrency, string paymentMethod, int type, string phoneNumber, string phoneCode)
        {
            try
            {
                var currentSite = GetCurrentSite();
                phoneNumber = phoneCode + phoneNumber; // remove phone code
                phoneNumber = phoneNumber.Trim().Replace(" ", ""); //remove spaces in phone number
                var user = DataUnitOfWork.Users.Get(q => q.Phone == phoneNumber).FirstOrDefault();
                PaymentBoundaryDetails paymentMethodDetailJson = new PaymentBoundaryDetails { };
                if (user != null)
                {
                    paymentMethodDetailJson = JsonConvert.DeserializeObject<PaymentBoundaryDetails>((type == (int)Sell ? user.SellPaymentMethodDetails : user.PaymentMethodDetails));
                }
                else
                {
                    paymentMethodDetailJson = SettingsManager.GetDefault().Get((type == (int)Sell ? "SellPaymentMethodDetails" : "SitePaymentMethodDetails")).GetJsonData<PaymentBoundaryDetails>();
                }
                if (paymentMethodDetailJson == null)
                    AuditLog.log("SitePaymentMethodDetails is not defined in the database.", (int)Data.Enums.AuditLogStatus.ApplicationError,
                        (int)Data.Enums.AuditTrailLevel.Debug);

                return GetOrdersizeBoundary(currency, forsellCurrency, paymentMethod, type, currentSite.Id, paymentMethodDetailJson);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in reading OrderSizeBoundary(" + paymentMethod + "," + currency + ").", ex);
            }
        }

        [EnableThrottling(PerSecond = 20, PerMinute = 50)]
        public ActionResult GetUserSizeBoundarySize(string currency, string forsellCurrency, string paymentMethod, int type, string phoneNumber, string phoneCode)
        {
            try
            {
                var currentSite = GetCurrentSite();
                phoneNumber = phoneCode + phoneNumber; // remove phone code
                phoneNumber = phoneNumber.Trim().Replace(" ", ""); //remove spaces in phone number
                var user = DataUnitOfWork.Users.Get(q => q.Phone == phoneNumber).Select(x => new { x.SellPaymentMethodDetails, x.PaymentMethodDetails }).FirstOrDefault();
                PaymentBoundaryDetails paymentMethodDetailJson = new PaymentBoundaryDetails { };
                if (user != null)
                {
                    if (type == (int)Sell && user.SellPaymentMethodDetails != null)
                    {
                        paymentMethodDetailJson = JsonConvert.DeserializeObject<PaymentBoundaryDetails>(user.SellPaymentMethodDetails);
                    }

                    else if (type == (int)Buy && user.PaymentMethodDetails != null)
                    {
                        paymentMethodDetailJson = JsonConvert.DeserializeObject<PaymentBoundaryDetails>(user.PaymentMethodDetails);
                    }

                    else
                    {
                        paymentMethodDetailJson = SettingsManager.GetDefault().Get((type == (int)Sell ? "SellPaymentMethodDetails" : "SitePaymentMethodDetails")).GetJsonData<PaymentBoundaryDetails>();
                    }
                }
                else
                {
                    paymentMethodDetailJson = SettingsManager.GetDefault().Get((type == (int)Sell ? "SellPaymentMethodDetails" : "SitePaymentMethodDetails")).GetJsonData<PaymentBoundaryDetails>();
                }
                for (var i = 0; i < paymentMethodDetailJson.PaymentMethods.Count; i++)
                {
                    paymentMethodDetailJson.PaymentMethods[i].DisplayName = Helpers.ResourceExtensions.Resource(HttpContext, "WMCResources", paymentMethodDetailJson.PaymentMethods[i].Name);
                }

                if (paymentMethodDetailJson == null)
                    AuditLog.log("SitePaymentMethodDetails is not defined in the database.", (int)Data.Enums.AuditLogStatus.ApplicationError,
                        (int)Data.Enums.AuditTrailLevel.Debug);
                var orderSizeboundary = GetOrderSizeBoundary(currentSite.Id, paymentMethod, type == (int)Buy ? currency : forsellCurrency, type, paymentMethodDetailJson);
                //get order size boundry
                return new BetterJsonResult
                {
                    Data = new
                    {
                        OrderSizeBoundary = orderSizeboundary.Item1,
                        Commission = orderSizeboundary.Item2,
                        SellPaymentMethodDetail = paymentMethodDetailJson
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error in reading OrderSizeBoundary(" + paymentMethod + "," + currency + ").", ex);
            }
        }

        public OrderSizeBoundary GetOrdersizeBoundary(string currency, string forsellCurrency, string paymentMethod, int type, long siteId, PaymentBoundaryDetails paymentMethodDetailJson)
        {
            //var paymentMethodDetail = paymentMethodDetailJson.FirstOrDefault(p => p.SiteId == 0 || p.SiteId == siteId);
            //var method = paymentMethodDetail.Methods.FirstOrDefault(m => m.Name == paymentMethod);

            var paymentMethodDetail = paymentMethodDetailJson;
            var latestExchangeRates = HttpContext.Application["LatestExchangeRates"] as Dictionary<string, decimal>;
            var rate = OpenExchangeRates.GetEURExchangeRate(currency, latestExchangeRates);
            if (paymentMethod.Equals(PaymentMethods.Bank))
            {
                OrderSizeBoundary boundaries = new OrderSizeBoundary();
                boundaries = paymentMethodDetail.Boundaries[currency];
                return new Models.OrderSizeBoundary
                {
                    Max = boundaries.Max * rate,
                    Min = boundaries.Min * rate
                };
            }
            else  //if (paymentMethod.Equals(PaymentMethods.CC))
            {
                OrderSizeBoundary boundaries = new OrderSizeBoundary();
                boundaries = paymentMethodDetail.CCOrderSizeBoundary;
                return new Models.OrderSizeBoundary
                {
                    Max = boundaries.Max * rate,
                    Min = boundaries.Min * rate
                };
            }

        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidateOrderSizeBoundary(long siteId, decimal amount, int type, string paymentMethod = null, string currency = null, string sellCurrency = null)
        {
            if ((type == (int)Buy) ? currency == null : sellCurrency == null)
                return false;
            var orderSizeBoundary = GetOrderSizeBoundary(siteId, paymentMethod, (type == (int)Buy) ? currency : sellCurrency, type); //Note: For Sell, range should always be in BTC
            if (amount < orderSizeBoundary.Min || amount > orderSizeBoundary.Max)
                return false;
            else
                return true;
        }

        [HttpGet]
        [EnableThrottling(PerSecond = 2, PerMinute = 5)]
        public string GetMessage(string key, string language)
        {
            string errorMessage = DataUnitOfWork.LanguageResource.GetLanguageResource(key, language);
            return errorMessage;
        }

        #endregion
    }
}