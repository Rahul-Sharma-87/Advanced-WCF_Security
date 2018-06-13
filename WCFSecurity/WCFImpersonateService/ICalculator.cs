using System.ServiceModel;

namespace WCFImpersonateService
{
    // Define a service contract.
    [ServiceContract(Namespace = "http://tempuri.org/ICalculator")]
    public interface ICalculator
    {
        [OperationContract]
        double Add(double n1, double n2);

        [OperationContract]
        double Subtract(double n1, double n2);

        [OperationContract]
        double Multiply(double n1, double n2);

        [OperationContract]
        double Divide(double n1, double n2);

        [OperationContract]
        bool CallOtherService(int port);

        [OperationContract]
        string ShowMessage(string serverStringMessage);
    }
}