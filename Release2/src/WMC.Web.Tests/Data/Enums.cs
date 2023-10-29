namespace WMC.Web.Tests.Data.Enums
{
    public enum AuditLogStatus : long
    {
        UserLogin = 1,
        YourPay = 2,
        Twilio = 3,
        IPInfo = 4,
        BitGo = 5,
        ApplicationError = 6,
        OrderBook = 7,
        PayLike = 8,
        PaymentError = 9,
        SentEmail = 10,
        TrustLogic = 11,
        TxSercret = 12,
        Admin = 13,
        TrustPay = 14,
        Chainalysis = 15
    }

    public enum OrderStatus : long
    {
        Cancel = 0,
        Quoted = 1,
        Paying = 2,
        /// <summary>
        /// Buy:
        /// Sell:transaction order received after recevied cryto from customer to merchant
        /// </summary>
        Paid = 3,
        KYCApprovalPending = 4,
        KYCApproved = 5,
        KYCDeclined = 6,
        AMLApprovalPending = 7,
        AMLApproved = 8,
        AMLDeclined = 9,
        Sending = 10,
        PayoutAwaitsApproval = 11,
        SendingAborted = 12,
        Sent = 13,
        ReleasingPayment = 14,
        ReleasingPaymentAborted = 15,
        ReleasedPayment = 16,
        Completed = 17,
        PayoutApproved = 18,
        PaymentAborted = 19,
        CaptureErrored = 20,
        ComplianceOfficerApproval = 21,
        CustomerResponsePending = 22,
        OrderCancelled = 23,
        KYCDecline = 24,
        ReleaseErrored = 25,
        EnhancedDueDiligence = 26,
        ReceivedCryptoPayment = 27

    }

    public enum AuditTrailLevel : long
    {
        Debug = 1,
        Info = 2,
        Warn = 3,
        Error = 4,
        Fatal = 5
    }

    public enum OrderType : long
    {
        Buy = 1,
        Sell = 2
    }

    public enum OrderPaymentType : long
    {
        CreditCard = 1,
        Bank = 2
    }

    public enum CurrencyTypes : long
    {
        Fiat = 1,
        Digital = 2
    }

    public enum FeesTypes : long
    {
        BitcoinMinersFees = 101,
        BitGoFees = 102
    }

    public enum ParticularType : long
    {
        NonFee = 0,
        MinersFee = 1,
        BitGoFee = 2
    }

    public static class AccountValueFor
    {
        public const string ToAccount = "ToAccount";
        public const string FromAccount = "FromAccount";
    }

    public enum CustomerTier : int
    {
        /// <summary>
        /// Default, customer in Tier1
        /// </summary>
        Tier1 = 0,
        /// <summary>
        /// KYC Approval Pending
        /// </summary>
        Tier2Pending = 1,
        /// <summary>
        /// Customer completed KYC approval
        /// </summary>
        Tier2 = 2,
        /// <summary>
        /// Enhanced Due Dilligence
        /// </summary>
        Tier3Pending = 3,
        /// <summary>
        /// Customer completed KYC approval and 'Enhanced Due Dilligence'
        /// </summary>
        Tier3 = 4,
    }
    public enum UserRiskLevelType : int
    {
        /// <summary>
        /// Default, customer is LowRisk
        /// </summary>
        LowRisk = 0,
        /// <summary>
        /// ElevatedRisk, if admin/customer service changes
        /// </summary>
        ElevatedRisk = 1,
        /// <summary>
        /// HighRisk, if admin/customer service changes
        /// </summary>
        HighRisk = 2
    }
}