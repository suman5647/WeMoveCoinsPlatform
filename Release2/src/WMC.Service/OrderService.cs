using FluentScheduler;
using SimpleServices;
using System.Globalization;
using System.Threading;
using WMC.Logic;
using WMC.Utilities;
using WMC.Data.Enums;
using System.Configuration;

namespace Hafniatrading.Service
{
    public partial class OrderService : IWindowsService
    {
        public ApplicationContext AppContext { get; set; }
        public OrderService()
        {
        }

        public void Start(string[] args)
        {
            SendStatusEmail("Started");
            AuditLog.log("Order Service Started", (int)AuditLogStatus.UserLogin, (int)AuditTrailLevel.Debug);
            var orderLogic = new OrderLogic();
//#if RUN_AT_LOCAL
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            //orderLogic.Rates = OpenExchangeRates.GetLatestExchangeRates().Rates;
            //orderLogic.UpdateOrderRateFromKrakenOrderBook();
            //orderLogic.UpdateMinersFee();
            //orderLogic.ProcessKYC();
            //orderLogic.ProcessOrderApproval();
            //orderLogic.ProcessOrderAwaitsApproval();
            //orderLogic.CapturePayment();
            //orderLogic.ProcessOrder();
            //orderLogic.NotifyOrderComplete();
            //orderLogic.ProcessKYCDecline();
            //orderLogic.ProcressKYCRequest();
            //orderLogic.ProcessCancelledOrders();
//#else
            JobManager.JobException += JobManager_JobException;
            JobManager.AddJob(() => { orderLogic.Rates = OpenExchangeRates.GetLatestExchangeRates().Rates; }, (a) => a.ToRunNow().AndEvery(1).Hours());
            // TODO: dont we need this?
            //JobManager.AddJob(() => { Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); orderLogic.UpdateMinersFee(); }, (a) => a.ToRunNow().AndEvery(5).Minutes());
            JobManager.AddJob(() => { Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); orderLogic.OrderAMLProcess(); }, (a) => a.NonReentrant().ToRunEvery(2).Minutes());
            JobManager.AddJob(() => { Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); orderLogic.CheckUserName(); }, (a) => a.NonReentrant().ToRunEvery(1).Minutes());
            JobManager.AddJob(() => { Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); orderLogic.UpdateOrderRateFromKrakenOrderBook(); }, (a) => a.NonReentrant().ToRunEvery(1).Minutes());
            JobManager.AddJob(() => { Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); orderLogic.ProcessKYC(); }, (a) => a.NonReentrant().ToRunEvery(3).Minutes());
            JobManager.AddJob(() => { Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); orderLogic.ProcessOrderApproval(); }, (a) => a.NonReentrant().ToRunEvery(1).Minutes());
            JobManager.AddJob(() => { Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); orderLogic.ProcessOrder(); }, (a) => a.NonReentrant().ToRunEvery(1).Minutes());
            JobManager.AddJob(() => { Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); orderLogic.ProcessOrderAwaitsApproval(); }, (a) => a.NonReentrant().ToRunEvery(1).Minutes());
            JobManager.AddJob(() => { Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); orderLogic.CapturePayment(); }, (a) => a.NonReentrant().ToRunEvery(1).Minutes());
            JobManager.AddJob(() => { Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); orderLogic.NotifyOrderComplete(); }, (a) => a.NonReentrant().ToRunEvery(1).Minutes());
            JobManager.AddJob(() => { Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); orderLogic.ProcessKYCDecline(); }, (a) => a.NonReentrant().ToRunEvery(1).Minutes());
            // TODO: ProcressKYCRequest not used anymore (obsolete)??
            JobManager.AddJob(() => { Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); orderLogic.ProcressKYCRequest(); }, (a) => a.NonReentrant().ToRunEvery(1).Minutes());
            JobManager.AddJob(() => { Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); orderLogic.ProcessCancelledOrders(); }, (a) => a.NonReentrant().ToRunEvery(1).Minutes());
            JobManager.AddJob(() => { Thread.CurrentThread.CurrentCulture = new CultureInfo("en-Us"); orderLogic.AuditTrailCleanUp(); }, (a) => a.NonReentrant().ToRunEvery(3).Months());
//#endif
        }

        private void JobManager_JobException(JobExceptionInfo obj)
        {
            AuditLog.log("Error in Job " + obj.Name + ". Exception details : " + obj.Exception.ToMessageAndCompleteStacktrace(), (int)AuditLogStatus.ApplicationError, (int)AuditTrailLevel.Error);
        }

        public void Stop()
        {
            SendStatusEmail("Stopped");
            AuditLog.log("Order Service Stopped", (int)AuditLogStatus.UserLogin, (int)AuditTrailLevel.Debug);
        }

        public void SendStatusEmail(string status)
        {
            try
            {
                var serverSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<SMTPServerSettings2>(ConfigurationManager.AppSettings["statusEmailSettings"]);
                EmailHelper.SendSimpleEmail(serverSettings,
                      serverSettings.To.ToArray(),
                      $"IMP: Order Service ({serverSettings.Environment}): {status}",
                      $"{serverSettings.Environment} Order Service is been {status}",
                      serverSettings.From,
                      $"Monni Apps ({serverSettings.Environment} Service)",
                      null,
                      false);
            }
            catch { }
        }
    }
}