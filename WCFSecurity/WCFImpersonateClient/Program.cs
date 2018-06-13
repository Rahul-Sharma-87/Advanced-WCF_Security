
using WCFImpersonateService;

namespace WCFImpersonateClient {
    public class Program {
        static void Main(string[] args) {
            Helper.LogWritter("***ExecuteServiceOperation****on port" + 6354 +"\r\n");
            Helper.ExecuteServiceShowMessage(6354);
        }
       
    }
}
