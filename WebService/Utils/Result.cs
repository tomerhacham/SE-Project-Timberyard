using System;

namespace WebService.Utils
{
    public class Result<T>
    {
        public bool Status { get; }
        public string Message { get; }
        public T Data { get; }

        public Result(bool status, T data, string message = "")
        {
            Status = status;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// Implementation of CPS for cleaner program flow
        /// </summary>
        /// <param name="success"></param>
        /// <param name="fail"></param>
        public void ContinueWith<T1>(Action<T> success, Action<T1> fail = null)
        {
            if (Status)
            {
                success.DynamicInvoke(Data);
            }
            else
            {
                if (!(fail is null))
                {
                    fail.DynamicInvoke(Message);
                }
            }
        }

    }
}
