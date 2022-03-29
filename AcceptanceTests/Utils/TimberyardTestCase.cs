using AcceptanceTests.Client;
using System;
using System.Collections.Generic;
using System.Text;
using WebService.Domain.Interface;

namespace AcceptanceTests.Utils
{
    public class TimberyardTestCase
    {
        protected ITimberyardClient Client;

        protected TimberyardTestCase()
        {
            Client = SystemBridge.GetService();
        }
    }
}
