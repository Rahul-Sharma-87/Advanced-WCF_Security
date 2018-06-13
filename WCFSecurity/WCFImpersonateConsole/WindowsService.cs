using System;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace WCFImpersonateService {

    public class CalculatorWindowsService : ServiceBase {

        public ServiceHost serviceHost = null;

        public CalculatorWindowsService() {
            // Name the Windows Service
            ServiceName = "WCFImpersonateWindowsService";
        }

        public static void Main() {
            ServiceBase.Run(new CalculatorWindowsService());
        }

        // Start the Windows service.
        protected override void OnStart(string[] args) {
            Debugger.Launch();
            if (serviceHost != null) {
                serviceHost.Close();
            }
            serviceHost = new ServiceHost(typeof(CalculatorService));
            string uri;
            var bind = Helper.GetTcpBinding(out uri,6354,true);
            serviceHost.AddServiceEndpoint(typeof(ICalculator), bind, uri);

            serviceHost.Credentials.ServiceCertificate.SetCertificate(
                StoreLocation.LocalMachine, 
                StoreName.My,
                X509FindType.FindByThumbprint, 
                "ff1075d6ae20d4d99b512d88a3f51cb967f29ecc"
            );
            serviceHost.Description.Behaviors.SetServiceAuthorizationBehavior();
            serviceHost.Open();
        }

        protected override void OnStop() {
            if (serviceHost != null) {
                serviceHost.Close();
                serviceHost = null;
            }
        }
    }

    // Provide the ProjectInstaller class which allows 
    // the service to be installed by the Installutil.exe tool
    //"C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe" "c:\myservice.exe"
    //"C:\Data\WCFHostingImpersonation\WCFImpersonateServiceLogger\Ouput\bin\WCFImpersonateService1.exe"
    [RunInstaller(true)]
    public class ProjectInstaller : Installer {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public ProjectInstaller() {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "WCFImpersonateWindowsService";
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}