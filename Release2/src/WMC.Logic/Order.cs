using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Paylike.NET;
using Paylike.NET.RequestModels.Transactions;
using RestSharp;
using WMC.Data;
using WMC.Data.Enums;
using WMC.Logic.Models;
using WMC.Utilities;
using static WMC.Data.Enums.OrderType;
using static WMC.Logic.WebhookUtility;

namespace WMC.Logic
{
    public class OrderLogic
    {
        public Dictionary<string, decimal> Rates { get; set; }
        public OrderLogic()
        {
        }

        // TODO: this must be operated from scheduller? seems it is commented now
        /// <summary>
        /// 
        /// </summary>
        public void UpdateMinersFee()
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            try
            {
                BitGoAccessSettings bitGoAccessSettings = SettingsManager.GetDefault().Get("BitGoAccessCode", true).GetJsonData<BitGoAccessSettings>();

                var cryptoCurrencies = DataUnitOfWork.Currencies.Get(curr => curr.CurrencyTypeId == (int)CurrencyTypes.Digital && curr.IsActive == true);
                foreach (var item in cryptoCurrencies)
                {
                    var bitgoSettingJson = item.BitgoSettings;
                    var bitgoSetting = JsonConvert.DeserializeObject<BitGoCurrencySettings>(bitgoSettingJson);
                    var bitGoAccess = new BitGoAccess(bitGoAccessSettings, item.Code);
                    var minersFee = BitGoUtil.GetEstimateFee(bitGoAccessSettings, bitgoSetting.DefaultWalletId, new Dictionary<string, long> { { bitGoAccess.GetWallet(bitgoSetting.DefaultWalletId).receiveAddress.address, (long)bitgoSetting.DefaultAmount } }, item.Code);
                    minersFee.fee = ((decimal)minersFee.fee / (decimal)bitgoSetting.TxUnit);
                    item.BitgoSettings = JsonConvert.SerializeObject(new BitGoCurrencySettings
                    {
                        DefaultWalletId = bitgoSetting.DefaultWalletId,
                        DefaultAmount = bitgoSetting.DefaultAmount,
                        TestCurrency = bitgoSetting.TestCurrency,
                        KrakenCode = bitgoSetting.KrakenCode,
                        KrakenEurPairCode = bitgoSetting.KrakenEurPairCode,
                        PassPhrase = bitgoSetting.PassPhrase,
                        TxUnit = bitgoSetting.TxUnit,
                        MinersFee = minersFee
                    });
                    DataUnitOfWork.Currencies.Update(item);
                    DataUnitOfWork.Commit();
                }

            }
            catch (Exception ex)
            {
                AuditLog.log(string.Format("Error updating MinersFee.\r\n" + ex.Message + "\r\n" + ex.StackTrace, ex), (int)AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        public void UpdateOrderRateFromKrakenOrderBook()
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            try
            {
                RetrierSettings retrierAppSettings = SettingsManager.GetDefault().Get("RetrierSettings").GetJsonData<RetrierSettings>();
                if (retrierAppSettings == null)
                    throw new Exception("Unable to find 'RetrierSettings' key in AppSettings.");

                var orderIds = DataUnitOfWork.Orders.Get(q => q.Status == (int)Data.Enums.OrderStatus.Paid || q.Status == (int)Data.Enums.OrderStatus.Sending || q.Status == (int)Data.Enums.OrderStatus.PayoutAwaitsApproval).Select(q => q.Id).ToList();
                foreach (var orderId in orderIds)
                {
                    var order = DataUnitOfWork.Orders.GetById(orderId);
                    // TODO: select
                    var currency = DataUnitOfWork.Currencies.GetById(order.CurrencyId);
                    // TODO: select
                    var cryptoCurrency = DataUnitOfWork.Currencies.Get(cur => cur.Id == order.CryptoCurrencyId).FirstOrDefault();
                    var eurExchangeRate = OpenExchangeRates.GetEURExchangeRate(currency.Code, Rates);
                    var dkkeurExchangeRate = OpenExchangeRates.GetEURExchangeRate("DKK", Rates);
                    decimal orderRate = 0.0M;

                    var orderAmount = order.Amount;
                    if (currency.Code != "EUR")
                        orderAmount = order.Amount / eurExchangeRate; // Convert order amount to EUR

                    var btceurPrice = new Retrier<decimal>().Try(() => KrakenExchange.GetBTCEURPrice(orderAmount.Value, cryptoCurrency.Code), Int32.Parse(retrierAppSettings.MaxRetries), Int32.Parse(retrierAppSettings.DelayInMilliseconds));
                    if (order.Type == (int)Data.Enums.OrderType.Buy)
                    {
                        order.RateBase = decimal.Round(btceurPrice, 8);
                        order.RateHome = decimal.Round(eurExchangeRate, 8);
                        order.RateBooks = decimal.Round(dkkeurExchangeRate, 8);
                        orderRate = decimal.Round(btceurPrice * eurExchangeRate, 8);
                        order.Rate = orderRate; // Get BTC EUR rate from Kraken and convert to order's currency
                        DataUnitOfWork.Orders.Update(order);
                        DataUnitOfWork.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: commented?
                //AuditLog.log(string.Format("Error updating Rate from Kraken order book. OrderRate : {0}.", ex), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        public void ProcessKYC()
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            try
            {
                //order.Status is equal to 'Paid' or 'KYC Approval Pending'
                var orderIds = DataUnitOfWork.Orders.Get(q => q.Status == (int)Data.Enums.OrderStatus.Paid || q.Status == (int)Data.Enums.OrderStatus.KYCApprovalPending).Select(q => q.Id).ToList();
                foreach (var orderId in orderIds)
                {
                    AuditLog.log("Executed ProcessKYC() for order:" + orderId, (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, orderId);
                    var order = DataUnitOfWork.Orders.GetById(orderId);
                    // TODO: Select
                    var user = DataUnitOfWork.Users.GetById(order.UserId);
                    //UserRiskLevel greater than LowRisk proccessed which internal does checks the kyc approval
                    if (user.UserRiskLevel == Data.Enums.UserRiskLevelType.LowRisk)
                    {

                        //                         Get all KYC files of the user of type PhotoID or ProofOfRecidency or SelfieId
                        var kycFiles = DataUnitOfWork.KycFiles.Get(q => q.UserId == order.UserId && (q.Type == 1 || q.Type == 2 || q.Type == 4));

                        var orderStatusChanged = false;

                        var checkKyc = CheckKyc.isKYCApproved(order.Id);

                        if (checkKyc)
                        {
                            order.Status = (int)Data.Enums.OrderStatus.KYCApproved; //KYC Approval Approved
                            orderStatusChanged = true;
                        }
                        else
                        {
                            order.Status = (int)Data.Enums.OrderStatus.KYCApprovalPending; //KYC Approval Pending
                            orderStatusChanged = false;
                        }

                        ////                         Any file KYC fine is not 'Approved'
                        //if (kycFiles.Any(q => !q.Approved.HasValue))
                        //    order.Status = (int)Data.Enums.OrderStatus.KYCApprovalPending; //KYC Approval Pending

                        //                         All KYC is 'Approved' and none are 'Obsolete'
                        if (kycFiles.All(q => q.Approved.HasValue && !q.Obsolete.HasValue) && kycFiles.Count() > 0)
                        {
                            foreach (var kycFile in kycFiles)
                                DataUnitOfWork.OrderKycfiles.Add(new OrderKycfile { Order = order, KycFile = kycFile });
                            order.Status = (int)Data.Enums.OrderStatus.KYCApproved; //KYC Approved
                            orderStatusChanged = true;
                        }


                        //                         None of the KYC is 'Approved' or 'Rejected'
                        if (kycFiles.All(q => q.Rejected.HasValue) && kycFiles.Count() > 0)
                        {
                            order.Status = (int)Data.Enums.OrderStatus.KYCDeclined; //KYC Declined
                            orderStatusChanged = true;
                        }
                        DataUnitOfWork.Orders.Update(order);
                        if (orderStatusChanged)
                        {
                            AuditLog.log("Updated Order Status at ProcessKYC() to " + Enum.GetName(typeof(Data.Enums.OrderStatus), order.Status) + " while processing KYC for the Order" + orderId, (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, orderId);
                        }
                        if (!orderStatusChanged && order.Type == (int)Sell && order.Status == (long)Data.Enums.OrderStatus.Paid && (kycFiles.Count() == 0 || kycFiles.All(q => q.Approved.HasValue)))
                        {
                            order.Status = (int)Data.Enums.OrderStatus.PayoutApproved; //Set order.Status as 'Payout approved'
                            DataUnitOfWork.Orders.Update(order);
                            AuditLog.log("Updated Order(" + orderId + ") Status to " + Enum.GetName(typeof(Data.Enums.OrderStatus), order.Status) + " while processing Sell Order in ProcessKYC().", (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, orderId);
                        }
                        DataUnitOfWork.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error while processing KYC. " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        public void ProcessOrderApproval()
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            try
            {
                RetrierSettings retrierAppSettings = SettingsManager.GetDefault().Get("RetrierSettings").GetJsonData<RetrierSettings>();
                if (retrierAppSettings == null)
                    throw new Exception("Unable to find 'RetrierSettings' key in AppSettings.");

                //order.Status is equal to 'Approved' or 'Sending'
                var orderIds = DataUnitOfWork.Orders.Get(q => q.Status == (int)Data.Enums.OrderStatus.KYCApproved || q.Status == (int)Data.Enums.OrderStatus.Sending).Select(q => q.Id).ToList();
                foreach (var orderId in orderIds)
                {
                    var order = DataUnitOfWork.Orders.GetById(orderId);
                    if (order.Type == (int)Data.Enums.OrderType.Buy) ///Include Direction in the query above.
                    {
                        // TODO: select
                        var currency = DataUnitOfWork.Currencies.GetById(order.CurrencyId);
                        var payoutAutoLimit = 0.0M;
                        var payoutAutoLimitAppSettings = SettingsManager.GetDefault().Get("PayoutAutoLimit").Value;
                        if (payoutAutoLimitAppSettings == null)
                            throw new Exception("Unable to find 'PayoutAutoLimit' key in AppSettings.");
                        if (!decimal.TryParse(payoutAutoLimitAppSettings, NumberStyles.Number, new CultureInfo("en-US"), out payoutAutoLimit)) //Get PayoutAutoLimit value
                            throw new Exception("Unable to parse 'PayoutAutoLimit' value in AppSettings.");

                        // Get Order Amount
                        var eurExchangeRate = OpenExchangeRates.GetEURExchangeRate(currency.Code, Rates);
                        var orderAmount = order.Amount;
                        if (currency.Code != "EUR")
                            orderAmount = order.Amount / eurExchangeRate; // Convert order amount to EUR

                        if (orderAmount > payoutAutoLimit)
                            order.Status = (int)Data.Enums.OrderStatus.PayoutAwaitsApproval; //Set order.Status as 'Payout awaits approval'
                        else
                            order.Status = (int)Data.Enums.OrderStatus.PayoutApproved; //Set order.Status as 'Payout approved'
                        DataUnitOfWork.Orders.Update(order);
                        AuditLog.log("Updated Order(" + orderId + ") Status to " + Enum.GetName(typeof(Data.Enums.OrderStatus), order.Status) + " while processing ProcessOrderApproval().", (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, orderId);
                        DataUnitOfWork.Commit();
                    }
                    else
                    {
                        order.Status = (int)Data.Enums.OrderStatus.PayoutApproved; //Set order.Status as 'Payout approved'
                        DataUnitOfWork.Orders.Update(order);
                        AuditLog.log("Updated Order(" + orderId + ") Status to " + Enum.GetName(typeof(Data.Enums.OrderStatus), order.Status) + " while processing Sell Order in ProcessOrderApproval().", (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, orderId);
                        DataUnitOfWork.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in ProcessOrderApproval(): " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        public void ProcessOrderAwaitsApproval()
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            try
            {
                var orderIds = DataUnitOfWork.Orders.Get(q => q.Status == (int)Data.Enums.OrderStatus.PayoutAwaitsApproval).Select(q => q.Id).ToList();
                foreach (var orderId in orderIds)
                {
                    var order = DataUnitOfWork.Orders.GetById(orderId);
                    string trustMessage = "";
                    var creditCardApproval = new TrustLogic.TrustLogic { Rates = Rates };
                    var riskscore = creditCardApproval.TrustLevelCalculation(orderId, ref trustMessage);
                    AuditLog.log("Riskscore TrustMessage" + trustMessage, (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Error, orderId);
                    creditCardApproval.TrustCalculationExecution(orderId, riskscore);
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in ProcessOrderAwaitsApproval(): " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        public void CapturePayment()
        {
            //TODO CAPTURE PAYMENT TO BE MOVED TO PROCCESS ORDER
            using (var dataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories())))
            {
                try
                {
                    RetrierSettings retrierAppSettings = SettingsManager.GetDefault().Get("RetrierSettings").GetJsonData<RetrierSettings>();
                    if (retrierAppSettings == null)
                        throw new Exception("Unable to find 'RetrierSettings' key in AppSettings.");

                    var orderIds = dataUnitOfWork.Orders.Get(q => q.Status == (int)Data.Enums.OrderStatus.PayoutApproved).Select(q => q.Id).ToList();
                    foreach (var orderId in orderIds)
                    {
                        var order = dataUnitOfWork.Orders.GetById(orderId);
                        // TODO: Select
                        var currency = dataUnitOfWork.Currencies.GetById(order.CurrencyId);
                        // TODO: Select
                        var cryptoCurrency = dataUnitOfWork.Currencies.Get(cur => cur.Id == order.CryptoCurrencyId).FirstOrDefault();
                        // TODO: Select
                        var site = dataUnitOfWork.Sites.GetById(order.SiteId.Value);

                        if (order.PaymentType == (int)OrderPaymentType.CreditCard && order.PaymentGatewayType == "PayLike" && order.Type == (int)Data.Enums.OrderType.Buy)
                        {
                            dynamic payLikeDetails = PayLikeService.GetPayLikeDetails(order.SiteId.Value, order.CurrencyId);
                            //var paylikeTransactionService = new PaylikeTransactionService(payLikeDetails.PayLikeAppKey.ToString());
                            var paylikeTransactionService = new PaylikeTransactionService(payLikeDetails.AppKey.ToString());
                            var captureTransaction = paylikeTransactionService.CaptureTransaction(new CaptureTransactionRequest
                            {
                                TransactionId = order.ExtRef,
                                Amount = (int)Convert.ToDouble((order.Amount * 100).ToString()),
                                Currency = currency.PayLikeCurrencyCode,
                                Descriptor = (site.Text.Contains("localhost") ? site.Text : site.Text.Split('.')[1] + "." + site.Text.Split('.')[2]) + ":" + order.TxSecret
                            });
                            if (!captureTransaction.IsError)
                            {
                                AuditLog.log("Paylike CapturePayment Response: " + JsonConvert.SerializeObject(captureTransaction),
                                    (int)Data.Enums.AuditLogStatus.PayLike, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                var transaction = dataUnitOfWork.Transactions.Get(q => q.ExtRef == order.ExtRef && q.OrderId == order.Id).FirstOrDefault();
                                if (transaction != null)
                                    transaction.Completed = DateTime.Now;
                                else
                                    AuditLog.log("Unable to find Order Transaction record.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug, order.Id);

                                //Set order.Status as 'Released payment'
                                order.Status = (int)Data.Enums.OrderStatus.ReleasedPayment;
                            }
                            else
                            {
                                AuditLog.log("Paylike CapturePayment Error Response: " + JsonConvert.SerializeObject(captureTransaction),
                                    (int)Data.Enums.AuditLogStatus.PayLike, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                //Set order.Status as 'Capture Errored'
                                order.Status = (int)Data.Enums.OrderStatus.CaptureErrored;
                            }
                        }
                        if (order.PaymentType == (int)OrderPaymentType.CreditCard && order.PaymentGatewayType == "YourPay")
                        {
                            var orderAmount = 0;
                            if (order.Type == (int)Buy)
                                orderAmount = (int)(order.Amount.Value * 100);
                            else if (order.Type == (int)Data.Enums.OrderType.Sell)
                            {
                                var transactionDetail = BitGoUtil.GetTransactionDetails(order.TransactionHash, cryptoCurrency.Code);
                                if (transactionDetail.Item3 < 1)
                                {
                                    AuditLog.log("Order confirmations: " + transactionDetail.Item3, (int)AuditLogStatus.BitGo, (int)Data.Enums.AuditTrailLevel.Debug, order.Id);
                                    continue;
                                }

                                var latestBTCEURRate = KrakenExchange.GetBTCEURRate();
                                order.Amount = order.BTCAmount * OpenExchangeRates.GetBTCExchangeRate(currency.Code, Rates, latestBTCEURRate[cryptoCurrency.Code].Value, cryptoCurrency.Code);
                                orderAmount = -(int)((order.Amount.Value * (1 - (Convert.ToDecimal(order.CommissionProcent) / 100))) * 100);
                            }
                            AuditLog.log("Yourpay CapturePayment Request: Order ExtRef:" + order.ExtRef + ", Order Amount Value:" + orderAmount,
                                (int)AuditLogStatus.YourPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                            var yourpayCapturePaymentResponse = new Retrier<YourpayCapturePaymentResponse>().Try(() => YourpayService.YourpayCapturePayment(order.ExtRef, orderAmount),
                                           Int32.Parse(retrierAppSettings.MaxRetries), Int32.Parse(retrierAppSettings.DelayInMilliseconds));

                            if (yourpayCapturePaymentResponse.Result == "1")
                            {
                                AuditLog.log("Yourpay CapturePayment Response: " + JsonConvert.SerializeObject(yourpayCapturePaymentResponse),
                                    (int)Data.Enums.AuditLogStatus.YourPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                var transaction = dataUnitOfWork.Transactions.Get(q => q.ExtRef == order.ExtRef && q.OrderId == order.Id).FirstOrDefault();
                                if (transaction != null)
                                    transaction.Completed = DateTime.Now;
                                else
                                    AuditLog.log("Unable to find Order Transaction record.", (int)Data.Enums.AuditLogStatus.ApplicationError,
                                        (int)Data.Enums.AuditTrailLevel.Debug, order.Id);

                                //For Buy orders, set order.Status as 'Released payment'
                                if (order.Type == (int)Buy)
                                    order.Status = (int)Data.Enums.OrderStatus.ReleasedPayment;
                                else //For Sell orders, set order.Status as 'Sent'
                                {
                                    order.Status = (int)Data.Enums.OrderStatus.Sent;
                                    dataUnitOfWork.Transactions.Add(new Transaction
                                    {
                                        Order = order,
                                        Amount = orderAmount,
                                        MethodId = 1, //BTC, Bitcoin payment
                                        Type = 2, // Outgoing
                                        ExtRef = order.ExtRef,
                                        CurrencyRef = order.Currency,
                                        FromAccount = 4, // 5960 - BitGo HotWallet BTC
                                        ToAccount = 1, // External
                                        Completed = DateTime.Now,
                                        Info = ""
                                    });
                                }
                            }
                            else
                            {
                                AuditLog.log("Yourpay CapturePayment Error Response: " + JsonConvert.SerializeObject(yourpayCapturePaymentResponse),
                                    (int)Data.Enums.AuditLogStatus.YourPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                //Set order.Status as 'Capture Errored'
                                order.Status = (int)Data.Enums.OrderStatus.CaptureErrored;
                            }
                        }
                        if (order.PaymentType == (int)OrderPaymentType.CreditCard && order.PaymentGatewayType == "TrustPay")
                        {
                            var trustPayCapturePaymentResponse = new Retrier<Payment>().Try(() => (new TrustPayService()).CapturePayment(currency.Code, String.Format("{0:.00}", order.Amount.Value), order.ExtRef, order.SiteId.Value),
                                                  Int32.Parse(retrierAppSettings.MaxRetries), Int32.Parse(retrierAppSettings.DelayInMilliseconds));

                            if (Regex.IsMatch(trustPayCapturePaymentResponse.result.code, "^000.000.|^000.100.1|^000.[36]|^000.400.0[^3]0|^000.400.100"))
                            {
                                AuditLog.log("TrustPay CapturePayment Response: " + JsonConvert.SerializeObject(trustPayCapturePaymentResponse),
                                   (int)Data.Enums.AuditLogStatus.TrustPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                //For Buy orders, set order.Status as 'Released payment'
                                if (order.Type == (int)Buy)
                                    order.Status = (int)Data.Enums.OrderStatus.ReleasedPayment;
                                else //For Sell orders, set order.Status as 'Sent'
                                {
                                    var method = dataUnitOfWork.TransactionMethods.Get(meth => meth.Text == cryptoCurrency.Code).FirstOrDefault();
                                    if (method == null)
                                    {
                                        AuditLog.log("Crypocurrency not fount in TransactionMethods Table for order Id " + order.Id + " crypotucurrency id " + order.CryptoCurrencyId, (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Info, orderId);
                                    }
                                    order.Status = (int)Data.Enums.OrderStatus.Sent;
                                    dataUnitOfWork.Transactions.Add(new Transaction
                                    {
                                        Order = order,
                                        Amount = order.Amount,
                                        MethodId = (method != null) ? method.Id : 3, //BTC, Bitcoin payment
                                        Type = 2, // Outgoing
                                        ExtRef = order.ExtRef,
                                        CurrencyRef = order.Currency,
                                        FromAccount = 4, // 5960 - BitGo HotWallet BTC
                                        ToAccount = 1, // External
                                        Completed = DateTime.Now,
                                        Info = ""
                                    });
                                }
                            }
                            else
                            {
                                AuditLog.log("TrustPay CapturePayment Error Response: " + JsonConvert.SerializeObject(trustPayCapturePaymentResponse),
                                    (int)Data.Enums.AuditLogStatus.TrustPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                //Set order.Status as 'Capture Errored'
                                order.Status = (int)Data.Enums.OrderStatus.CaptureErrored;
                            }
                        }
                        if (order.PaymentType == (int)OrderPaymentType.CreditCard && order.PaymentGatewayType == "QuickPay" && order.Type == (int)Buy)
                        {
                            //Set order.Status as 'Released payment' for Buy using QuickPayCreditCard
                            order.Status = (int)Data.Enums.OrderStatus.ReleasedPayment;
                        }
                        if (order.PaymentType == (int)OrderPaymentType.Bank)
                        {
                            if (order.Type == (int)Buy)
                                //Set order.Status as 'Released payment' for Buy using Bank
                                order.Status = (int)Data.Enums.OrderStatus.ReleasedPayment;
                            else if (order.Type == (int)Sell)
                            {
                                var transactionDetail = BitGoUtil.GetTransactionDetails(order.TransactionHash, cryptoCurrency.Code);
                                if (transactionDetail.Item3 > 1)
                                {
                                    #region OrderBook update
                                    var eurExchangeRate = OpenExchangeRates.GetEURExchangeRate(currency.Code, Rates);
                                    var dkkeurExchangeRate = OpenExchangeRates.GetEURExchangeRate("DKK", Rates);

                                    try
                                    {
                                        var orderAmount = order.Amount;
                                        if (currency.Code != "EUR")
                                            orderAmount = order.Amount / eurExchangeRate; // Convert order amount to EUR

                                        var btceurPrice = new Retrier<decimal>().Try(() => KrakenExchange.GetBTCEURPrice(orderAmount.Value, cryptoCurrency.Code), Int32.Parse(retrierAppSettings.MaxRetries), Int32.Parse(retrierAppSettings.DelayInMilliseconds));
                                        //var btceurPrice = KrakenExchange.GetBTCEURPrice(orderAmount.Value);
                                        order.RateBase = decimal.Round(btceurPrice, 8);
                                        order.RateHome = decimal.Round(eurExchangeRate, 8);
                                        order.RateBooks = decimal.Round(dkkeurExchangeRate, 8);

                                        if (order.Type == (int)Data.Enums.OrderType.Buy)
                                        {
                                            var oldRate = order.Rate;
                                            var newRate = decimal.Round(btceurPrice * eurExchangeRate, 8);
                                            order.Rate = newRate;
                                            AuditLog.log(string.Format("New rate updated from Kraken orderbook. Old Rate [{0}], New Rate [{1}].\r\nBTCEURPrice:{2}\r\nEurExchangeRate:{3}", oldRate, newRate, eurExchangeRate, btceurPrice),
                                                (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        //Set order.Status as 'Sending Aborted'
                                        order.Status = (int)Data.Enums.OrderStatus.SendingAborted;
                                        dataUnitOfWork.Orders.Update(order);
                                        AuditLog.log(string.Format("Error updating Rate from Kraken order book. OrderRate : {0}.\r\n{1}", order.Rate, ex),
                                            (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                                        continue;
                                    }
                                    #endregion

                                    var testOrProd = "";
                                    if (!site.Text.Contains("localhost"))
                                        if (site.Text.Split('.')[0].ToLower() == "apptest")
                                            testOrProd = "TEST: ";
                                    var cultureInfo = dataUnitOfWork.Countries.GetCultureCodeByCurrency(currency.Code);
                                    var ci = new CultureInfo(cultureInfo);
                                    PushoverHelper.SendNotification(testOrProd + (site.Text.Contains("localhost") ? site.Text : site.Text.Split('.')[1]) + ", SELL BANK ", order.Number + " " + order.Amount.Value.ToString("N0", ci) + " " + currency.Code + ", " + order.Name + ", " + order.CommissionProcent.Value.ToString("F") + "%" + ", " + (order.OurFee * dataUnitOfWork.Orders.GetOrderDiscount(order.Id)).Value.ToString("F") + "%" + ", " + order.FixedFee.Value.ToString("F") + "EUR");
                                    order.Status = (int)Data.Enums.OrderStatus.ReceivedCryptoPayment;
                                }
                            }
                        }
                        AuditLog.log("Updated Order(" + orderId + ") Status to " + Enum.GetName(typeof(Data.Enums.OrderStatus), order.Status) + " while Capturepayment() ", (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                        dataUnitOfWork.Orders.Update(order);
                        dataUnitOfWork.Commit();
                    }
                }
                catch (Exception ex)
                {
                    AuditLog.log("Error in CapturePayment() " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                }
            }
        }

        public void ProcessOrder()
        {
            var dataUnitOfWork1 = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            try
            {
                RetrierSettings retrierAppSettings = SettingsManager.GetDefault().Get("RetrierSettings").GetJsonData<RetrierSettings>();
                BitGoAccessSettings bitGoAccessSettings = SettingsManager.GetDefault().Get("BitGoAccessCode", true).GetJsonData<BitGoAccessSettings>();
                //                                                                        OrderStatus.ReleasedPayment
                var orderIds = dataUnitOfWork1.Orders.Get(q => q.Status == (int)Data.Enums.OrderStatus.ReleasedPayment).Select(q => q.Id).ToList();
                foreach (var orderId in orderIds)
                {
                    using (var dataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories())))
                    {
                        using (var orderLock = dataUnitOfWork.Orders.LockAndGet(orderId))
                        {
                            if (!orderLock.Usable) continue;
                            var order = orderLock.Domain;
                            try
                            {
                                var currency = dataUnitOfWork.Currencies.GetById(order.CurrencyId);
                                var cryptoCurrency = dataUnitOfWork.Currencies.Get(cur => cur.Id == order.CryptoCurrencyId).FirstOrDefault();
                                if (cryptoCurrency == null)
                                {
                                    AuditLog.log("Unable to find BTC record in Currency table in the database.",
                                        (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug, order.Id);
                                    continue;
                                }
                                // Stop payout if Outgoing transaction is already made.
                                var payoutTransaction = dataUnitOfWork.Transactions.Get(q => q.OrderId == orderId && q.Type == 2);
                                if (payoutTransaction.Count() > 0)
                                {
                                    order.Status = (int)Data.Enums.OrderStatus.ReleasingPaymentAborted;
                                    AuditLog.log("Order is already paid out.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug, order.Id);
                                    dataUnitOfWork.Orders.Update(order);
                                    dataUnitOfWork.Commit();
                                    continue;
                                }

                                // Stop payout if credit card reservation is already made for credit card orders.
                                if (order.PaymentType == 1)
                                {
                                    if (string.IsNullOrEmpty(order.ExtRef))
                                    {
                                        order.Status = (int)Data.Enums.OrderStatus.ReleasingPaymentAborted;
                                        AuditLog.log("No credit card reservation made for order.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug, order.Id);
                                        dataUnitOfWork.Orders.Update(order);
                                        dataUnitOfWork.Commit();
                                        continue;
                                    }
                                }

                                // Stop payout if credit card reservation amount does not matches amount of credit card orders.
                                if (order.PaymentType == 1 && string.IsNullOrEmpty(order.ExtRef))
                                {
                                    order.Status = (int)Data.Enums.OrderStatus.ReleasingPaymentAborted;
                                    AuditLog.log("Credit card reservation amount does not matches with order.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug, order.Id);
                                    dataUnitOfWork.Orders.Update(order);
                                    dataUnitOfWork.Commit();
                                    continue;
                                }


                                #region OrderBook update
                                var eurExchangeRate = OpenExchangeRates.GetEURExchangeRate(currency.Code, Rates);
                                var dkkeurExchangeRate = OpenExchangeRates.GetEURExchangeRate("DKK", Rates);

                                try
                                {
                                    var orderAmount = order.Amount;
                                    if (currency.Code != "EUR")
                                        orderAmount = order.Amount / eurExchangeRate; // Convert order amount to EUR

                                    var btceurPrice = new Retrier<decimal>().Try(() => KrakenExchange.GetBTCEURPrice(orderAmount.Value, cryptoCurrency.Code), Int32.Parse(retrierAppSettings.MaxRetries), Int32.Parse(retrierAppSettings.DelayInMilliseconds));
                                    var btceurPriceWithFactor = btceurPrice * buyFactorValue(System.Convert.ToDecimal(order.Spread), System.Convert.ToDecimal(order.FxMarkUp));
                                    var newRate = btceurPriceWithFactor * eurExchangeRate;  //Final rate with factor value and eurexchange rate
                                    order.RateBase = decimal.Round(btceurPrice, 8);
                                    order.RateHome = decimal.Round(eurExchangeRate, 8);
                                    order.RateBooks = decimal.Round(dkkeurExchangeRate, 8);
                                    var oldRate = order.Rate;
                                    order.Rate = newRate;
                                    // TODO: must be a method to calculate a formule
                                    var calculatedBTC =
                                                   (
                                                       (
                                                           (
                                                               order.Amount *
                                                                   (1 - (order.CommissionProcent / 100)
                                                                      - (order.OurFee / 100 * dataUnitOfWork.Orders.GetOrderDiscount(order.Id))
                                                                   )
                                                           ) / eurExchangeRate
                                                       ) / (newRate / eurExchangeRate)
                                                   ).Value;
                                    calculatedBTC -= (decimal)order.MinersFee; //Substract miners fee
                                    AuditLog.log(string.Format("New rate updated from Kraken orderbook. Old Rate [{0}], New Rate [{1}], Displayed Rate [{4}].\r\nBTCEURPrice:{3}\r\nEurExchangeRate:{2}", oldRate, newRate, eurExchangeRate, btceurPrice, (order.Amount - (order.MinersFee * newRate) - (order.Amount * order.OurFee / 100 * dataUnitOfWork.Orders.GetOrderDiscount(order.Id))) / calculatedBTC),
                                        (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                }
                                catch (Exception ex)
                                {
                                    // Set order.Status as 'Sending Aborted'
                                    order.Status = (int)Data.Enums.OrderStatus.SendingAborted;
                                    dataUnitOfWork.Orders.Update(order);
                                    AuditLog.log(string.Format("Error updating Rate from Kraken order book. OrderRate : {0}.\r\n{1}", order.Rate, ex),
                                        (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                                    dataUnitOfWork.Commit();
                                    continue;
                                }
                                #endregion

                                BitGoAccess bitgoClient;
                                if (!BitGoAccess.TryGetAndValidate(bitGoAccessSettings, cryptoCurrency.Code, out bitgoClient, order.Id))
                                {
                                    continue;
                                }

                                var cryptoCurrencyBitGoSettings = JsonConvert.DeserializeObject<BitGoCurrencySettings>(cryptoCurrency.BitgoSettings);
                                var wallet = (BitGoWallet)bitgoClient.GetWallet(cryptoCurrencyBitGoSettings.DefaultWalletId);

                                var amountInBTC = 0.0m;
                                long amountInSatoshi = 0;
                                var minersFee = 0.0m;
                                try
                                {
                                    minersFee = cryptoCurrencyBitGoSettings.MinersFee.fee;
                                    // TODO: must be a method to calculate a formule
                                    amountInBTC =
                                                    (
                                                        (
                                                            (
                                                                order.Amount *
                                                                    (1 - (order.CommissionProcent / 100)
                                                                       - (order.OurFee / 100 * dataUnitOfWork.Orders.GetOrderDiscount(order.Id))
                                                                    )
                                                            ) / eurExchangeRate
                                                        ) / (order.Rate / eurExchangeRate)
                                                    ).Value;
                                    var amountInBTCOld = amountInBTC; //Copy amount in BTC
                                    amountInBTC -= minersFee; //Substract miners fee
                                    amountInSatoshi = (long)((double)amountInBTC * cryptoCurrencyBitGoSettings.TxUnit);  // Convert order amount to EUR & convert order rate to EUR then convert to BTC
                                    AuditLog.log("Old amountInBTC:" + amountInBTCOld + "New amountInBTC:" + amountInBTC + "Fee:" + minersFee + "Order Amount: " + order.Amount + ", Amount after Commission:" + (order.Amount * (1 - (order.CommissionProcent / 100))) + "Amount after OurFee:" + (order.Amount * (1 - (order.OurFee / 100))) + "Amount after discount:" + (order.Amount * (1 - (order.CommissionProcent / 100) - (order.OurFee / 100 * dataUnitOfWork.Orders.GetOrderDiscount(order.Id)))) + " .", (int)Data.Enums.AuditLogStatus.BitGo, (int)Data.Enums.AuditTrailLevel.Debug, order.Id);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Error in calculating fee.", ex);
                                }

                                if (wallet.balance < amountInSatoshi)
                                {
                                    order.Status = (int)Data.Enums.OrderStatus.SendingAborted;
                                    dataUnitOfWork.Orders.Update(order);
                                    string balanceString = string.Format("b:{0},bs:{1},cb:{2},cbs:{3},sb:{4},sbs:{5},", wallet.balance, wallet.balanceString, wallet.confirmedBalance, wallet.confirmedBalanceString, wallet.spendableBalance, wallet.spendableBalanceString);
                                    AuditLog.log("Insufficient balance for sending BTC. Current wallet(" + cryptoCurrencyBitGoSettings.DefaultWalletId.ToString() + ") balance is " +
                                        wallet.balance + ". Amount in BTC:" + amountInBTC + ". Amount in Satoshi:" + amountInSatoshi + ". BanalnceString:" + balanceString,
                                        (int)Data.Enums.AuditLogStatus.BitGo, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                    dataUnitOfWork.Commit();
                                    continue;
                                }

                                QuickPayService quickPayService = null;
                                var isQuickPayCC = order.PaymentType == (int)OrderPaymentType.CreditCard && order.PaymentGatewayType == "QuickPay" && order.Type == (int)Buy;
                                if (isQuickPayCC)
                                {
                                    quickPayService = new QuickPayService((long)order.SiteId);
                                    var getPayment = quickPayService.GetPayment(order.ExtRef);
                                    QuickPayResponse getPaymentContent = JsonConvert.DeserializeObject<QuickPayResponse>(getPayment.Content);
                                    var check = getPaymentContent.Operations.Any(x => x.Type == "capture" || x.Type == "cancel");
                                    if (check)
                                    {
                                        order.Status = (int)Data.Enums.OrderStatus.SendingAborted;
                                        AuditLog.log("Order is already captured or cancelled. Updated Order(" + orderId + ") status to " + Enum.GetName(typeof(Data.Enums.OrderStatus), order.Status) + " while Processing order.", (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                                        dataUnitOfWork.Commit();
                                        continue;
                                    }
                                }

                                try
                                {
                                    if (!bitgoClient.RefreshSession(order.Id))
                                    {
                                        continue;
                                    }

                                    var sendBTCResponse = bitgoClient.SendCoins(wallet.id, order.CryptoAddress, amountInSatoshi, cryptoCurrencyBitGoSettings.PassPhrase.ToString(), 0);
                                    if (sendBTCResponse.StatusCode == System.Net.HttpStatusCode.OK)
                                    {
                                        AuditLog.log("SendBTC Response. Response Content: " + sendBTCResponse.Content,
                                            (int)Data.Enums.AuditLogStatus.BitGo, (int)Data.Enums.AuditTrailLevel.Debug, order.Id);
                                        //Check below only for creditcard and quickpay payments
                                        if (isQuickPayCC)
                                        {
                                            //capture quickpay payment 
                                            long orderAmount = Convert.ToInt64(order.Amount * 100);
                                            long paymentId = Convert.ToInt64(order.ExtRef);
                                            var captureCCPayment = quickPayService.CapturePayment(paymentId, orderAmount);
                                            AuditLog.log("QuickPay CapturePayment Request: Order ExtRef:" + order.ExtRef + ", Order Amount Value:" + orderAmount,
                                              (int)AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                            QuickPayResponse capturePaymentContent = JsonConvert.DeserializeObject<QuickPayResponse>(captureCCPayment.Content);
                                            bool? captureAccepted = capturePaymentContent.Accepted;

                                            //Check captureAccepted if true then proceed further checks else CaptureErrored 
                                            if (captureAccepted.GetValueOrDefault())
                                            {
                                                string strPaymentId = order.ExtRef.ToString();
                                                var getPaymentContent = new Retrier<QuickPayResponse>().Try(() =>
                                                    {
                                                        var getPayment2 = quickPayService.GetPayment(strPaymentId);
                                                        QuickPayResponse getPaymentContent2 = JsonConvert.DeserializeObject<QuickPayResponse>(getPayment2.Content);
                                                        var paymentAccepted2 = getPaymentContent2.Accepted;
                                                        var authorizedStatusCodes2 = getPaymentContent2.Operations.Where(x => x.Type == "capture").Select(x => new { x.AqStatusCode, x.QpStatusCode }).LastOrDefault();
                                                        if (authorizedStatusCodes2 == null)
                                                        {
                                                            throw new Exception(); // retry
                                                        }

                                                        string qpStatusCode2 = authorizedStatusCodes2.QpStatusCode;
                                                        string aqStatusCode2 = authorizedStatusCodes2.AqStatusCode;
                                                        if (qpStatusCode2 == null || aqStatusCode2 == null)
                                                        {
                                                            throw new Exception(); // retry
                                                        }

                                                        //if (paymentAccepted2.GetValueOrDefault() && qpStatusCode2 == "20000" && aqStatusCode2 == "20000")
                                                        //{
                                                        return getPaymentContent2;
                                                        //}
                                                        //else
                                                        //{
                                                        //}
                                                    }
                                                    , int.Parse(retrierAppSettings.MaxRetries), int.Parse(retrierAppSettings.DelayInMilliseconds));

                                                // QuickPayResponse getPaymentContent = JsonConvert.DeserializeObject<QuickPayResponse>(getPayment.Content);
                                                if (getPaymentContent != null)
                                                {
                                                    var paymentAccepted = getPaymentContent.Accepted;
                                                    var authorizedStatusCodes = getPaymentContent.Operations.Where(x => x.Type == "capture").Select(x => new { x.AqStatusCode, x.QpStatusCode }).LastOrDefault();
                                                    string qpStatusCode = authorizedStatusCodes.QpStatusCode;
                                                    string aqStatusCode = authorizedStatusCodes.AqStatusCode;

                                                    if (paymentAccepted.GetValueOrDefault() && qpStatusCode == "20000" && aqStatusCode == "20000")
                                                    {
                                                        AuditLog.log("QuickPay CapturePayment Response content: " + JsonConvert.SerializeObject(getPaymentContent),
                                                       (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                                        //any nothing todo?
                                                        //update the incoming transaction with completed as current datetime only after order status moved to Completed
                                                        //var getTx = dataUnitOfWork.Transactions.Get(x => x.OrderId == order.Id && x.Type == 1).FirstOrDefault();
                                                        //getTx.Completed = DateTime.Now;
                                                        //dataUnitOfWork.Commit();
                                                    }
                                                    else
                                                    {
                                                        AuditLog.log("QuickPay CapturePayment Error Response Content: " + JsonConvert.SerializeObject(capturePaymentContent),
                                                                  (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                                        //Set order.Status as 'Capture Errored'
                                                        order.Status = (int)Data.Enums.OrderStatus.CaptureErrored;
                                                        dataUnitOfWork.Commit();
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    AuditLog.log("QuickPay CapturePayment Error Response Content: " + JsonConvert.SerializeObject(capturePaymentContent),
                                                                  (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                                    //Set order.Status as 'Capture Errored'
                                                    order.Status = (int)Data.Enums.OrderStatus.CaptureErrored;
                                                    dataUnitOfWork.Commit();
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                AuditLog.log("BTC sent but QuickPay CapturePayment Failed Response Content: " + JsonConvert.SerializeObject(capturePaymentContent),
                                                              (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                                //Set order.Status as 'Capture Errored'
                                                order.Status = (int)Data.Enums.OrderStatus.CaptureErrored;
                                                dataUnitOfWork.Commit();
                                                continue;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        AuditLog.log("Error sending BTC. Request Content: " +
                                            "EurExchangeRate:" + eurExchangeRate + " " +
                                            "order.Amount:" + order.Amount + " " +
                                            "order.CommissionProcent:" + order.CommissionProcent + " " +
                                            "order.Rate:" + order.Rate + " " +
                                            "amountInBTC:" + amountInBTC + " " +
                                            "amountInSatoshi:" + amountInSatoshi + " " +
                                            "Response Content: " + JsonConvert.SerializeObject(sendBTCResponse),
                                            (int)Data.Enums.AuditLogStatus.BitGo, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                                        order.Status = (int)Data.Enums.OrderStatus.SendingAborted;
                                        AuditLog.log("Updated Order(" + orderId + ") status to " + Enum.GetName(typeof(Data.Enums.OrderStatus), order.Status) + " while sending btc.", (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                                        dataUnitOfWork.Commit();
                                        continue;
                                    }

                                    //var btcCurrency = DataUnitOfWork.Currencies.Get(q => q.Code == "BTC").FirstOrDefault();
                                    dynamic sendBTCResponseContent = JsonConvert.DeserializeObject(sendBTCResponse.Content);
                                    int bitGoWalletDetection = sendBTCResponseContent.transfer.value;
                                    int bitGoWalletMinerFees = Convert.ToInt32(sendBTCResponseContent.transfer.feeString);
                                    decimal absoluteBitGoWalletDetection = Convert.ToDecimal((Math.Abs(bitGoWalletDetection))) / 100000000;
                                    decimal minerFees = Convert.ToDecimal(bitGoWalletMinerFees) / 100000000; //todo 100000000 move to constant
                                    int bitGoPayAsYouGo = sendBTCResponseContent.transfer.payGoFee;
                                    decimal absoluteBitGoPayAsYouGo = Convert.ToDecimal(Math.Abs(bitGoPayAsYouGo)) / 100000000;
                                    string uuid = CommonUUID.UUID();
                                    //Need to split  into  4 transactions to obtain in transaction list
                                    //1.Transaction for BTC deducted from our Bitgo wallet 
                                    // TODO: can all transaction entry to be in repository - and more organised? - there are only specific transactions which all must be sperate methods respectively
                                    dataUnitOfWork.Transactions.Add(new Transaction
                                    {
                                        Order = order,
                                        Amount = absoluteBitGoWalletDetection,
                                        MethodId = 3, //BTC( Bitcoin payment)
                                        Type = 2, // Outgoing
                                        ExtRef = sendBTCResponseContent.txid,
                                        CurrencyRef = cryptoCurrency,
                                        FromAccount = Accounting.AccountingUtil.GetAccountId(order.Type, order.CryptoCurrencyId, AccountValueFor.FromAccount, 2, ParticularType.NonFee),
                                        ToAccount = null,
                                        // Completed = DateTime.Now,
                                        Info = "",
                                        BatchNumber = uuid
                                    });

                                    //2.Transaction for Customer received 
                                    dataUnitOfWork.Transactions.Add(new Transaction
                                    {
                                        Order = order,
                                        Amount = amountInBTC,
                                        MethodId = 3, //BTC(Bitcoin payment) 
                                        Type = 2, // Outgoing
                                        ExtRef = sendBTCResponseContent.txid,
                                        CurrencyRef = cryptoCurrency,
                                        FromAccount = null,
                                        ToAccount = Accounting.AccountingUtil.GetAccountId(order.Type, order.CryptoCurrencyId, AccountValueFor.ToAccount, 2, ParticularType.NonFee),
                                        // Completed = DateTime.Now,
                                        Info = "",
                                        BatchNumber = uuid
                                    });

                                    //3.Transaction for MinerFee 
                                    dataUnitOfWork.Transactions.Add(new Transaction
                                    {
                                        Order = order,
                                        Amount = minerFees,
                                        MethodId = 3, //BTC(Bitcoin payment)
                                        Type = 2, // Outgoing
                                        ExtRef = sendBTCResponseContent.txid,
                                        CurrencyRef = cryptoCurrency,
                                        FromAccount = null,
                                        ToAccount = Accounting.AccountingUtil.GetAccountId(order.Type, order.CryptoCurrencyId, AccountValueFor.ToAccount, 2, ParticularType.MinersFee),
                                        //   Completed = DateTime.Now,
                                        Info = "",
                                        BatchNumber = uuid
                                    });

                                    //4.Transaction for Bitgo PayAsYouGoFee
                                    dataUnitOfWork.Transactions.Add(new Transaction
                                    {
                                        Order = order,
                                        Amount = absoluteBitGoPayAsYouGo,
                                        MethodId = 3, //BTC(Bitcoin payment)
                                        Type = 2, // Outgoing
                                        ExtRef = sendBTCResponseContent.txid,
                                        CurrencyRef = cryptoCurrency,
                                        FromAccount = null,
                                        ToAccount = Accounting.AccountingUtil.GetAccountId(order.Type, order.CryptoCurrencyId, AccountValueFor.ToAccount, 2, ParticularType.BitGoFee),
                                        // Completed = DateTime.Now,
                                        Info = "",
                                        BatchNumber = uuid
                                    });

                                    decimal payoutFee = 1;
                                    if (sendBTCResponseContent.feeString != null && decimal.TryParse(sendBTCResponseContent.feeString, out payoutFee))
                                    {
                                        minersFee = payoutFee / cryptoCurrencyBitGoSettings.TxUnit;
                                    }

                                    order.MinersFee = minersFee;
                                    order.TransactionHash = sendBTCResponseContent.txid;
                                    order.BTCAmount = decimal.Round(amountInBTC, 8);
                                    order.Status = (int)Data.Enums.OrderStatus.Sent; //Set order.Status as 'Sent'                
                                                                                     //trying to set and we are getting {"Order is locked for modification"}
                                }
                                catch (Exception ex)
                                {
                                    AuditLog.log("Debugger2 " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                                    order.Status = (int)Data.Enums.OrderStatus.SendingAborted;
                                }
                            }
                            catch (Exception ex)
                            {
                                //Set order.Status as 'Sending Aborted'
                                order.Status = (int)Data.Enums.OrderStatus.SendingAborted;
                                AuditLog.log("Debugger3 " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                            }

                            dataUnitOfWork.Orders.Update(order);
                            //AuditLog.log("Updated Order(" + orderId + ") in ProcessOrder() " + JsonConvert.SerializeObject(order), (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                            dataUnitOfWork.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in ProcessOrder(): " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        public async void NotifyOrderComplete()
        {
            var dataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            try
            {
                var orderIds = dataUnitOfWork.Orders.Get(q => q.Status == (int)Data.Enums.OrderStatus.Sent).Select(q => q.Id).ToList();
                foreach (var orderId in orderIds)
                {
                    var order = dataUnitOfWork.Orders.GetById(orderId);
                    var user = dataUnitOfWork.Users.GetById(order.UserId);
                    var site = dataUnitOfWork.Sites.GetById(order.SiteId.Value);

                    var testOrProd = "";
                    //Appending "TEST:" to Pushover if payment type is Credit card
                    if (order.PaymentType == 1)
                        if (!site.Text.Contains("localhost"))
                            if (site.Text.Split('.')[0].ToLower() == "apptest")
                                testOrProd = "TEST: ";
                    var currency = dataUnitOfWork.Currencies.GetById(order.CurrencyId);
                    var cryptoCurrency = dataUnitOfWork.Currencies.GetById(order.CryptoCurrencyId);
                    var cultureInfo = dataUnitOfWork.Countries.GetCultureCodeByCurrency(currency.Code);
                    var ci = new CultureInfo(cultureInfo);

                    PushoverHelper.SendNotification(testOrProd + (site.Text.Contains("localhost") ? site.Text : site.Text.Split('.')[1]) + ", payout", order.Number + " " + order.Amount.Value.ToString("N0", ci) + " " + currency.Code + ", " + order.Name + ", " + order.CommissionProcent.Value.ToString("F") + "%" + order.OurFee.Value.ToString("F") + "%");
                    var currentSite = dataUnitOfWork.Sites.Get(q => q.Url == site.Url).FirstOrDefault();
                    var customCss = string.Empty;
                    var strCss = @"<style type=""text/css"">";
                    if (string.IsNullOrEmpty(order.CardNumber))
                    {
                        strCss += @".hideRow1";
                        strCss += @"{";
                        strCss += @"display: none ";
                        strCss += @"}";

                    }
                    if (order.OurFee.Value == 0)
                    {

                        strCss += @".hideRow2";
                        strCss += @"{";
                        strCss += @"display: none ";
                        strCss += @"}";
                    }
                    if (order.CommissionProcent.Value == 0)
                    {
                        strCss += @".hideRow3";
                        strCss += @"{";
                        strCss += @"display: none ";
                        strCss += @"}";

                    }
                    strCss += @"</style>";
                    customCss += strCss;
                    EmailHelper.SendEmail(user.Email, (order.Type == (int)Data.Enums.OrderType.Sell ? order.PaymentType == 1 ? "SellOrderCompleted" : "OrderSellCompleted" : "OrderCompleted"), ///Pending SellOrderCompleted
                        new Dictionary<string, object>
                        {
                            { "UserFirstName", user.Fname },
                            { "OrderNumber", order.Number },
                            { "OrderAmount", order.Type == (int)Data.Enums.OrderType.Buy ? order.Amount.Value.ToString("N2", ci) : (order.Amount.Value * (1 - (Convert.ToDecimal(order.OurFee) / 100))).ToString("N2", ci)}, // ;
                            { "TransactionExtRef", order.TransactionHash },
                            { "OrderCurrency", currency.Code },
                            { "CardNumber", order.PaymentType == 1 ? order.CardNumber : "" }, // if paymenttype is credit card set the card number
                            { "OrderCommission", (order.Amount * (order.CommissionProcent / 100)).Value.ToString("N2", ci) },
                            { "OrderOurFee", (order.Amount / 100 * order.OurFee * dataUnitOfWork.Orders.GetOrderDiscount(order.Id)).Value.ToString("N2", ci) },
                            { "OurFee", (order.OurFee.Value.ToString("N2", ci) + "%")},
                            { "CardFee",order.CommissionProcent.Value.ToString("N2", ci) + "%" },
                            { "OrderRate", order.Rate.Value.ToString("N2", ci) },
                            { "CryptoAddress", order.CryptoAddress },
                            { "TxAmount", order.BTCAmount.Value.ToString("N8", ci) },
                            { "BccTrustPilotAddress", currentSite.TrustPilotAddress},
                            { "MinersFee", (order.MinersFee.HasValue ? (order.MinersFee.Value * order.Rate.Value) : 0M).ToString("N2", ci) + " " + currency.Code}
                        }, site.Text, order.BccAddress, null, customCss);

                    //Set order.Status as 'Completed'
                    order.Status = (int)Data.Enums.OrderStatus.Completed;
                    dataUnitOfWork.Orders.Update(order);

                    //Here order status moved to completed update both incoming and outing tx completed value with current datetime
                    if (order.Type == (int)Buy)
                    {
                        var currentDateTime = DateTime.Now;
                        //update incoming tx
                        var inTx = dataUnitOfWork.Transactions.Get(x => x.OrderId == order.Id && x.Type == 1).FirstOrDefault();
                        inTx.Completed = currentDateTime;
                        //update outgoing tx
                        var account1 = Accounting.AccountingUtil.GetAccountId(order.Type, order.CryptoCurrencyId, AccountValueFor.FromAccount, 2, ParticularType.NonFee);
                        var account2 = Accounting.AccountingUtil.GetAccountId(order.Type, order.CryptoCurrencyId, AccountValueFor.ToAccount, 2, ParticularType.NonFee);
                        var account3 = Accounting.AccountingUtil.GetAccountId(order.Type, order.CryptoCurrencyId, AccountValueFor.ToAccount, 2, ParticularType.MinersFee);
                        var account4 = Accounting.AccountingUtil.GetAccountId(order.Type, order.CryptoCurrencyId, AccountValueFor.ToAccount, 2, ParticularType.BitGoFee);
                        var outTx1 = dataUnitOfWork.Transactions.Get(x => x.OrderId == order.Id && x.Type == 2 && x.FromAccount == account1).FirstOrDefault();
                        var outTx2 = dataUnitOfWork.Transactions.Get(x => x.OrderId == order.Id && x.Type == 2 && x.ToAccount == account2).FirstOrDefault();
                        var outTx3 = dataUnitOfWork.Transactions.Get(x => x.OrderId == order.Id && x.Type == 2 && x.ToAccount == account3).FirstOrDefault();
                        var outTx4 = dataUnitOfWork.Transactions.Get(x => x.OrderId == order.Id && x.Type == 2 && x.ToAccount == account4).FirstOrDefault();
                        outTx1.Completed = currentDateTime;
                        outTx2.Completed = currentDateTime;
                        outTx3.Completed = currentDateTime;
                        outTx4.Completed = currentDateTime;
                        dataUnitOfWork.Commit();
                        //var orderProcess = new OrderProcessor();

                        //OrderStatusChangedMessageBroker.OrderStateChanged
                        //OrderStatusChangedMessageBroker.OrderState("NotifyOrder", new OrderStateChangedArgs { OrderId = order.Id, Status = "Completed" });
                        //orderProcess.Completed(order.Id);
                        //call the MerchantWebhook if referenceId and merchantCode are not null
                        //new method
                        await PublishOrder(order.Id);
                    }

                    try
                    {
                        ChainalysisInterface.PostOutputs(order.UserId.ToString(), string.Format("{0}:{1}", order.TransactionHash, order.CryptoAddress));
                    }
                    catch (Exception ex)
                    {
                        AuditLog.log(string.Format("Unable send output from Chainalysis for userid:{0}.\r\nError: {1}", order.UserId, ex),
                            (int)Data.Enums.AuditLogStatus.Chainalysis, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
                    }
                    dataUnitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in NotifyOrderComplete() " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        public async void ProcessKYCDecline()
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            try
            {
                RetrierSettings retrierAppSettings = SettingsManager.GetDefault().Get("RetrierSettings").GetJsonData<RetrierSettings>();
                if (retrierAppSettings == null)
                    throw new Exception("Unable to find 'RetrierSettings' key in AppSettings.");

                //KYC Decline
                var orderIds = DataUnitOfWork.Orders.Get(q => q.Status == (int)Data.Enums.OrderStatus.KYCDecline).Select(q => q.Id).ToList();
                foreach (var orderId in orderIds)
                {
                    var order = DataUnitOfWork.Orders.GetById(orderId);
                    // Do not do release payment if the order is paidout.
                    var payoutTransaction = DataUnitOfWork.Transactions.Get(q => q.OrderId == orderId && q.Type == 2);
                    if (payoutTransaction.Count() > 0)
                    {
                        AuditLog.log("KYC decline cannot be processed because the order is already paid out.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug, order.Id);
                        continue;
                    }
                    if (order.PaymentType == 1) // if order is using credit card payment
                    {
                        switch (order.PaymentGatewayType)
                        {
                            case "PayLike":
                                {
                                    dynamic payLikeDetails = PayLikeService.GetPayLikeDetails(order.SiteId.Value, order.CurrencyId);
                                    var paylikeTransactionService = new PaylikeTransactionService(payLikeDetails.AppKey.ToString());
                                    var voidTransaction = paylikeTransactionService.VoidTransaction(new VoidTransactionRequest
                                    {
                                        TransactionId = order.ExtRef,
                                        Amount = (int)Convert.ToDouble((order.Amount * 100).ToString()),
                                    });
                                    if (!voidTransaction.IsError)
                                    {
                                        AuditLog.log("Paylike ReleasePayment Response: " + JsonConvert.SerializeObject(voidTransaction),
                                          (int)Data.Enums.AuditLogStatus.PayLike, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                        //Set order.Status as 'KYC Declined'
                                        order.Status = (int)Data.Enums.OrderStatus.KYCDeclined;
                                        EmailOrder(order, "KYCDeclined");
                                    }
                                    else
                                    {
                                        AuditLog.log("Paylike ReleasePayment Error Response: " + JsonConvert.SerializeObject(voidTransaction),
                                            (int)Data.Enums.AuditLogStatus.PayLike, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                        //Set order.Status as 'Release Errored'
                                        order.Status = (int)Data.Enums.OrderStatus.ReleaseErrored;
                                    }
                                    break;
                                }
                            case "YourPay":
                                {
                                    var yourpayReleasePaymentResponse = new Retrier<YourpayReleasePaymentResponse>().Try(() => YourpayService.YourpayReleasePayment(order.ExtRef, (int)(order.Amount.Value * 100), order.SiteId.Value),
                                              Int32.Parse(retrierAppSettings.MaxRetries), Int32.Parse(retrierAppSettings.DelayInMilliseconds));
                                    if (yourpayReleasePaymentResponse.Result == "1")
                                    {
                                        AuditLog.log("Yourpay ReleasePayment Response: " + JsonConvert.SerializeObject(yourpayReleasePaymentResponse),
                                            (int)Data.Enums.AuditLogStatus.YourPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                        //Set order.Status as 'KYC Declined'
                                        order.Status = (int)Data.Enums.OrderStatus.KYCDeclined;
                                        EmailOrder(order, "KYCDeclined");
                                    }
                                    else
                                    {
                                        AuditLog.log("Yourpay ReleasePayment Error Response: " + JsonConvert.SerializeObject(yourpayReleasePaymentResponse),
                                            (int)Data.Enums.AuditLogStatus.YourPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                        //Set order.Status as 'Release Errored'
                                        order.Status = (int)Data.Enums.OrderStatus.ReleaseErrored;
                                    }
                                    break;
                                }
                            case "TrustPay":
                                {
                                    var trustPayReleasePaymentResponse = new Retrier<Payment>().Try(() => (new TrustPayService()).ReversalPayment(order.ExtRef, order.SiteId.Value),
                                               Int32.Parse(retrierAppSettings.MaxRetries), Int32.Parse(retrierAppSettings.DelayInMilliseconds));
                                    if (Regex.IsMatch(trustPayReleasePaymentResponse.result.code, "^000.000.|^000.100.1|^000.[36]|^000.400.0[^3]0|^000.400.100"))
                                    {
                                        AuditLog.log("TrustPay ReleasePayment Response: " + JsonConvert.SerializeObject(trustPayReleasePaymentResponse),
                                           (int)Data.Enums.AuditLogStatus.TrustPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                        //Set order.Status as 'KYC Declined'
                                        order.Status = (int)Data.Enums.OrderStatus.KYCDeclined;
                                        EmailOrder(order, "KYCDeclined");
                                    }
                                    else
                                    {
                                        AuditLog.log("TrustPay ReleasePayment Error Response: " + JsonConvert.SerializeObject(trustPayReleasePaymentResponse),
                                            (int)Data.Enums.AuditLogStatus.TrustPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                        //Set order.Status as 'Release Errored'
                                        order.Status = (int)Data.Enums.OrderStatus.ReleaseErrored;
                                    }
                                    break;
                                }
                            case "QuickPay":
                                {
                                    // TODO: better to be moved to a method (also each type above)
                                    var quickPayService = new QuickPayService(Convert.ToInt64(order.SiteId));
                                    long strPaymentId = Convert.ToInt64(order.ExtRef);
                                    var cancelPayment = quickPayService.CancelPayment(strPaymentId);
                                    QuickPayResponse cancelPaymentContent = JsonConvert.DeserializeObject<QuickPayResponse>(cancelPayment.Content);
                                    bool? cancelAccepted = cancelPaymentContent.Accepted;
                                    if (cancelAccepted.GetValueOrDefault())
                                    {
                                        // var getPayment = quickPayService.GetPayment(strPaymentId.ToString());
                                        // QuickPayResponse getPaymentContent = JsonConvert.DeserializeObject<QuickPayResponse>(getPayment.Content);
                                        var getPaymentContent = new Retrier<QuickPayResponse>().Try(() =>
                                        {
                                            var getPayment2 = quickPayService.GetPayment(strPaymentId.ToString());
                                            QuickPayResponse getPaymentContent2 = JsonConvert.DeserializeObject<QuickPayResponse>(getPayment2.Content);
                                            var paymentAccepted2 = getPaymentContent2.Accepted;
                                            var authorizedStatusCodes2 = getPaymentContent2.Operations.Where(x => x.Type == "cancel").Select(x => new { x.AqStatusCode, x.QpStatusCode }).LastOrDefault();
                                            if (authorizedStatusCodes2 == null)
                                            {
                                                throw new Exception(); // retry
                                            }

                                            string qpStatusCode2 = authorizedStatusCodes2.QpStatusCode;
                                            string aqStatusCode2 = authorizedStatusCodes2.AqStatusCode;
                                            if (qpStatusCode2 == null || aqStatusCode2 == null)
                                            {
                                                throw new Exception(); // retry
                                            }

                                            //if (paymentAccepted2.GetValueOrDefault() && qpStatusCode2 == "20000" && aqStatusCode2 == "20000")
                                            //{
                                            return getPaymentContent2;
                                            //}
                                            //else
                                            //{
                                            //}
                                        }
                                        , int.Parse(retrierAppSettings.MaxRetries), int.Parse(retrierAppSettings.DelayInMilliseconds));

                                        if (getPaymentContent != null)
                                        {
                                            bool? paymentAccepted = getPaymentContent.Accepted;
                                            var authorizedStatusCodes = getPaymentContent.Operations.Where(x => x.Type == "cancel").Select(x => new { x.QpStatusCode, x.AqStatusCode }).LastOrDefault();
                                            string qpStatusCode = authorizedStatusCodes.QpStatusCode;
                                            string aqStatusCode = authorizedStatusCodes.AqStatusCode;

                                            if (paymentAccepted.GetValueOrDefault() && qpStatusCode == "20000" && aqStatusCode == "20000")
                                            {
                                                AuditLog.log("QuickPay ReleasePayment Response: " + JsonConvert.SerializeObject(getPaymentContent),
                                              (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                                //Set order.Status as 'KYC Declined'
                                                order.Status = (int)Data.Enums.OrderStatus.KYCDeclined;
                                                EmailOrder(order, "KYCDeclined");
                                            }
                                            else
                                            {
                                                AuditLog.log("QuickPay ReleasePayment Error Response: " + JsonConvert.SerializeObject(getPaymentContent),
                                              (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                                //Set order.Status as 'Release Errored'
                                                order.Status = (int)Data.Enums.OrderStatus.ReleaseErrored;
                                            }
                                        }
                                        else
                                        {
                                            AuditLog.log("QuickPay ReleasePayment Error Response: " + JsonConvert.SerializeObject(getPaymentContent),
                                          (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                            //Set order.Status as 'Release Errored'
                                            order.Status = (int)Data.Enums.OrderStatus.ReleaseErrored;
                                        }
                                    }
                                    else
                                    {
                                        AuditLog.log("QuickPay ReleasePayment Error Response: " + JsonConvert.SerializeObject(cancelPaymentContent),
                                           (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                        //Set order.Status as 'Release Errored'
                                        order.Status = (int)Data.Enums.OrderStatus.ReleaseErrored;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else if (order.PaymentType == 2)
                    {
                        //Set order.Status as 'KYC Declined'
                        order.Status = (int)Data.Enums.OrderStatus.KYCDeclined;
                    }
                    AuditLog.log("Updated Order(" + orderId + ") status to " + Enum.GetName(typeof(Data.Enums.OrderStatus), order.Status) + " while Processing KYCDecline.", (int)Data.Enums.AuditLogStatus.IPInfo, (int)Data.Enums.AuditTrailLevel.Info, orderId);
                    DataUnitOfWork.Orders.Update(order);
                    DataUnitOfWork.Commit();
                    //call the MerchantWebhook if referenceId and merchantCode are not null
                    await PublishOrder(order.Id);
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in ProcessKYCDecline() " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        public async void ProcessCancelledOrders()
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            try
            {
                RetrierSettings retrierAppSettings = SettingsManager.GetDefault().Get("RetrierSettings").GetJsonData<RetrierSettings>();
                if (retrierAppSettings == null)
                    throw new Exception("Unable to find 'RetrierSettings' key in AppSettings.");

                var orderIds = DataUnitOfWork.Orders.Get(q => q.Status == (int)Data.Enums.OrderStatus.Cancel).Select(q => q.Id).ToList();
                foreach (var orderId in orderIds)
                {
                    var order = DataUnitOfWork.Orders.GetById(orderId);
                    // Do not do release payment if the order is paidout.
                    var payoutTransaction = DataUnitOfWork.Transactions.Get(q => q.OrderId == orderId && q.Type == 2);
                    if (payoutTransaction.Count() > 0)
                    {
                        AuditLog.log("Order cannot be canceled because the order is already paid out.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug, order.Id);
                        continue;
                    }
                    if (order.PaymentType == 1) // if order is using credit card payment
                    {
                        switch (order.PaymentGatewayType)
                        {
                            case "PayLike":
                                {
                                    dynamic payLikeDetails = PayLikeService.GetPayLikeDetails(order.SiteId.Value, order.CurrencyId);
                                    var paylikeTransactionService = new PaylikeTransactionService(payLikeDetails.AppKey.ToString());
                                    //var paylikeTransactionService = new PaylikeTransactionService(payLikeDetails.PayLikeAppKey.ToString());
                                    var voidTransaction = paylikeTransactionService.VoidTransaction(new VoidTransactionRequest
                                    {
                                        TransactionId = order.ExtRef,
                                        Amount = (int)Convert.ToDouble((order.Amount * 100).ToString()),
                                    });
                                    if (!voidTransaction.IsError)
                                    {
                                        AuditLog.log("Paylike ReleasePayment Response: " + JsonConvert.SerializeObject(voidTransaction),
                                            (int)Data.Enums.AuditLogStatus.PayLike, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                        order.Status = (int)WMC.Data.Enums.OrderStatus.OrderCancelled;
                                        EmailOrder(order, "OrderCancelled");
                                    }
                                    else
                                    {
                                        AuditLog.log("Paylike ReleasePayment Error Response: " + JsonConvert.SerializeObject(voidTransaction),
                                            (int)Data.Enums.AuditLogStatus.PayLike, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                        order.Status = (int)WMC.Data.Enums.OrderStatus.ReleaseErrored;
                                    }
                                }
                                break;
                            case "YourPay":
                                {
                                    var yourpayReleasePaymentResponse = new Retrier<YourpayReleasePaymentResponse>().Try(() => YourpayService.YourpayReleasePayment(order.ExtRef, (int)(order.Amount.Value * 100), order.SiteId.Value),
                                               Int32.Parse(retrierAppSettings.MaxRetries), Int32.Parse(retrierAppSettings.DelayInMilliseconds));
                                    if (yourpayReleasePaymentResponse.Result == "1")
                                    {
                                        AuditLog.log("Yourpay ReleasePayment Response: " + JsonConvert.SerializeObject(yourpayReleasePaymentResponse),
                                            (int)Data.Enums.AuditLogStatus.YourPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                        order.Status = (int)WMC.Data.Enums.OrderStatus.OrderCancelled;
                                        EmailOrder(order, "OrderCancelled");
                                    }
                                    else
                                    {
                                        AuditLog.log("Yourpay ReleasePayment Error Response: " + JsonConvert.SerializeObject(yourpayReleasePaymentResponse),
                                            (int)Data.Enums.AuditLogStatus.YourPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                        order.Status = (int)WMC.Data.Enums.OrderStatus.ReleaseErrored;
                                    }
                                }
                                break;
                            case "TrustPay":
                                {
                                    var trustPayReleasePaymentResponse = new Retrier<Payment>().Try(() => (new TrustPayService()).ReversalPayment(order.ExtRef, order.SiteId.Value),
                                              Int32.Parse(retrierAppSettings.MaxRetries), Int32.Parse(retrierAppSettings.DelayInMilliseconds));
                                    if (Regex.IsMatch(trustPayReleasePaymentResponse.result.code, "^000.000.|^000.100.1|^000.[36]|^000.400.0[^3]0|^000.400.100"))
                                    {
                                        AuditLog.log("TrustPay ReleasePayment Response: " + JsonConvert.SerializeObject(trustPayReleasePaymentResponse),
                                        (int)Data.Enums.AuditLogStatus.TrustPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                        order.Status = (int)WMC.Data.Enums.OrderStatus.OrderCancelled;
                                        EmailOrder(order, "OrderCancelled");
                                    }
                                    else
                                    {
                                        AuditLog.log("TrustPay ReleasePayment Error Response: " + JsonConvert.SerializeObject(trustPayReleasePaymentResponse),
                                            (int)Data.Enums.AuditLogStatus.TrustPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                        order.Status = (int)WMC.Data.Enums.OrderStatus.ReleaseErrored;
                                    }
                                }
                                break;
                            case "QuickPay":
                                {
                                    // TODO: better to be moved to a method (also each type above)
                                    var quickPayService = new QuickPayService(Convert.ToInt64(order.SiteId));
                                    long strPaymentId = Convert.ToInt64(order.ExtRef);
                                    var cancelPayment = quickPayService.CancelPayment(strPaymentId);
                                    QuickPayResponse cancelPaymentContent = JsonConvert.DeserializeObject<QuickPayResponse>(cancelPayment.Content);
                                    bool? cancelAccepted = cancelPaymentContent.Accepted;
                                    if (cancelAccepted.GetValueOrDefault())
                                    {
                                        //var getPayment = quickPayService.GetPayment(strPaymentId.ToString());
                                        //QuickPayResponse getPaymentContent = JsonConvert.DeserializeObject<QuickPayResponse>(getPayment.Content);
                                        var getPaymentContent = new Retrier<QuickPayResponse>().Try(() =>
                                        {
                                            var getPayment2 = quickPayService.GetPayment(strPaymentId.ToString());
                                            QuickPayResponse getPaymentContent2 = JsonConvert.DeserializeObject<QuickPayResponse>(getPayment2.Content);
                                            var paymentAccepted2 = getPaymentContent2.Accepted;
                                            var authorizedStatusCodes2 = getPaymentContent2.Operations.Where(x => x.Type == "cancel").Select(x => new { x.AqStatusCode, x.QpStatusCode }).LastOrDefault();
                                            if (authorizedStatusCodes2 == null)
                                            {
                                                throw new Exception(); // retry
                                            }

                                            string qpStatusCode2 = authorizedStatusCodes2.QpStatusCode;
                                            string aqStatusCode2 = authorizedStatusCodes2.AqStatusCode;
                                            if (qpStatusCode2 == null || aqStatusCode2 == null)
                                            {
                                                throw new Exception(); // retry
                                            }

                                            //if (paymentAccepted2.GetValueOrDefault() && qpStatusCode2 == "20000" && aqStatusCode2 == "20000")
                                            //{
                                            return getPaymentContent2;
                                            //}
                                            //else
                                            //{
                                            //}
                                        }
                                        , int.Parse(retrierAppSettings.MaxRetries), int.Parse(retrierAppSettings.DelayInMilliseconds));

                                        if (getPaymentContent != null)
                                        {
                                            bool? paymentAccepted = getPaymentContent.Accepted;
                                            var authorizedStatusCodes = getPaymentContent.Operations.Where(x => x.Type == "cancel").Select(x => new { x.AqStatusCode, x.QpStatusCode }).LastOrDefault();
                                            string qpStatusCode = authorizedStatusCodes.QpStatusCode;
                                            string aqStatusCode = authorizedStatusCodes.AqStatusCode;
                                            if (paymentAccepted.GetValueOrDefault() && qpStatusCode == "20000" && aqStatusCode == "20000")
                                            {
                                                AuditLog.log("QuickPay ReleasePayment Response: " + JsonConvert.SerializeObject(getPaymentContent),
                                              (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);
                                                order.Status = (int)Data.Enums.OrderStatus.OrderCancelled;
                                                EmailOrder(order, "OrderCancelled");
                                            }
                                            else
                                            {
                                                AuditLog.log("QuickPay ReleasePayment Error Response: " + JsonConvert.SerializeObject(getPaymentContent),
                                              (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                                //Set order.Status as 'Release Errored'
                                                order.Status = (int)Data.Enums.OrderStatus.ReleaseErrored;
                                            }
                                        }
                                        else
                                        {
                                            AuditLog.log("QuickPay ReleasePayment Error Response: " + JsonConvert.SerializeObject(getPaymentContent),
                                          (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                            //Set order.Status as 'Release Errored'
                                            order.Status = (int)Data.Enums.OrderStatus.ReleaseErrored;
                                        }
                                    }
                                    else
                                    {
                                        AuditLog.log("QuickPay ReleasePayment Error Response: " + JsonConvert.SerializeObject(cancelPaymentContent),
                                           (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Info, order.Id);

                                        order.Status = (int)WMC.Data.Enums.OrderStatus.ReleaseErrored;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else if (order.PaymentType == 2)
                    {
                        //Set order.Status as 'Order Cancelled'
                        order.Status = (int)Data.Enums.OrderStatus.OrderCancelled;
                    }
                    AuditLog.log("Updated order(" + orderId + ") status to " + Enum.GetName(typeof(Data.Enums.OrderStatus), order.Status) + " while processing CancelledOrders().", (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info, orderId);
                    DataUnitOfWork.Orders.Update(order);
                    DataUnitOfWork.Commit();
                    //call the MerchantWebhook if referenceId and merchantCode are not null
                    await PublishOrder(order.Id);
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in ProcessCancelledOrders() " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        private void EmailOrder(Order order, string responseType)
        {
            try
            {
                var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
                var site = DataUnitOfWork.Sites.GetById(order.SiteId.Value);
                var user = DataUnitOfWork.Users.GetById(order.UserId);
                EmailHelper.SendEmail(user.Email, responseType, new Dictionary<string, object>
                {
                    { "UserFirstName", user.Fname },
                    { "OrderNumber", order.Number },
                }, site.Text, order.BccAddress);
            }
            catch (Exception ex)
            {
                AuditLog.log("Unable to send email '" + responseType + "'.\r\nError: " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error, order.Id);
            }
        }

        // TODO: obsolete?
        public void ProcressKYCRequest()
        {
        }

        public void AuditTrailCleanUp()
        {
            try
            {
                var dc = new MonniData();
                AuditTrailCleanUp auditCleanUp = new AuditTrailCleanUp();
                var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
                var auditTrailCleanUpTime = SettingsManager.GetDefault().Get("AuditTrailCleanUpTimeInMonths").Value;
                int auditTrailCleanUpTimeInMonths = Convert.ToInt32(auditTrailCleanUpTime);
                List<AuditTrail> trails = dc.AuditTrails.Where(trail => auditCleanUp.LogsToBeCleanedUp.Any(s => trail.Message.Contains(s)) && trail.Created < DateTime.Now.AddMonths(auditTrailCleanUpTimeInMonths * (-1)).Date).ToList();
                dc.AuditTrails.RemoveRange(trails);
                dc.SaveChanges();
            }
            catch (Exception ex)
            {
                AuditLog.log("The following exception was thrown while trying to perform RunAuditTrailArchiver() - " + ex.Message, (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        public void OrderAMLProcess()
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            try
            {
                var pendingOrders = DataUnitOfWork.Orders.Get(q => q.Status == (int)Data.Enums.OrderStatus.Paid).Select(q => new { q.Id, q.Status, q.UserId, q.Type }).ToList();

                foreach (var orderLite in pendingOrders)
                {
                    var userRiskLevel = DataUnitOfWork.Users.Get(x => x.Id == orderLite.UserId).Select(x => x.UserRiskLevel).FirstOrDefault();
                    //Check userRiskLevel and move to AMLApprovalPending(this makes orders move to EDD/Compliance Page)
                    //Sell: Quoted orders should not be moved to amlpending, Only Paid orders should be moved to amlapprovalpending, otherwise will go to  race condition
                    if ((userRiskLevel == Data.Enums.UserRiskLevelType.HighRisk || userRiskLevel == Data.Enums.UserRiskLevelType.ElevatedRisk) &&
                        (orderLite.Status == (int)Data.Enums.OrderStatus.Paid))
                    {
                        var order = DataUnitOfWork.Orders.GetById(orderLite.Id);
                        AuditLog.log("Executed OrderAMLProcess() for order:" + orderLite, (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, orderLite.Id);
                        order.Status = (int)Data.Enums.OrderStatus.AMLApprovalPending;
                        DataUnitOfWork.Orders.Update(order);
                        AuditLog.log("Updated order to " + Enum.GetName(typeof(Data.Enums.OrderStatus), order.Status) + " in OrderAMLProcess() for :" + orderLite, (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, orderLite.Id);
                        DataUnitOfWork.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in OrderAMLProcess(): " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        public void CheckUserName()
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            try
            {
                var pendingOrders = DataUnitOfWork.Orders.Get(q => q.Status == (int)Data.Enums.OrderStatus.Paid || q.Status == (int)Data.Enums.OrderStatus.Quoted || q.Status == (int)Data.Enums.OrderStatus.EnhancedDueDiligence).Select(q => new { q.Id, q.Status, q.UserId, q.Type }).ToList();

                foreach (var orderLite in pendingOrders)
                {
                    var userName = DataUnitOfWork.Users.Get(x => x.Id == orderLite.UserId).Select(x => x.Fname).FirstOrDefault();
                    var dob = DataUnitOfWork.Users.Get(x => x.Id == orderLite.UserId).Select(x => x.DateOfBirth).FirstOrDefault();
                    var sanctionListChecked = DataUnitOfWork.Users.Get(x => x.Id == orderLite.UserId).Select(x => x.SanctionListChecked).FirstOrDefault();
                    var sanctionListCheckedForDob = DataUnitOfWork.Users.Get(x => x.Id == orderLite.UserId).Select(x => x.SanctionListCheckedForDob).FirstOrDefault();
                    //Check userName if username found then move the perticulaar order to EDD else
                    if (((orderLite.Status == (int)Data.Enums.OrderStatus.Paid && orderLite.Type == (int)Sell))
                        || (orderLite.Status == (int)Data.Enums.OrderStatus.Quoted && orderLite.Type == (int)Buy)
                        && (sanctionListChecked == null) || (sanctionListCheckedForDob == null) && (dob != null))
                    {
                        var sanctionlist = SanctionsListUtility.SearchByName(userName).ToList();
                        var userNameFound = false;
                        var source = "";
                        var user = DataUnitOfWork.Users.GetById(orderLite.UserId);
                        user.SanctionListChecked = DateTime.UtcNow;
                        DataUnitOfWork.Users.Update(user);
                        DataUnitOfWork.Commit();
                        if (sanctionlist.Count > 0)
                        {
                            userNameFound = true;
                            source = sanctionlist.Select(q => q.FromSourceValue).FirstOrDefault();
                        }
                        if (userNameFound)
                        {
                            if(dob != null)
                            {
                                user.SanctionListCheckedForDob = DateTime.UtcNow;
                                DataUnitOfWork.Users.Update(user);
                                DataUnitOfWork.Commit();
                                var sanctionListConfirmed = SanctionsListUtility.SearchByDob(userName, dob);
                                var dobFound = false;
                                var sourceDob = "";
                                if(sanctionListConfirmed != null)
                                {
                                    dobFound = true;
                                    sourceDob = sanctionlist.Select(q => q.FromSourceValue).FirstOrDefault();
                                    var auditMessage = "CUSTOMER: " + orderLite.UserId + "name and date of birth found in the source  " + source;
                                    AuditLog.log(auditMessage, (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info);
                                }
                                if(dobFound == false)
                                {
                                    var auditMessage = "CUSTOMER: " + orderLite.UserId + "name was found but date of birth was not found in any source";
                                    AuditLog.log(auditMessage, (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info);
                                }
                            }
                            var order = DataUnitOfWork.Orders.GetById(orderLite.Id);
                            var message = "CUSTOMER: " + orderLite.UserId + "name found in the source  " + source;
                            AuditLog.log(message, (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info);
                            var orderStatus = DataUnitOfWork.OrderStatus.GetById(26).Id;
                            order.Status = (int)Data.Enums.OrderStatus.EnhancedDueDiligence;
                            DataUnitOfWork.Orders.Update(order);
                            AuditLog.log("Username found for order: " + orderLite + "in the source  " + source, (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, orderLite.Id);
                            AuditLog.log("Updated order to " + Enum.GetName(typeof(Data.Enums.OrderStatus), order.Status) + " in checkUserName() for :" + orderLite, (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, orderLite.Id);
                            DataUnitOfWork.Commit();
                        }
                        //Todo:committing below audit log need to revisit on this
                        //AuditLog.log("Username not found in any sanction list for order: " + orderLite + "in the source  " + source, (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, orderLite.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in checkUserName(): " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
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
    }
}


