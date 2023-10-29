using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using WMC.Data;
using WMC.Logic;

namespace WMC.Utilities
{
    public class YourpayService
    {
        public static YourpayCredential GetYourpaySession()
        {
            var details = GetYourPayDetails();
            var client = new RestClient("https://webservice.yourpay.dk/v4/validatelogin");
            var request = new RestRequest();
            request.AddParameter("username", details.Username);
            request.AddParameter("password", details.Password);
            var response = client.Execute(request);
            return JsonConvert.DeserializeObject<YourpayCredential>(response.Content);
        }

        public static YourpayCapturePaymentResponse YourpayCapturePayment(string paymentid, int amount)
        {
            var yourpayCredentials = GetYourpaySession();
            var client = new RestClient("https://webservice.yourpay.dk/v4/capture_payment");
            var request = new RestRequest();
            request.AddParameter("uid", yourpayCredentials.Uid);
            request.AddParameter("sessionkey", yourpayCredentials.Sessionkey);
            request.AddParameter("paymentid", paymentid);
            request.AddParameter("amount", amount);
            var response = client.Execute(request);
            return JsonConvert.DeserializeObject<YourpayCapturePaymentResponse>(response.Content);
        }

        public static YourpayReleasePaymentResponse YourpayReleasePayment(string paymentid, int amount, long siteid)
        {
            var yourpayCredentials = GetYourpaySession();
            var client = new RestClient("https://webservice.yourpay.dk/v4/delete_payment");
            //var client = new RestClient("https://webservice.yourpay.dk/v4/refund_payment");
            var request = new RestRequest { Method = Method.POST };
            //request.AddParameter("merchantid", GetYourPayMerchantNumber(siteid));
            request.AddParameter("sessionkey", yourpayCredentials.Sessionkey);
            request.AddParameter("transid", paymentid);
            //request.AddParameter("paymentid", paymentid);
            //request.AddParameter("amount", amount);
            var response = client.Execute(request);
            return JsonConvert.DeserializeObject<YourpayReleasePaymentResponse>(response.Content);
        }

        public static YourPayAppSettings GetYourPayDetails()
        {
            var dc = new MonniData();
            var appSettings = dc.AppSettings.ToList();
            var yourPayTestOrProd = SettingsManager.GetDefault().Get("YourPayTestOrProd").Value;
            var yourPayDetails = SettingsManager.GetDefault().Get("YourPayTestDetails").GetJsonData<YourPayAppSettings>();
            if (yourPayTestOrProd == null)
            {
                AuditLog.log("YourPayTestOrProd is not defined in the database.",(int)Data.Enums.AuditLogStatus.ApplicationError,(int)Data.Enums.AuditTrailLevel.Error);
                throw new Exception("YourPayTestOrProd is not defined in the database.");
            }
            if (yourPayTestOrProd == "Prod")
            {
                yourPayDetails = SettingsManager.GetDefault().Get("YourPayProdDetails").GetJsonData<YourPayAppSettings>();
                if (yourPayDetails == null)
                {
                    AuditLog.log("YourPayProdDetails is not defined in the database.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                    throw new Exception("YourPayProdDetails is not defined in the database.");
                }
            }
            else if ((yourPayTestOrProd == "Test"))
            {
                yourPayDetails = SettingsManager.GetDefault().Get("YourPayTestDetails").GetJsonData<YourPayAppSettings>();
                if (yourPayDetails == null)
                {
                    AuditLog.log("YourPayTestDetails is not defined in the database.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);                    
                    throw new Exception("YourPayTestDetails is not defined in the database.");
                }
            }
            return yourPayDetails;
        }

        public static string GetYourPayMerchantNumber(long siteId)
        {
            var yourPayDetails = GetYourPayDetails();
            var siteMerchantDetail = yourPayDetails.SiteMerchantDetails.Where(q => q.SiteId == siteId).FirstOrDefault();
            if (siteMerchantDetail == null)
            {
                AuditLog.log("siteMerchantDetail is empty in GetYourPayMerchantNumber for the site " + siteId, (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                throw new Exception("Unable to find MerchantNumber for siteId " + siteId + ".");
            }
            return siteMerchantDetail.MerchantNumber.ToString();
        }

        public static string GetYourPayApiToken(int siteId)
        {
            var yourPayDetails = GetYourPayDetails();
            var siteMerchantDetail = yourPayDetails.SiteMerchantDetails.Where(q => q.SiteId == siteId).FirstOrDefault();
            if (siteMerchantDetail == null)
            {
                AuditLog.log("siteMerchantDetail is empty in GetYourPayApiToken for the site " + siteId, (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                throw new Exception("Unable to find MerchantNumber for siteId " + siteId + ".");
            }
            return siteMerchantDetail.ApiToken.ToString();
        }

        public static List<YourPayTransactionList> TransactionList(int listType, int limit, int from, DateTime timeFrom, DateTime timeUntill)
        {
            var time_from = GetUnixTime(timeFrom);
            var time_until = GetUnixTime(timeUntill);
            var details = GetYourpaySession();
            var client = new RestClient("https://webservice.yourpay.dk/v4/transaction_list");
            var request = new RestRequest();
            request.AddParameter("uid", details.Uid);
            request.AddParameter("sessionkey", details.Sessionkey);
            request.AddParameter("listtype", listType);
            request.AddParameter("limit", limit);
            request.AddParameter("from", from);
            request.AddParameter("time_from", time_from);
            request.AddParameter("time_until", time_until);
            var response = client.Execute(request);
            try
            {
                dynamic test = JsonConvert.DeserializeObject(response.Content);
                test[0].status.ToString();
                return new List<YourPayTransactionList>();
            }
            catch (ArgumentOutOfRangeException)
            {
                return new List<YourPayTransactionList>();
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
            {
                return JsonConvert.DeserializeObject<List<List<YourPayTransactionList>>>(response.Content)[0].Where(q => q.ReqCaptureTime.HasValue).ToList();
            }
            catch (Exception ex)
            {
                AuditLog.log("Error while listing transactions in TransactionList()", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                throw new Exception("Error!!", ex);
            }
        }

        public static List<YourPayTransactionsOutgoingOverviewList> TransactionsOutgoingOverview(DateTime timeFrom, DateTime timeUntill, int siteId)
        {
            var yourPayApiToken = GetYourPayApiToken(siteId);
            var time_from = GetUnixTime(timeFrom);
            var time_until = GetUnixTime(timeUntill);
            var client = new RestClient("https://webservice.yourpay.dk/v4/transactions_outgoing_overview");
            var request = new RestRequest();
            request.AddParameter("token", yourPayApiToken);
            request.AddParameter("date_start", time_from);
            request.AddParameter("date_end", time_until);
            var response = client.Execute(request);
            try
            {
                dynamic test = JsonConvert.DeserializeObject(response.Content);
                test[0].status.ToString();
                return new List<YourPayTransactionsOutgoingOverviewList>();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return new List<YourPayTransactionsOutgoingOverviewList>();
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
            {
                var yourPayTransactionsOutgoingOverviewList = JsonConvert.DeserializeObject<List<YourPayTransactionsOutgoingOverviewList>>(response.Content);
                var yourPayTransactionsOutgoingOverviewListResult = new List<YourPayTransactionsOutgoingOverviewList>();
                foreach (var yourPayTransactionsOutgoingOverview in yourPayTransactionsOutgoingOverviewList)
                {
                    var yourPayTransactionsOutgoingSpecifiedList = TransactionsOutgoingSpecified(yourPayTransactionsOutgoingOverview.dateid, siteId);
                    //if (yourPayTransactionsOutgoingSpecifiedList.Count > 0)
                    foreach (var yourPayTransactionsOutgoingSpecified in yourPayTransactionsOutgoingSpecifiedList)
                    {
                        var dc = new MonniData();
                        var order = dc.Orders.FirstOrDefault(q => q.Number == yourPayTransactionsOutgoingSpecified.orderID);
                        var currency = "";
                        if (order != null)
                            currency = dc.Currencies.FirstOrDefault(q => q.Id == order.CurrencyId).Code;
                        yourPayTransactionsOutgoingOverviewListResult.Add(new YourPayTransactionsOutgoingOverviewList
                        {
                            dateid = yourPayTransactionsOutgoingOverview.dateid,
                            accountid = yourPayTransactionsOutgoingOverview.accountid,
                            captured_amount = yourPayTransactionsOutgoingOverview.captured_amount,
                            currency = currency,
                            captured_fee = yourPayTransactionsOutgoingOverview.captured_fee,
                            released_amount = yourPayTransactionsOutgoingOverview.released_amount,
                            refund_amount = yourPayTransactionsOutgoingOverview.refund_amount,
                            manual_adjustments = yourPayTransactionsOutgoingOverview.manual_adjustments,
                            daily_percentage = yourPayTransactionsOutgoingOverview.daily_percentage,
                            daily_settlement_period = yourPayTransactionsOutgoingOverview.daily_settlement_period,
                            date_start = yourPayTransactionsOutgoingOverview.date_start,
                            date_expected_release = yourPayTransactionsOutgoingOverview.date_expected_release,
                            conversionrate = yourPayTransactionsOutgoingOverview.conversionrate,
                            ActionID = yourPayTransactionsOutgoingSpecified.ActionID,
                            PaymentID = yourPayTransactionsOutgoingSpecified.PaymentID,
                            req_timestamp = yourPayTransactionsOutgoingSpecified.req_timestamp,
                            amount = yourPayTransactionsOutgoingSpecified.amount,
                            captured = yourPayTransactionsOutgoingSpecified.captured,
                            handlingtype = yourPayTransactionsOutgoingSpecified.handlingtype,
                            orderID = yourPayTransactionsOutgoingSpecified.orderID,
                        });
                    }
                    //else
                    //    yourPayTransactionsOutgoingOverviewListResult.Add(new YourPayTransactionsOutgoingOverviewList
                    //    {
                    //        dateid = yourPayTransactionsOutgoingOverview.dateid,
                    //        accountid = yourPayTransactionsOutgoingOverview.accountid,
                    //        captured_amount = yourPayTransactionsOutgoingOverview.captured_amount,
                    //        captured_fee = yourPayTransactionsOutgoingOverview.captured_fee,
                    //        released_amount = yourPayTransactionsOutgoingOverview.released_amount,
                    //        refund_amount = yourPayTransactionsOutgoingOverview.refund_amount,
                    //        manual_adjustments = yourPayTransactionsOutgoingOverview.manual_adjustments,
                    //        daily_percentage = yourPayTransactionsOutgoingOverview.daily_percentage,
                    //        daily_settlement_period = yourPayTransactionsOutgoingOverview.daily_settlement_period,
                    //        date_start = yourPayTransactionsOutgoingOverview.date_start,
                    //        date_expected_release = yourPayTransactionsOutgoingOverview.date_expected_release,
                    //        conversionrate = yourPayTransactionsOutgoingOverview.conversionrate
                    //    });
                }
                return yourPayTransactionsOutgoingOverviewListResult;
            }
            catch (Exception ex)
            {
                AuditLog.log("Error while listing YourPayTransactionsOutgoingOverviewList in TransactionsOutgoingOverview()", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                return new List<YourPayTransactionsOutgoingOverviewList>();
            }
        }

        public static List<YourPayTransactionsOutgoingSpecifiedList> TransactionsOutgoingSpecified(string dateid, int siteId)
        {
            var yourPayApiToken = GetYourPayApiToken(siteId);
            var client = new RestClient("https://webservice.yourpay.dk/v4/transactions_outgoing_specified");
            var request = new RestRequest();
            request.AddParameter("token", yourPayApiToken);
            request.AddParameter("dateid", dateid);
            var response = client.Execute(request);
            try
            {
                dynamic test = JsonConvert.DeserializeObject(response.Content);
                test[0].status.ToString();
                return new List<YourPayTransactionsOutgoingSpecifiedList>();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return new List<YourPayTransactionsOutgoingSpecifiedList>();
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
            {
                return JsonConvert.DeserializeObject<List<YourPayTransactionsOutgoingSpecifiedList>>(response.Content);
            }
            catch (Exception ex)
            {
                AuditLog.log("Error while listing YourPayTransactionsOutgoingSpecifiedList in TransactionsOutgoingSpecified()", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                return new List<YourPayTransactionsOutgoingSpecifiedList>();
            }
        }

        private static long GetUnixTime(DateTime time)
        {
            return (long)(time - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
    }

    public class YourPayAppSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<SiteMerchantDetail> SiteMerchantDetails { get; set; }
        public string IntegrationCode { get; set; }
        public string ApiToken { get; set; }
    }
    public class SiteMerchantDetail
    {
        public int SiteId { get; set; }
        public int MerchantNumber { get; set; }
        public string ApiToken { get; set; }
    }
    public class YourpayCredential
    {
        public string Uid { get; set; }
        public string Sessionkey { get; set; }
    }
    public class YourpayCapturePaymentResponse
    {
        public string Result { get; set; }
        public string Responsecode { get; set; }
        public string TextResponse { get; set; }
    }
    public class YourpayReleasePaymentResponse
    {
        public string Result { get; set; }
        public string Responsecode { get; set; }
        public string TextResponse { get; set; }
    }

    public class YourPayTransactionList
    {
        public string cardtype { get; set; }
        public string currency { get; set; }
        public string Currency_
        {
            get
            {
                if (string.IsNullOrEmpty(currency))
                    return "";
                var currencyObject = new MonniData().Currencies.Where(q => q.YourPayCurrencyCode == currency).FirstOrDefault();
                if (currencyObject == null)
                    return "";
                else
                    return currency;
            }
        }
        public long restimestamp { get; set; }
        public DateTime? Res_Time_Stamp
        {
            get
            {
                if (restimestamp == 0)
                    return null;
                double datestart = 0d;
                if (double.TryParse(restimestamp.ToString(), out datestart))
                    return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(datestart).ToLocalTime();
                else
                    return null;
            }
        }
        public string req_capture { get; set; }
        public long req_capture_time { get; set; }
        public DateTime? ReqCaptureTime
        {
            get
            {
                if (req_capture_time == 0)
                    return null;
                double datestart = 0d;
                if (double.TryParse(req_capture_time.ToString(), out datestart))
                    return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(datestart).ToLocalTime();
                else
                    return null;
            }
        }
        public int PaymentID { get; set; }
        public int TransID { get; set; }
        public int orderID { get; set; }
        public int amount { get; set; }
        public int testtrans { get; set; }
        public int dateid { get; set; }
        public int trans_fee { get; set; }
        public string cardholder { get; set; }
        public string daily_percentage { get; set; }
        public string conversionrate { get; set; }
    }

    public class YourPayTransactionsOutgoingOverviewList
    {
        //public YourPayTransactionsOutgoingOverviewList()
        //{
        //    var transactionsOutgoingSpecifiedList = YourpayService.TransactionsOutgoingSpecified(dateid);
        //    YourPayTransactionsOutgoingSpecified = transactionsOutgoingSpecifiedList.FirstOrDefault();
        //}
        public string Date_Id;
        public string dateid
        {
            get { return Date_Id; }
            set
            {
                //var transactionsOutgoingSpecifiedList = YourpayService.TransactionsOutgoingSpecified(value);
                //YourPayTransactionsOutgoingSpecifiedList = transactionsOutgoingSpecifiedList;
                //var dc = new MonniData();
                //var order = dc.Orders.FirstOrDefault(q => q.Number == YourPayTransactionsOutgoingSpecifiedList.orderID);
                //var ordercurrency = dc.Currencies.FirstOrDefault(q => q.Id == order.CurrencyId);
                //currency = ordercurrency.Code;
                Date_Id = value;
            }
        }
        public string accountid { get; set; }
        public string captured_amount { get; set; }
        public string currency { get; set; }
        public string captured_fee { get; set; }
        public string released_amount { get; set; }
        public string refund_amount { get; set; }
        public string manual_adjustments { get; set; }
        public string daily_percentage { get; set; }
        public string daily_settlement_period { get; set; }
        public string date_start { get; set; }
        public DateTime? DateStart
        {
            get
            {
                double datestart = 0d;
                if (double.TryParse(date_start, out datestart))
                    return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(datestart).ToLocalTime();
                else
                    return null;
            }
        }
        public string date_expected_release { get; set; }
        public DateTime? DateExpectedRelease {
            get
            {
                double datestart = 0d;
                if (double.TryParse(date_expected_release, out datestart))
                    return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(datestart).ToLocalTime();
                else
                    return null;
            }
        }
        public string conversionrate { get; set; }
        public string ActionID { get; set; }
        public string PaymentID { get; set; }
        public string req_timestamp { get; set; }
        public DateTime? ReqTimestamp
        {
            get
            {
                double datestart = 0d;
                if (double.TryParse(req_timestamp, out datestart))
                    return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(datestart).ToLocalTime();
                else
                    return null;
            }
        }
        public string amount { get; set; }
        public string captured { get; set; }
        public string handlingtype { get; set; }
        public string orderID { get; set; }
        //public List<YourPayTransactionsOutgoingSpecifiedList> YourPayTransactionsOutgoingSpecifiedList { get; set; }
    }

    public class YourPayTransactionsOutgoingSpecifiedList
    {
        public string ActionID { get; set; }
        public string PaymentID { get; set; }
        public string req_timestamp { get; set; }
        public DateTime? ReqTimestamp
        {
            get
            {
                double datestart = 0d;
                if (double.TryParse(req_timestamp, out datestart))
                    return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(datestart).ToLocalTime();
                else
                    return null;
            }
        }
        public string dateid { get; set; }
        public string amount { get; set; }
        public string captured { get; set; }
        public string handlingtype { get; set; }
        public string orderID { get; set; }
    }
}