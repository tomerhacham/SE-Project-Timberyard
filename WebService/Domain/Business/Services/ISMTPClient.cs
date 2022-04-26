using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebService.Utils;

namespace WebService.Domain.Business.Services
{
    public interface ISMTPClient
    {
        Task<Result<string>> SendEmail(string subject, string message, List<string> receivers);
    }
}
