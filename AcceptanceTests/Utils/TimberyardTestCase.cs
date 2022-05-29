
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using TimberyardClient.Client;
using Xunit;

namespace AcceptanceTests.Utils
{
    [Trait("Category", "Acceptance")]

    public class TimberyardTestCase
    {
        protected ITimberyardClient Client;

        protected TimberyardTestCase([Optional] UserCredentials userCredentials)
        {
            Client = SystemBridge.GetService();

            if (userCredentials != null)
            {
                var clientProxy = Client as TimberyardClientProxy;
                var clientAdapter = clientProxy.RealClient as TimberyardClientRealAdapter;
                var client = clientAdapter.RealClient as TimberyardClient.Client.TimberyardClient;
                client.UserCredentials = userCredentials;
            }
        }
        protected ServiceProvider GetServiceProvider()
        {
            var clientProxy = Client as TimberyardClientProxy;
            var clientAdapter = clientProxy.RealClient as TimberyardClientRealAdapter;
            return clientAdapter.ServiceProvider;
        }
    }
}
