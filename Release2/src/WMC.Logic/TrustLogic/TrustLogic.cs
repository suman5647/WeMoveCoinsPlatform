using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using WMC.Data;
using WMC.Data.Enums;
using WMC.Utilities;

namespace WMC.Logic.TrustLogic
{
    public class TrustLogic
    {
        public Dictionary<string, decimal> Rates { get; set; }
        string trustLogic = "";
        /// <summary>
        /// Executes an Order based on a RiskScore value between 0 and 1.
        /// There are four outcomes: 1) KYC Decline, 2) TxSecret Request, 3) �Manual Compliance Review, 4) Auto Payout
        /// RiskScore value is calulated in TrustLevelCalculation()
        /// NOTE: Emails functionality shall utilize the functionality implemented in the App
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="ActualOrderStatus"></param> 'This makes sure that we only execute if Order is in the status we expects
        /// <param name="RiskScore"></param> ' Input value
        /// <returns>Returns True/False based on a successfull execution. Noy really used though</returns>
        public bool TrustCalculationExecution(long orderId, Decimal RiskScore)
        {
            try
            {
                //TODO RISKSCORE TO CONST AND ORDER ONLY REQ
                var dc = new MonniData();
                bool result = true;
                trustLogic += "RiskScore:" + RiskScore + ",\r\n";
                if (RiskScore == RiskScoreConsts.RiskScore0)
                {
                    //TODO SELECT ONLY REQUIRED 
                    var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                    if (order == null)
                        throw new Exception("Unable to fetch order with orderid:" + orderId);
                    order.RiskScore = RiskScore;
                    order.Status = (int)Data.Enums.OrderStatus.KYCDecline;
                    trustLogic += "order status:" + (int)Data.Enums.OrderStatus.KYCDecline;
                }
                else if (RiskScore > RiskScoreConsts.RiskScore0 && RiskScore < RiskScoreConsts.RiskScore02)
                {
                    var currentOrder = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                    if (currentOrder == null)
                        throw new Exception("Unable to fetch order with orderid:" + orderId);

                    trustLogic += "cardNumber:" + currentOrder.CardNumber + ",\r\n";

                    var order = dc.Orders.Where(q => q.CardNumber == currentOrder.CardNumber &&
                    (q.Status == (int)Data.Enums.OrderStatus.PayoutAwaitsApproval ||
                     q.Status == (int)Data.Enums.OrderStatus.KYCApprovalPending ||
                     q.Status == (int)Data.Enums.OrderStatus.Completed ||
                     q.Status == (int)Data.Enums.OrderStatus.ComplianceOfficerApproval ||
                     q.Status == (int)Data.Enums.OrderStatus.CustomerResponsePending) &&
                     !string.IsNullOrEmpty(q.TxSecret) && !string.IsNullOrEmpty(q.CardNumber)).OrderBy(q => q.Quoted).FirstOrDefault();
                    if (order != null)
                    {
                        var user = dc.Users.FirstOrDefault(q => q.Id == order.UserId);
                        var site = dc.Sites.FirstOrDefault(q => q.Id == order.SiteId);
                        var currency = dc.Currencies.FirstOrDefault(q => q.Id == order.CurrencyId);
                        if (!currentOrder.CreditCardUserIdentity.HasValue)
                        {
                            AuditLog.log("Order CreditCardUserIdentity is null", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug, orderId);
                            return false;
                        }

                        EmailHelper.SendEmail(user.Email, "RequestTxSecret", new Dictionary<string, object>
                            {
                                { "UserIdentity", currentOrder.CreditCardUserIdentity },
                                { "SiteName", site.Text },
                                { "UserFirstName", order.Name },
                                { "OrderNumber", order.Number },
                                { "OrderCompleted", order.Quoted },
                                { "OrderAmount", order.Amount },
                                { "OrderCurrency", currency.Code },
                                { "CreditCard", order.CardNumber },
                            }, site.Text, order.BccAddress);

                        currentOrder.RiskScore = RiskScore;
                        currentOrder.Status = (int)Data.Enums.OrderStatus.CustomerResponsePending;
                        AddNote(orderId, " Tx-Secret Requested:" + order.TxSecret);
                        AuditLog.log("Tx-Secret Requested:" + order.TxSecret + "TrustCalculationExecution", (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, orderId);
                        trustLogic += "Order status:" + (int)Data.Enums.OrderStatus.CustomerResponsePending;
                    }
                    else
                    {
                        AuditLog.log("NOT SUCCESSFULL Tx-Secret Request - TrustCalculationExecution", (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, orderId);
                        AddNote(orderId, " NOT SUCCESSFULL Tx-Secret Request");
                        result = false;
                    }
                }
                else if (RiskScore >= RiskScoreConsts.RiskScore02 && RiskScore < RiskScoreConsts.RiskScore09)
                {
                    //TODO SELECT ONLY REQUIRED 
                    var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                    if (order != null)
                    {
                        var user = dc.Users.FirstOrDefault(q => q.Id == order.UserId);
                        var site = dc.Sites.FirstOrDefault(q => q.Id == order.SiteId);
                        var results = SettingsManager.GetDefault().Get("ComplianceAvailability").Value;
                        var isAdminAway = String.Compare(results, "Away", true) == 0 ? "true" : "false";
                        if (isAdminAway == "true")
                            EmailHelper.SendEmail(user.Email, "OrderComplianceInspection", new Dictionary<string, object>
                                {
                                    { "UserFirstName", order.Name },
                                    { "OrderNumber", order.Number },
                                }, site.Text, order.BccAddress);

                        var testOrProd = "";
                        if (order.PaymentType == 1)
                            if (!site.Text.Contains("localhost"))
                                if (site.Text.Split('.')[0].ToLower() == "apptest")
                                    testOrProd = "TEST: ";

                        var currency = dc.Currencies.FirstOrDefault(q => q.Id == order.CurrencyId);
                        var country = dc.Countries.FirstOrDefault(q => q.CurrencyId == currency.Id);
                        PushoverHelper.SendNotification(testOrProd + (site.Text.Contains("localhost") ? site.Text : site.Text.Split('.')[1]), order.Number + "COMPLIANCE REVIEW: " + order.Amount.Value.ToString("N0", new CultureInfo(country.CultureCode)) + " " + currency.Code + ", " + order.Name + ", " + order.CommissionProcent + " %");

                        order.RiskScore = RiskScore;
                        order.Status = (int)Data.Enums.OrderStatus.ComplianceOfficerApproval;
                        trustLogic += "order status to:" + (int)Data.Enums.OrderStatus.ComplianceOfficerApproval;
                    }
                    else
                    {
                        AddNote(orderId, " NOT SUCCESSFULL OrderComplianceInspection Notification");
                        AuditLog.log("NOT SUCCESSFULL OrderComplianceInspection Notification - TrustCalculationExecution", (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, orderId);
                        result = false;
                    }
                }
                else if ((RiskScore >= RiskScoreConsts.RiskScore09))
                {
                    var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                    order.RiskScore = RiskScore;
                    order.Approved = DateTime.Now;
                    order.Status = (int)Data.Enums.OrderStatus.PayoutApproved;
                    trustLogic += "order status:" + (int)Data.Enums.OrderStatus.PayoutApproved;
                }
                else
                {
                    throw new Exception("RiskScore does not fit within any range.");
                }
                dc.SaveChanges();
                AuditLog.log("TrustCalculationExecution:\r\n" + trustLogic, (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, orderId);
                return result;
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in TrustCalculationExecution(" + orderId + ", " + RiskScore + ")\r\n" + ex.ToMessageAndCompleteStacktrace(),
                    (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error, orderId);
                throw ex;
            }
        }

        /// <summary>
        /// Base on different Order-variables, this functions calculates a RiskScore
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="trustMessage"></param> A readable message-string if constructed and set to te passing parameter 
        /// <returns>RiskScore Value</returns>
        public Decimal TrustLevelCalculation(long orderId, ref string trustMessage)
        {
            try
            {
                var dc = new MonniData();
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                // Warning!!! Optional parameters not supported
                //string CardNumber = eQuote.ExecuteScalar(("SELECT CardNumber FROM [Order] WHERE Id =" + orderId.ToString)).ToString.Trim;
                //int TxLimitEUR = int.Parse(eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = \'TxLimitEUR\'"));
                bool CardIsApproved = IsCardApproved(orderId);
                trustLogic += "CardIsApproved:" + CardIsApproved + ",\r\n";
                decimal riskScore = 0;
                var isCardUS = IsCardUS(orderId);
                trustLogic += "IsCardUS:" + isCardUS + ",\r\n";
                if (isCardUS && !CardIsApproved)
                {
                    // KYC DECLINE(24)
                    trustMessage = "Decline - US Card";
                    riskScore = RiskScoreConsts.RiskScore0;
                }
                else
                {
                    decimal CountryTrust = GetCountryTrustLevel(orderId);
                    trustLogic += "CountryTrust:" + CountryTrust + ",\r\n";
                    decimal SiteTrust = GetSiteTrustLevel(orderId);
                    trustLogic += "SiteTrust:" + SiteTrust + ",\r\n";
                    var TxLimitEUR = int.Parse(SettingsManager.GetDefault().Get("TxLimitEUR").Value);
                    trustLogic += "TxLimitEUR:" + TxLimitEUR + ",\r\n";
                    var OrderSumEUR = GetOrderSumEUR(orderId);
                    trustLogic += "OrderSumEUR:" + OrderSumEUR + ",\r\n";
                    var OrderSumTotalEUR = GetOrderSumTotalEUR(orderId, order.CardNumber);
                    trustLogic += "OrderSumTotalEUR:" + OrderSumTotalEUR + ",\r\n";
                    //AuditLog.log(string.Format("!CardIsApproved:{0} && ((OrderSumEUR:{1} + OrderSumTotalEUR:{2}) > (TxLimitEUR:{3} * CountryTrust:{4} * SiteTrust:{5}))", !CardIsApproved, OrderSumEUR, OrderSumTotalEUR.HasValue ? OrderSumTotalEUR.Value : 0, TxLimitEUR, CountryTrust, SiteTrust), (int)Data.Enums.AuditLogStatus.ApplicationError);
                    if (!CardIsApproved && ((OrderSumEUR + OrderSumTotalEUR) > (TxLimitEUR * CountryTrust * SiteTrust)))
                    {
                        // TX SECRET -> CUSTOMER RESPONSE PENDING(22)
                        string PhoneCountry = GetPhoneCountry(orderId);
                        trustLogic += "PhoneCountry:" + PhoneCountry + ",\r\n";
                        var isAdminAway = dc.AppSettings.FirstOrDefault(q => q.ConfigKey == "ComplianceAvailability").ConfigValue == "Away";
                        trustLogic += "IsAdminAway:" + isAdminAway + ",\r\n";
                        if (PhoneCountry == "DK" && !isAdminAway)
                        {
                            trustMessage = "(DK) Manual Approval: Tx Limit is Exceeded ";
                            riskScore = RiskScoreConsts.RiskScore02;
                        }
                        else
                        {
                            riskScore = RiskScoreConsts.RiskScore01;
                            trustMessage = "Ask for Tx Secret";
                        }
                    }
                    else
                    {
                        bool PhoneIsVirtual = IsPhoneVitual(orderId);
                        trustLogic += "PhoneIsVirtual:" + PhoneIsVirtual + ",\r\n";
                        int CardUsedElsewhere = GetCardUsedElsewhere(orderId);
                        trustLogic += "CardUsedElsewhere:" + CardUsedElsewhere + ",\r\n";
                        int EmailUsedElsewhere = GetEmailUsedElsewhere(orderId);
                        trustLogic += "EmailUsedElsewhere:" + EmailUsedElsewhere + ",\r\n";
                        int IpUsedElsewhere = 0; //GetIpUsedElsewhere(orderId);
                        trustLogic += "IpUsedElsewhere:" + IpUsedElsewhere + ",\r\n";

                        var crossedSpeedLimit = CrossedSpeedLimit(orderId);

                        var phoneCardOriginMatch = PhoneCardOrigin_Match(orderId);
                        trustLogic += "PhoneCardOriginMatch:" + phoneCardOriginMatch + ",\r\n";
                        var phoneIPMatch = PhoneIP_Match(orderId);
                        trustLogic += "PhoneIPMatch:" + phoneIPMatch + ",\r\n";
                        //var isWalletAddressScoreRed = IsWalletAddressScoreRed(order.UserId.ToString(), order.CryptoAddress);
                        //trustLogic += "isWalletAddressScoreRed:" + isWalletAddressScoreRed + ",\r\n";
                        if (PhoneIsVirtual || (CardUsedElsewhere > 0 || EmailUsedElsewhere > 0 || IpUsedElsewhere > 0) || crossedSpeedLimit || !phoneCardOriginMatch || !phoneIPMatch )
                            /* || isWalletAddressScoreRed*/
                        {
                            if (PhoneIsVirtual)
                                trustMessage += "Virtual Number, ";
                            if (CardUsedElsewhere > 0 || EmailUsedElsewhere > 0 || IpUsedElsewhere > 0)
                                trustMessage += "Data Used Elsewhere, ";
                            if (!phoneCardOriginMatch)
                                trustMessage += "Phone/Card Mismatch, ";
                            if (!phoneIPMatch)
                                trustMessage += "  - Phone/IP Mismatch, ";
                            //if (isWalletAddressScoreRed)
                            //  trustMessage += " WalletAddressScore is Red, ";

                            bool UserIsTrusted = IsUserTrusted(orderId);
                            if (UserIsTrusted)
                            {
                                // PAYOUT APPROVED(18)
                                trustMessage = "Approved although:" + trustMessage;
                                riskScore = RiskScoreConsts.RiskScore09;
                            }
                            else
                            {
                                // COMPLIANCE OFFICER APPROVAL(21)
                                trustMessage = "Manual Approval: " + trustMessage;
                                riskScore = RiskScoreConsts.RiskScore02;
                            }
                        }
                        else
                        {
                            // PAYOUT APPROVED(18)
                            trustMessage = "Approved";
                            riskScore = RiskScoreConsts.RiskScore05;   //changed to 0.5 from 1
                        }
                    }
                }
                trustLogic += "TrustMessage:" + trustMessage + ",\r\n";
                trustLogic += "RiskScore:" + riskScore;
                AuditLog.log("TrustLevelCalculation:" + trustLogic, (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info, orderId);
                return riskScore;
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in TrustLevelCalculation(" + orderId + ", " + trustMessage + ")\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error, orderId);
                throw ex;
            }
        }

        public bool IsCardApproved(long orderId)
        {
            try
            {
                var dc = new MonniData();
                //TODO SELECT ONLY REQUIRED
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                if (string.IsNullOrEmpty(order.CardNumber))
                    return false;

                var approvedCardOrderCount = dc.Orders.Count(q => q.CardNumber == order.CardNumber && q.CardApproved.HasValue);
                trustLogic += "ApprovedCardOrderCount:" + approvedCardOrderCount + ",\r\n";
                return approvedCardOrderCount > 0;
                }
            catch (Exception ex)
            {
                throw new Exception("Error in IsCardApproved(" + orderId + ")", ex);
            }

            //object CardNumber = eQuote.ExecuteScalar("SELECT CardNumber FROM [Order] WHERE Id =" + OrderId.ToString);
            //if (Information.IsDBNull(CardNumber))
            //    return false;
            //int n = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE CardNumber = N'" + CardNumber.ToString + "' AND NOT (CardApproved IS NULL)");
            //if (n == 0)
            //{
            //    return false;
            //}
            //else
            //{
            //    if ((c != null))
            //        c.BackColor = Drawing.Color.FromName("#66FF66");
            //    return true;
            //}
        }

        public decimal GetCountryTrustLevel(long orderId)
        {
            try
            {
                var countryCode = GetPhoneCountry(orderId);
                trustLogic += "CountryCode:" + countryCode;
                var dc = new MonniData();
                var country = dc.Countries.FirstOrDefault(q => q.Code == countryCode);
                if (country?.TrustValue == null)
                    return 0m;
                trustLogic += "Country trust value:" + (country.TrustValue.HasValue ? country.TrustValue.Value : 0m);
                return country.TrustValue.HasValue ? country.TrustValue.Value : 0m;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetCountryTrustLevel(" + orderId + ")", ex);
            }
            //string CountryCode = GetPhoneCountry(OrderId);
            //return eQuote.ExecuteScalar("SELECT TrustValue FROM Country WHERE Code = N'" + CountryCode + "'");
        }

        //speedlimit is true when user have crossed the 10000 EUR limit within last 24 hrs
        public bool CrossedSpeedLimit(long orderId)
        {
            try
            {
                var dc = new MonniData();
                SpeedLimitSettings speedLimitSettingsJSON = SettingsManager.GetDefault().Get("SpeedLimitSettings").GetJsonData<SpeedLimitSettings>();
                //TODO FURTHER RESEARCH WHAT DIFFERENCE FROM DAYLIMIT AND THIS
                var durationInteger = -int.Parse((string)speedLimitSettingsJSON.PreviousDuration);
                var previousOrderAmountInteger = int.Parse((string)speedLimitSettingsJSON.PreviousOrderAmount);
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                var previousDuration = DateTime.Now.AddHours(durationInteger);
                var user = dc.Users.FirstOrDefault(q => q.Id == order.UserId);
                var previousOrders = user.ApprovedOrders.Where(q => q.Status == (int)Data.Enums.OrderStatus.Completed && q.Quoted > previousDuration).ToList();
                var previousOrderAmount = 0M;
                if (previousOrders.Any())
                    foreach (var previousOrder in previousOrders)
                        previousOrderAmount += previousOrder.Amount.Value / OpenExchangeRates.GetEURExchangeRate(previousOrder.Currency.Code, Rates);
                if (previousOrderAmount > previousOrderAmountInteger)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CrossedSpeedLimit(" + orderId + ")", ex);
            }
        }

        public bool IsWalletAddressScoreRed(string userId, string withdrawalAddress)
        {
            try
            {
                var score = ChainalysisInterface.GetWithdrawalAddress(userId, withdrawalAddress);
                trustLogic += "User address score:" + JsonSerializerEx.SerializeObject(score, 1);
                return score.score == "red";
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetWalletAddressScore(" + userId + "," + withdrawalAddress + ")", ex);
            }
        }

        public decimal GetSiteTrustLevel(long orderId)
        {
            try
            {
                var dc = new MonniData();
                //TODO SELECT ONLY
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                if (order.SiteId == 1)
                    return 0.8m;
                else
                    return 1;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetSiteTrustLevel(" + orderId + ")", ex);
            }

            //int SiteId = eQuote.ExecuteScalar("SELECT SiteId FROM [Order] WHERE Id = " + orderId.ToString);
            //if (SiteId == 1)
            //    return 0.8;
            //else
            //    return 1;
        }

        public bool IsPhoneVitual(long orderId)
        {
            try
            {
                int nameCount = GetDifferentNameUser(orderId);
                trustLogic += "Names:" + nameCount + ",\r\n";
                int emailCount = GetDifferentEmailUser(orderId);
                trustLogic += "Emails:" + emailCount + ",\r\n";
                if ((nameCount > 1 && emailCount > 1) || nameCount > 2 || emailCount > 2)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in IsPhoneVitual(" + orderId + ")", ex);
            }

            //int names = GetDifferentNameUser(orderId);
            //int emails = GetDifferentEmailUser(orderId);
            //if ((names > 1 & emails > 1) | names > 2 | emails > 2)
            //{
            //    if ((c != null))
            //        c.BackColor = Drawing.Color.FromName("#FFFF66");
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        public bool PhoneCardOrigin_Match(long orderId)
        {
            try
            {
                var getPhoneCountry = GetPhoneCountry(orderId);
                trustLogic += "PhoneCountry:" + getPhoneCountry + ",\r\n";
                var getCardCountry = GetCardCountry(orderId);
                trustLogic += "CardCountry:" + getCardCountry + ",\r\n";
                if (getPhoneCountry == getCardCountry || getCardCountry == "GB")
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in PhoneCardOrigin_Match(" + orderId + ")", ex);
            }
        }

        public bool PhoneIP_Match(long orderId)
        {
            try
            {
            var getPhoneCountry = GetPhoneCountry(orderId);
            trustLogic += "PhoneCountry:" + getPhoneCountry + ",\r\n";
            var getIpCountry = GetIpCountry(orderId);
            trustLogic += "IpCountry:" + getIpCountry + ",\r\n";
            if (getPhoneCountry == getIpCountry)
                return true;
            else
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in PhoneIP_Match(" + orderId + ")", ex);
            }
        }

        public int GetCardUsedElsewhere(long orderId)
        {
            try
            {
                var dc = new MonniData();
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                var user = dc.Users.FirstOrDefault(q => q.Id == order.UserId);
                var orderCount = dc.Orders.Count(q => q.UserId != user.Id && q.CardNumber == order.CardNumber);
                trustLogic += "CardUsedElsewhereOrderCount = " + orderCount + ",\r\n";
                return orderCount;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetCardUsedElsewhere(" + orderId + ")", ex);
            }

            //int UserId = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + orderId.ToString);
            //SqlDataReader dr = eQuote.GetDataReader("SELECT [Transaction].Info, [Order].UserId, [User].Phone, [Order].CardNumber, [Order].CountryCode, [Order].Amount, [Order].CurrencyId, AuditTrail.Message AS Phone_Message, AuditTrail_1.Message AS IP_Message, [Order].CryptoAddress, [User].Email, [Order].IP  FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].orderId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN AuditTrail ON [Order].Id = AuditTrail.orderId INNER JOIN AuditTrail AS AuditTrail_1 ON [Order].Id = AuditTrail_1.orderId WHERE ([Order].Id = " + orderId.ToString + ") AND ([Transaction].Type = 1) AND (AuditTrail.Status = 3) AND (AuditTrail_1.Status = 4)");
            //int n = 0;
            //if (dr.HasRows)
            //{
            //    dr.Read();
            //    try
            //    {
            //        n = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + UserId.ToString + ")) AND (CardNumber = N'" + dr["CardNumber"] + "')").ToString;
            //        if ((c != null))
            //        {
            //            c.CommandArgument = dr["CardNumber"];
            //            if (n > 0)
            //                c.BackColor = Drawing.Color.FromName("#FFFF66");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        n = -1;
            //    }
            //}
            //dr.Close();
            //return n;
        }

        public int GetEmailUsedElsewhere(long orderId)
        {
            try
            {
                var dc = new MonniData();
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                var user = dc.Users.FirstOrDefault(q => q.Id == order.UserId);
                var orderCount = dc.Orders.Count(q => q.UserId != user.Id && q.Email == user.Email);
                trustLogic += "GetEmailUsedElsewhere:orderCount = " + orderCount + ",\r\n";
                return orderCount;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetEmailUsedElsewhere(" + orderId + ")", ex);
            }
            //int UserId = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + orderId.ToString);
            //SqlDataReader dr = eQuote.GetDataReader("
            //SELECT [User].Email FROM [Order] INNER JOIN
            //[Transaction] ON [Order].Id = [Transaction].orderId INNER JOIN 
            //[User] ON [Order].UserId = [User].Id INNER JOIN 
            //AuditTrail ON [Order].Id = AuditTrail.orderId INNER JOIN 
            //AuditTrail AS AuditTrail_1 ON [Order].Id = AuditTrail_1.orderId WHERE ([Order].Id = " + orderId.ToString + ") AND ([Transaction].Type = 1) AND (AuditTrail.Status = 3) AND (AuditTrail_1.Status = 4)");
            //int n = 0;
            //if (dr.HasRows)
            //{
            //    dr.Read();
            //    try
            //    {
            //        n = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + UserId.ToString + ")) AND (Email = N'" + dr["Email"] + "')").ToString;
            //        if ((c != null))
            //        {
            //            c.CommandArgument = dr["Email"];
            //            if (n > 1)
            //                c.BackColor = Drawing.Color.FromName("#FFFF66");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        n = -1;
            //    }
            //}
            //dr.Close();
            //return n;
        }

        public int GetIpUsedElsewhere(int orderId)
        {
            try
            {
                var dc = new MonniData();
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                var user = dc.Users.FirstOrDefault(q => q.Id == order.UserId);
                var orderCount = dc.Orders.Count(q => q.UserId != user.Id && q.IP == order.IP);
                trustLogic += "IpUsedElsewhereOrderCount = " + orderCount + ",\r\n";
                return orderCount;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetIpUsedElsewhere(" + orderId + ")", ex);
            }

            //int UserId = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + orderId.ToString);
            //SqlDataReader dr = eQuote.GetDataReader("SELECT [Transaction].Info, [Order].UserId, [User].Phone, [Order].CardNumber, [Order].CountryCode, [Order].Amount, [Order].CurrencyId, AuditTrail.Message AS Phone_Message, AuditTrail_1.Message AS IP_Message, [Order].CryptoAddress, [User].Email, [Order].IP  FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].orderId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN AuditTrail ON [Order].Id = AuditTrail.orderId INNER JOIN AuditTrail AS AuditTrail_1 ON [Order].Id = AuditTrail_1.orderId WHERE ([Order].Id = " + orderId.ToString + ") AND ([Transaction].Type = 1) AND (AuditTrail.Status = 3) AND (AuditTrail_1.Status = 4)");
            //int n = 0;
            //if (dr.HasRows)
            //{
            //    dr.Read();
            //    try
            //    {
            //        n = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + UserId.ToString + ")) AND (IP = N'" + dr["IP"] + "')").ToString;
            //        if ((c != null))
            //        {
            //            if (n > 1)
            //                c.BackColor = Drawing.Color.FromName("#FFFF66");
            //            c.CommandArgument = dr["IP"];
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        n = -1;
            //    }
            //}
            //dr.Close();
            //return n;
        }

        public bool IsCardUS(long orderId)
        {
            try
            {
                var getCardCountry = GetCardCountry(orderId);
                trustLogic += "IsCardUS:getCardCountry = " + getCardCountry + ",\r\n";
                if (getCardCountry == "US")
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in IsCardUS(" + orderId + ")", ex);
            }
        }

        public bool IsCardUS(string creditCardNumber)
        {
            try
            {
                var getCardCountry = GetBinListLookup(creditCardNumber.Replace(" ", "").Substring(0, 6));
                trustLogic += "IsCardUS:getCardCountry = " + getCardCountry + ",\r\n";
                if (getCardCountry == "US")
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in IsCardUS(" + creditCardNumber + ")", ex);
            }
        }

        public decimal GetOrderSumEUR(long orderId)
        {
            try
            {
                var dc = new MonniData();
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                if (order.RateHome.HasValue)
                    return order.Amount.Value / order.RateHome.Value;
                else
                {
                    var amountEUR = 0m;
                    var localAmount = order.Amount.Value;
                    var currency = dc.Currencies.FirstOrDefault(q => q.Id == order.CurrencyId);
                    switch (currency.Code)
                    {
                        case "DKK":
                            amountEUR = localAmount / 7.44m;
                            break;
                        case "GBP":
                            amountEUR = localAmount / 0.852829m;
                            break;
                        case "INR":
                            amountEUR = localAmount / 72.76m;
                            break;
                        case "SEK":
                            amountEUR = localAmount / 9.45m;
                            break;
                        case "NOK":
                            amountEUR = localAmount/ 0.75m;
                            break;
                        case "USD":
                            amountEUR = localAmount / 6.20m;
                            break;
                        case "EUR":
                            amountEUR = localAmount;
                            break;
                    }
                    trustLogic += "LocalAmount:" + localAmount + ",\r\n";
                    trustLogic += "AmountEUR:" + amountEUR + ",\r\n";
                    return amountEUR;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetOrderSumEUR(" + orderId + ")", ex);
            }

            //decimal AmountEUR = default(decimal);
            //try
            //{
            //    AmountEUR = eQuote.ExecuteScalar("SELECT Amount / RateHome AS AmountEUR FROM [Order] WHERE Id = " + orderId.ToString);
            //}
            //catch (Exception ex)
            //{            //    string CurrencyCode = eQuote.ExecuteScalar("SELECT Currency.Code FROM [Order] INNER JOIN Currency ON [Order].CurrencyId = Currency.Id WHERE [Order].Id = " + orderId.ToString);
            //    decimal LocalAmount = eQuote.ExecuteScalar("SELECT Amount FROM [Order] WHERE Id = " + orderId.ToString);
            //    switch (CurrencyCode)
            //    {
            //        case "DKK":
            //            AmountEUR = LocalAmount / 7.44;
            //            break;
            //        case "GBP":
            //            AmountEUR = LocalAmount / 0.852829;
            //            break;
            //        case "INR":
            //            AmountEUR = LocalAmount / 72.76;
            //            break;
            //        case "SEK":
            //            AmountEUR = LocalAmount / 9.45;
            //            break;
            //        case "EUR":
            //            AmountEUR = LocalAmount;
            //            break;
            //    }
            //}
            //return AmountEUR;
        }

        public decimal? GetOrderSumTotalEUR(long orderId, string CardNumber = "")
        {
            try
            {
                var dc = new MonniData();
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                if (order.CardNumber.Length > 0)
                {
                    var orders = dc.Orders.Where(q => q.UserId == order.UserId && q.Status == (int)Data.Enums.OrderStatus.Completed && q.CardNumber == CardNumber && (q.Amount / q.RateHome).HasValue).ToList();
                    var orderSumTotalEUR = (decimal?)0m;
                    foreach (var orderRecord in orders)
                        if (dc.Transactions.Where(q => q.OrderId == orderRecord.Id && q.Completed.HasValue && q.Type == 1).Any())
                            orderSumTotalEUR += (orderRecord.Amount / orderRecord.RateHome);
                    trustLogic += "orderSumTotalEUR:" + orderSumTotalEUR + ",\r\n";
                    return orderSumTotalEUR;
                }
                else
                {
                    var orders = dc.Orders.Where(q => q.UserId == order.UserId && q.Status == (int)Data.Enums.OrderStatus.Completed && (q.Amount / q.RateHome).HasValue).ToList();
                    var orderSumTotalEUR = (decimal?)0m;
                    foreach (var orderRecord in orders)
                        if (dc.Transactions.Where(q => q.OrderId == orderRecord.Id && q.Completed.HasValue && q.Type == 1).Any())
                            orderSumTotalEUR += (orderRecord.Amount / orderRecord.RateHome);
                    trustLogic += "OrderSumEUR:" + orderSumTotalEUR + ",\r\n";
                    return orderSumTotalEUR;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetOrderSumTotalEUR(" + orderId + ", " + CardNumber + ")", ex);
            }
            //int UserId = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + orderId.ToString);
            //decimal AmountEUR = default(decimal);
            //string WhereCard = "";
            //if (CardNumber.Length > 0)
            //    WhereCard = " AND CardNumber = N'" + CardNumber + "'";
            //try
            //{
            //    AmountEUR = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome) AS AmountEUR FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].orderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type 
            //WHERE  (NOT ([Transaction].Completed IS NULL)) AND ([Transaction].Type = 1) AND (NOT ([Order].Amount / [Order].RateHome IS NULL)) AND (OrderStatus.Text = N'Completed') AND [Order].UserId = " + UserId.ToString + WhereCard);
            // New
            //AmountEUR = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome) AS AmountEUR FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType  ON TransactionType.Id = [Transaction].Type
            //    INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE  (NOT ([Transaction].Completed IS NULL)) AND ([Transaction].Type = 1) AND (NOT ([Order].Amount / [Order].RateHome IS NULL)) AND (OrderStatus.Text = N'Completed') AND [Order].UserId = " + UserId.ToString + WhereCard)
            // New
            //}
            //catch (Exception ex)
            //{
            //    AmountEUR = 0;
            //}
            //return AmountEUR;
        }

        public bool IsUserTrusted(long orderId)
        {
            try
            {
                var dc = new MonniData();
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                var user = dc.Users.FirstOrDefault(q => q.Id == order.UserId);
                trustLogic += "IsUserTrusted:" + user.Trusted.HasValue + ",\r\n";
                return user.Trusted.HasValue;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in IsUserTrusted(" + orderId + ")", ex);
            }
            //int UserId = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + orderId.ToString);
            //if (Information.IsDBNull(eQuote.ExecuteScalar("SELECT Trusted FROM [User] WHERE Id = " + UserId.ToString)))
            //{
            //    return false;
            //}
            //else
            //{
            //    if ((c != null))
            //        c.BackColor = Drawing.Color.FromName("#66FF66");
            //    return true;
            //}
        }

        public int GetDifferentNameUser(long orderId)
        {
            try
            {
                var dc = new MonniData();
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                var nameCount = dc.Orders.Where(q => q.UserId == order.UserId).Select(q => q.Name).Distinct().Count();
                trustLogic += "GetDifferentNameUser:orderCount = " + nameCount + ",\r\n";
                return nameCount;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetDifferentNameUser(" + orderId + ")", ex);
            }

            //int UserId = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + orderId.ToString);
            //try
            //{
            //    int n = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT Name) FROM [Order] WHERE UserId = " + UserId.ToString);
            //    if ((c != null))
            //    {
            //        if (n > 1 & n < 3)
            //            c.BackColor = Drawing.Color.FromName("#FFFF66");
            //        if (n > 2)
            //            c.BackColor = Drawing.Color.FromName("#FFA8C5");
            //    }
            //    return n - 1;
            //}
            //catch (Exception ex)
            //{
            //    return -1;
            //}
        }

        public int GetDifferentEmailUser(long orderId)
        {
            try
            {
                var dc = new MonniData();
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                var emailCount = dc.Orders.Where(q => q.UserId == order.UserId).Select(q => q.Email).Distinct().Count();
                trustLogic += "GetDifferentEmailUser:orderCount = " + emailCount + ",\r\n";
                return emailCount;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetDifferentEmailUser(" + orderId + ")", ex);
            }
            //int UserId = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + orderId.ToString);
            //try
            //{
            //    int n = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT Email) FROM [Order] WHERE UserId = " + UserId.ToString);
            //    if ((c != null))
            //    {
            //        if (n > 1 & n < 4)
            //            c.BackColor = Drawing.Color.FromName("#FFFF66");
            //        if (n > 3)
            //            c.BackColor = Drawing.Color.FromName("#FFA8C5");
            //    }
            //    return n - 1;
            //}
            //catch (Exception ex)
            //{
            //    return -1;
            //}
        }

        public bool IsKycApproved(int orderId)
        {
            try
            {
                var dc = new MonniData();
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                var nApproved = dc.KycFiles.Where(q => q.Approved.HasValue && q.UserId == order.UserId && (q.KycType.Text == "ProofOfRecidency" || q.KycType.Text == "PhotoID")).Count();
                trustLogic += "nApproved:" + nApproved + ",\r\n";
                var nTotal = dc.KycFiles.Where(q => q.UserId == order.UserId && (q.KycType.Text == "ProofOfRecidency" || q.KycType.Text == "PhotoID")).Count();
                trustLogic += "nTotal:" + nTotal + ",\r\n";
                if (nApproved > 0)
                {
                    if (nTotal > nApproved)
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in IsKycApproved(" + orderId + ")", ex);
            }

            //int UserId = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + orderId.ToString);
            //int nApproved = eQuote.ExecuteScalar("SELECT COUNT(*) FROM KycFile WHERE NOT (Approved IS NULL) AND UserId = " + UserId.ToString);
            //int nTotal = eQuote.ExecuteScalar("SELECT COUNT(*) FROM KycFile WHERE UserId = " + UserId.ToString);
            //if (nApproved > 0)
            //{
            //    if (nTotal > nApproved)
            //    {
            //        return false;
            //    }
            //    else
            //    {
            //        if ((c != null))
            //            c.BackColor = Drawing.Color.FromName("#66FF66");
            //        return true;
            //    }
            //}
            //else
            //{
            //    return false;
            //}
        }

        public string GetPhoneCountry(long orderId)
        {
            try
            {
                var dc = new MonniData();
                //TODO SELECT ONLY FOR order and user
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                var user = dc.Users.FirstOrDefault(q => q.Id == order.UserId);
                if (user.CountryId == null)
                    return null;
                var countryCode = dc.Countries.FirstOrDefault(q => q.Id == user.CountryId).Code;
                trustLogic += "PhoneCountry:" + countryCode + ",\r\n";
                return countryCode;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetPhoneCountry(" + orderId + ")", ex);
            }

            //SELECT Country.Code FROM Country INNER JOIN[User] ON Country.Id = [User].CountryId INNER JOIN[Order] ON[User].Id = [Order].UserId WHERE[Order].Id = " + orderId

            //string phoneCode = eQuote.ExecuteScalar("SELECT Country.Code FROM Country INNER JOIN [User] ON Country.Id = [User].CountryId INNER JOIN [Order] ON [User].Id = [Order].UserId WHERE [Order].Id = " + orderId.ToString);
            //return phoneCode;
        }
        //
        public string GetCardCountry(long orderId)
        {
            try
            {
                var dc = new MonniData();
                //get only Info from transaction table for a particular txId
                var transaction = dc.Transactions.Where(q => q.OrderId == orderId && q.Type == 1).Select(q => new { q.Info }).FirstOrDefault();
                //get only Origin and PaymentGateway from order for a particular orderId
                var order = dc.Orders.Where(q => q.Id == orderId).Select(q => new { q.Origin, q.PaymentGatewayType }).FirstOrDefault();
                var bin = transaction.Info.Replace(" ", "").Substring(0, 6);
                //Below check conditions required to continue the flow in test and local
                if (order.PaymentGatewayType == "QuickPay" && (order.Origin.Contains("localhost") || order.Origin.Contains("test.")) )
                {
                    trustLogic += "Bin:" + bin + ",\r\n";
                    trustLogic += "BinListResponse:" + "DKK"+ ",\r\n";
                    return "DKK";
                }
                else
                {
                    if (transaction == null)
                        throw new Exception("No incoming transaction(1) for orderid:" + orderId);
                   
                    try
                    {
                        trustLogic += "Bin:" + bin + ",\r\n";
                        return GetBinListLookup(bin);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error creating binlist lookup request for bin:" + bin, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetCardCountry(" + orderId + ")", ex);
            }
        }

        private string GetBinListLookup(string bin)
        {
            var wc = new WebClient();
            wc.Headers.Add("accept-version", "3");
            var binListResponse = wc.DownloadString("https://lookup.binlist.net/" + bin);
            trustLogic += "BinListResponse:" + binListResponse.Substring(binListResponse.IndexOf("alpha2") + 9, 2) + ",\r\n";
            return binListResponse.Substring(binListResponse.IndexOf("alpha2") + 9, 2);
        }

        public string GetIpCountry(long orderId)
        {
            try
            {
                var dc = new MonniData();
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                trustLogic += "IpCode:" + order.IpCode + ",\r\n";
                if (string.IsNullOrEmpty(order.IpCode))
                    throw new Exception("Error getting IpCode.");
                return order.IpCode;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetIpCountry(" + orderId + ")", ex);
            }
        }

        public void AddNote(long orderId, string note)
        {
            try
            {
                var dc = new MonniData();
                var order = dc.Orders.FirstOrDefault(q => q.Id == orderId);
                var getMemoLogScript = string.Format("{0} at {1}{2}", string.Format(" {0:d}", DateTime.Now), string.Format(" {0:t}", DateTime.Now), note);
                trustLogic += "AddNote:getMemoLogScript = " + getMemoLogScript + ",\r\n";
                order.Note = getMemoLogScript;
                dc.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in AddNote(" + orderId + "," + note + ")", ex);
            }
        }
    }

    public class SpeedLimitSettings
    {
        public string PreviousDuration { get; set; }
        public string PreviousOrderAmount { get; set; }
    }
}
