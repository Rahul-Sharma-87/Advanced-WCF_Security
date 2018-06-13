using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;


namespace WCFImpersonateService {
    public static class Helper {
        //public static bool enableSecurity = true;
        public static Binding GetTcpBinding(out string uri,int port, bool withOutSecurity = false) {
            UriBuilder uribuilder = new UriBuilder(
            "net.tcp",
            Dns.GetHostName(),
            port,
            "CalculatorService"
            );
            TransferMode mode = TransferMode.Streamed;
           
            NetTcpBinding binding =
                new NetTcpBinding {
                    OpenTimeout = TimeSpan.FromMinutes(60),
                    SendTimeout = TimeSpan.FromMinutes(60),
                    ReceiveTimeout = TimeSpan.MaxValue,
                    CloseTimeout = TimeSpan.FromMinutes(60),
                    TransferMode = mode,
                    MaxReceivedMessageSize = Int32.MaxValue,
                };

            binding.Security = new NetTcpSecurity {
                Mode = SecurityMode.Transport,
                Transport = { ClientCredentialType = TcpClientCredentialType.None}
            };
            binding.ReliableSession.Enabled = false;
            binding.ReliableSession.InactivityTimeout = TimeSpan.FromMinutes(60);
            uri = uribuilder.Uri.AbsoluteUri;
            return binding;
        }

        public static bool ExecuteServiceOperation(int port) {
            string uri;
            var myBinding = Helper.GetTcpBinding(out uri, port);
            var myEndpoint = new EndpointAddress(uri);
            ICalculator client = null;
            var myChannelFactory = new ChannelFactory<ICalculator>(myBinding, myEndpoint);

            try {
                client = myChannelFactory.CreateChannel();
                var data = client.Add(10, 10);
                if (data > 0) {
                    LogWritter("ExecuteServiceOperation non service with add data: "+ data);
                    LogWritter("ExecuteServiceOperation.data" + data);
                }
                ((ICommunicationObject) client).Close();
                return true;
            } catch(Exception ex) {
                LogWritter("ExecuteServiceOperation.exception: " + ex.Message+"Trace " + ex.StackTrace);
                if (client != null) {
                    ((ICommunicationObject) client).Abort();
                }
            }
            return false;
        }

        public static bool ExecuteServiceShowMessage(int port) {
            string uri;
            var myBinding = Helper.GetTcpBinding(out uri, port);
            var myEndpoint = new EndpointAddress(uri);
            ICalculator client = null;
           
            var myChannelFactory = new ChannelFactory<ICalculator>(myBinding, myEndpoint);
            try {
                myChannelFactory.Open();
                client = myChannelFactory.CreateChannel();
                var data = client.ShowMessage("This is message for the service.");
                Console.WriteLine(data);
                ((ICommunicationObject) client).Close();
                return true;
            } catch(Exception ex) {
                LogWritter("ExecuteServiceOperation.exception: " + ex.Message+"Trace " + ex.StackTrace);
                if (client != null) {
                    ((ICommunicationObject) client).Abort();
                }
            }
            return false;
        }

        /// <summary>
        /// Adds the service authorization behavior based on configuration
        /// </summary>
        /// <param name="serviceBehaviors"></param>
        public static void SetServiceAuthorizationBehavior(
            this KeyedByTypeCollection<IServiceBehavior> serviceBehaviors
        ) {
            bool addNewEntry = false;
            var serviceAuthorizationBehavior =
                serviceBehaviors.Find<ServiceAuthorizationBehavior>();
            if (serviceAuthorizationBehavior == null) {
                addNewEntry = true;
                serviceAuthorizationBehavior = new ServiceAuthorizationBehavior();
            }
            Console.WriteLine("Added service behavior with user group access");
            serviceAuthorizationBehavior.PrincipalPermissionMode = 
                PrincipalPermissionMode.UseWindowsGroups;
            serviceAuthorizationBehavior.ServiceAuthorizationManager = 
                new SecuredServiceAuthorizationManager(new List<string>(){"BUILTIN\\Administrators","NT AUTHORITY\\SYSTEM","CODE1\\gd_IN_Employees_HT"}, new List<string>(){"Test1"} );
            if (addNewEntry) {
                serviceBehaviors.Add(serviceAuthorizationBehavior);
            }
        }

        public static void LogWritter(string lines) {
            try
            {
                using (var file = new System.IO.StreamWriter("c:\\test.txt",append: true)) {
                    file.WriteLine(lines);
                }
            }
            catch (Exception ex) {
                //Ignore errors
            }
            
        }
    }
}
