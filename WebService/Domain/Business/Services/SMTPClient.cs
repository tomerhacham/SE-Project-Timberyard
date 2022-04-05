using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebService.Domain.Business.Services
{
    public class SMTPClient : ISMTPClient
    {
        public Task SendEmail(string subject, string message, List<string> receivers)
        {
            throw new NotImplementedException();
        }
    }
}
