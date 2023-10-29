using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceProcess;
using SimpleServices;
using System;
using System.Configuration;

namespace Hafniatrading.Service
{
    [RunInstaller(true)]
    public class Program : SimpleServiceApplication
    {
        public static void Main(string[] args)
        {
            var serviceName = ConfigurationManager.AppSettings["serviceName"];
#if RUN_AT_LOCAL
            new OrderService().Start(new string[] { "" });
            Console.ReadKey();
#else
            new SimpleServices.Service(args, new List<IWindowsService> { new OrderService() }.ToArray,
            installationSettings: (serviceInstaller, serviceProcessInstaller) =>
            {
                serviceInstaller.ServiceName = serviceName;
                serviceInstaller.StartType = ServiceStartMode.Automatic;
                serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            }, configureContext: x => { }).Host();
#endif
        }
    }
}
