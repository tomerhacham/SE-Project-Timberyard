using AcceptanceTests.Client;
using System;
using System.Collections.Generic;
using System.Text;
using WebService.Domain.Interface;

namespace AcceptanceTests.Utils
{
    public class SystemBridge
    {
        public static ITimberyardClient GetService()
        {
            TimberyardClientProxy proxy = new TimberyardClientProxy();
            // Uncomment when real application is ready
            // proxy.RealClient = new TimeryardClientRealAdapter();
            return proxy;
        }
    }
}
