using System;
using System.Collections.Generic;
using System.Text;
using WebService.Domain.Interface;

namespace AcceptanceTests.Utils
{
    public class TimberyardTestCase
    {
        protected ISystemInterface sut;

        protected TimberyardTestCase()
        {
            sut = SystemBridge.GetService();
        }
    }
}
