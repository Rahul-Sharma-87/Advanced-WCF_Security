using System;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Diagnostics;

namespace WCFImpersonateService {
    public class CalculatorWindowsService : ServiceBase {
        public ServiceHost serviceHost = null;
        public CalculatorWindowsService() {
            // Name the Windows Service
            ServiceName = "WCFImpersonateWindowsService2";
        }

        public static void Main() {
            ServiceBase.Run(new CalculatorWindowsService());
        }

        // Start the Windows service.
        protected override void OnStart(string[] args) {
            if (serviceHost != null) {
                serviceHost.Close();
            }
            // Create a ServiceHost for the CalculatorService type and 
            // provide the base address.
            serviceHost = new ServiceHost(typeof(CalculatorService));
            string uri;
            var bind = Helper.GetTcpBinding(out uri,6355,true);
            serviceHost.AddServiceEndpoint(typeof(ICalculator), bind, uri);
            //if (Helper.enableSecurity) {
            serviceHost.Description.Behaviors.SetServiceAuthorizationBehavior();
            //}
            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
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
    [RunInstaller(true)]
    public class ProjectInstaller : Installer {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public ProjectInstaller() {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "WCFImpersonateWindowsService2";
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
