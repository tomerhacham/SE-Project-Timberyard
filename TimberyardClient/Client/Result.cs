using System;
using System.Collections.Generic;
using System.Text;

namespace TimberyardClient.Client
{
    public class Result<T>
    {
        public bool Status { get; }
        public string Message { get; }
        public T Data { get; }

    }
}
