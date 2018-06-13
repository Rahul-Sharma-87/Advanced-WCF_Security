namespace WCFImpersonateService {
     // Implement the ICalculator service contract in a service class.
    public class CalculatorService : ICalculator {
        // Implement the ICalculator methods.
        public double Add(double n1, double n2) {
            Helper.LogWritter("CalculatorService.Add called");
            double result = n1 + n2;
            return result;
        }

        public string ShowMessage(string serverStringMessage) {
            return serverStringMessage + "This is message from Client";
        }

        public double Subtract(double n1, double n2) {
            Helper.LogWritter("CalculatorService.Subtract called");
            double result = n1 - n2;
            return result;
        }

        public double Multiply(double n1, double n2) {
            Helper.LogWritter("CalculatorService.Multiply called");
            double result = n1 * n2;
            return result;
        }

        public double Divide(double n1, double n2) {
            Helper.LogWritter("CalculatorService.Divide called");
            double result = n1 / n2;
            return result;
        }

        public bool CallOtherService(int port) {
            Helper.LogWritter("CalculatorService.ExecuteServiceOperation called on port: " + port );
            return Helper.ExecuteServiceOperation(port);
        }
    }

    public class ScientificCalculatorService : ICalculator {
        // Implement the ICalculator methods.
        public double Add(double n1, double n2) {
            Helper.LogWritter("CalculatorService.Add called");
            double result = n1 + n2;
            return result;
        }

        public double Subtract(double n1, double n2) {
            Helper.LogWritter("CalculatorService.Subtract called");
            double result = n1 - n2;
            return result;
        }

        public double Multiply(double n1, double n2) {
            Helper.LogWritter("CalculatorService.Multiply called");
            double result = n1 * n2;
            return result;
        }

        public double Divide(double n1, double n2) {
            Helper.LogWritter("CalculatorService.Divide called");
            double result = n1 / n2;
            return result;
        }
        
        public bool CallOtherService(int port) {
            Helper.LogWritter("CalculatorService.ExecuteServiceOperation called on port: " + port );
            return Helper.ExecuteServiceOperation(port);
        }

        public string ShowMessage(string serverStringMessage) {
            return serverStringMessage + "This is message from Client";
        }
    }
}
