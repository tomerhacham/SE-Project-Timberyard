using AcceptanceTests.Client;

namespace AcceptanceTests.Utils
{
    public class SystemBridge
    {
        public static ITimberyardClient GetService()
        {
            TimberyardClientProxy proxy = new TimberyardClientProxy();
            // Uncomment when real application is ready
            proxy.RealClient = new TimberyardClientRealAdapter();
            return proxy;
        }
    }
}
