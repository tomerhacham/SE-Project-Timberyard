using System.Runtime.InteropServices;
using TimberyardClient.Client;

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
                TimberyardClient.Client.TimberyardClient realClient = adapter.RealClient as TimberyardClient.Client.TimberyardClient;
                realClient.UserCredentials = userCredentials;
            }
        }
    }
}
