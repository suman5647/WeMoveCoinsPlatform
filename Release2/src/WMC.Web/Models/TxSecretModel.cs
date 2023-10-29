namespace WMC.Web.Models
{
    public class TxSecretModel
    {
        public string UserIdentity { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public long TxSecreteAttempts { get; set; }
        public long TxSecreteAttemptsFromDB { get; internal set; }
        public bool IsVerified { get; set; }
        public string TxSecreteMessage { get; set; }
        public decimal Commission { get; internal set; }
        public string OrderNumber { get; internal set; }
        public string OrderDate { get; internal set; }
    }
}