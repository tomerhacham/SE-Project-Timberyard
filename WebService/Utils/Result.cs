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
    }
}
