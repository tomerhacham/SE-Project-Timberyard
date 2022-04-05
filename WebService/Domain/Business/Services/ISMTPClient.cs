using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Domain.Business.Services
{
    public interface ISMTPClient
    {
        Task SendEmail(string subject, string message, List<string> receivers);
    }
}
