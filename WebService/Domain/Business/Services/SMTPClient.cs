using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Smtp;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WebService.Utils;
using WebService.Utils.Models;

namespace WebService.Domain.Business.Services
{
    public class SMTPClient : ISMTPClient
    {

        //Properties
        public SMPTClientSettings ClientSettings { get; set; }
        public ILogger Logger { get; set; }
        private SmtpSender Sender { get; set; }

        public SMTPClient(IOptions<SMPTClientSettings> clientSettings, ILogger logger)
        {
            ClientSettings = clientSettings.Value;
            Logger = logger;

            Sender = new SmtpSender(() => new System.Net.Mail.SmtpClient(ClientSettings.Host)
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Port = ClientSettings.Port
            });
        }

        public async Task<Result<string>> SendEmail(string subject, string message, List<string> receivers)
        {
            Email.DefaultSender = Sender;
            List<Address> addressList = receivers.ConvertAll(receiver => new Address(receiver));
            try
            {
                var emailResponse = await Email
                    .From(ClientSettings.SenderAddress)
                    .To(addressList)
                    .Subject(subject)
                    .Body(message)
                    .SendAsync();

                return new Result<string>(emailResponse.Successful, emailResponse.Successful ? emailResponse.MessageId :
                                                                                              emailResponse.ErrorMessages.Aggregate(new StringBuilder(),
                                                                                              (sb, a) => sb.AppendLine(String.Join(",", a)), sb => sb.ToString()));
            }
            catch (Exception e)
            {
                return new Result<string>(false, "Could'nt sent email", e.Message);
            }

        }
    }
}
