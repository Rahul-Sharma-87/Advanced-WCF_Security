Analysis for possibility to provide just server side trust root certificate in WCF (TCP binding) to align with HTTP binding and client side WCF must pick based on client installed certificate with explicit settings.

Below are observations:
Tested all below scenarios with sample service with proper certificates and spanned across machines.

Option 1:
If we use windows authentication, we need to use windows stream security. Any server Certificate supplied along with this will be ignored. (Tested with sample application)

   binding.Security = new NetTcpSecurity {
                Mode = SecurityMode.Transport,
                Transport = {
                    ClientCredentialType = TcpClientCredentialType.Windows,
                    //Protection level none is authentication only
                    ProtectionLevel = ProtectionLevel.EncryptAndSign
            } } ;


Option 2 will not use windows authentication and providing the server certificate while hosting. Then client doesn’t need to supply certificate.
But this option excludes windows authentication, hence only Confidentiality and integrity.

At server hosting, below needs to be provided.

serviceHost.Credentials.ServiceCertificate.SetCertificate(
                StoreLocation.LocalMachine, 
                StoreName.My,
                X509FindType.FindByThumbprint, 
                "ff1075d6ae20d4d99b512d88a3f51cb967f29ecc"
            );

Downside is, since no windows authentication, hence not windows credentials will be sent to the server hence configured users and group based authorization will not work.


Third option, is use certificate based authentication and same will be used for confidentiality and integrity. In this case we need to provide certificate at both sides client and server.

Other approach is SecurityMode.TransportWithMessageCredential generally used to implement security for basic HTTP binding.

In this case message will be encrypted with the certificate and windows authentication also can be used, but option also requires certificate to be supplied at client side too.
- Requires certificate installed with private key. (.pfx certificate file)
- Configured users and group based authorization is not working.
