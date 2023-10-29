namespace WMC.Logic
{
    public interface IAuditLog
    {
        void RecordLog(string message, long status, long auditTrailLevel, long? orderId = null);
    }
}
