using AcceptanceTests.Client;
using System.Runtime.InteropServices;

namespace AcceptanceTests.Utils
{
    public class TimberyardTestCase
    {
        protected ITimberyardClient Client;

        protected TimberyardTestCase([Optional] UserCredentials userCredentials)
        {
            Client = SystemBridge.GetService();

            if (userCredentials != null)
            {
                TimberyardClientRealAdapter adapter = Client as TimberyardClientRealAdapter;
                TimberyardClient realClient = adapter.RealClient as TimberyardClient;
                realClient.UserCredentials = userCredentials;
            }
        }
    }
}
