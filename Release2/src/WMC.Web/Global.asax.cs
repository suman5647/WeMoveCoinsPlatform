using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FluentScheduler;
using WMC.FaceTec;
using WMC.Logic;
using WMC.Utilities;
using WMC.Web.ModelBinders;
using WMC.Web.Utilities;

namespace WMC.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            try
            {
                //Remove All Engine
                ViewEngines.Engines.Clear();
                //Add Razor Engine
                ViewEngines.Engines.Add(new RazorViewEngine());
                GlobalConfiguration.Configure(RouteConfig.Register);
                GlobalConfiguration.Configure(FaceTecHttpProxy.Register);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
                System.Web.Mvc.ModelBinders.Binders.Add(typeof(KYCFileInfo), new KYCFileInfoBinder());
                JobManager.JobException += JobManager_JobException;
                JobManager.AddJob(() => { Application["LatestExchangeRates"] = OpenExchangeRates.GetLatestExchangeRates().Rates; }, (a) => a.ToRunNow().AndEvery(1).Days());
                JobManager.AddJob(() =>
                {
                    Application["LatestBTCEURRate"] = KrakenExchange.GetBTCEURRate();
                    if (Application["LatestExchangeRates"] == null)
                        Application["LatestExchangeRates"] = OpenExchangeRates.GetLatestExchangeRates().Rates;
                }, (a) => a.ToRunNow().AndEvery(5).Seconds());
            }
            catch (System.Exception ex)
            {
                AuditLog.log("Error in App Start. Exception details : " + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        private void JobManager_JobException(JobExceptionInfo sender, FluentScheduler.UnhandledExceptionEventArgs e)
        {
            AuditLog.log("Error in Jobs in App. Exception details : " + e.ExceptionObject.ToString(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

            // Get the exception object.
            Exception exc = Server.GetLastError();

            // Handle HTTP errors
            if (exc.GetType() == typeof(HttpException))
            {
                // The Complete Error Handling Example generates
                // some errors using URLs with "NoCatch" in them;
                // ignore these here to simulate what would happen
                // if a global.asax handler were not implemented.
                if (exc.Message.Contains("NoCatch") || exc.Message.Contains("maxUrlLength"))
                    return;

                //Redirect HTTP errors to HttpError page
                Server.Transfer("HttpErrorPage.aspx");
            }

            // For other kinds of errors give the user some information
            // but stay on the default page
            Response.Write("<h2>Global Page Error</h2>\n");
            Response.Write("<p>" + exc.Message + "</p>\n");
            Response.Write("<p>" + exc.ToMessageAndCompleteStacktrace() + "</p>\n");
            Response.Write("Return to the <a href='Default.aspx'>" +
                "Default Page</a>\n");

            // Log the exception and notify system operators
            ExceptionUtility.LogException(exc, "DefaultPage");
            ExceptionUtility.NotifySystemOps(exc);

            // Clear the error from the server
            Server.ClearError();
        }
    }
}