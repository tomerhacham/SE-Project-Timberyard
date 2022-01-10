using System;
using System.Collections.Generic;
using System.Text;
using WebService.Domain.Interface;

namespace AcceptanceTests.Utils
{
    public class SystemBridge
    {
        public static ISystemInterface GetService()
        {
            SystemInterfaceProxy proxy = new SystemInterfaceProxy();
            // Uncomment when real application is ready
            proxy.Real = new SystenRealAdapter();
            return proxy;
        }
    }
}
