using System;
using System.Collections.Generic;
using System.Text;

namespace ETL.Utils
{
    public class Result<T>
    {
        public bool Status { get; }
        public string Message { get; }
        public T Data { get; }

        public Result(bool status, T data, string message="")
        {
            Status = status;
            Message = message;
            Data = data;
        }

       public void ContinueWith(Action<T> success, Action<T> fail=null)
        {
            if (Status)
            {
                success.DynamicInvoke(Data);
            }
            else
            {
                Console.WriteLine(Message);
                if (!(fail is null))
                {
                    fail.DynamicInvoke(Data);
                }
            }
        }

    }
}
