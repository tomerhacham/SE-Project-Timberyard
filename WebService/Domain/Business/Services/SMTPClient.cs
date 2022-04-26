using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Smtp;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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

            List<Address> addressList = new List<Address>();
            foreach (string recever in receivers)
            {
                addressList.Add(new Address(recever));
            }

            try
            {
                var email = await Email
                    .From(ClientSettings.SenderAddress)
                    .To(addressList)
                    .Subject(subject)
                    .Body(message)
                    .SendAsync();

                return new Result<String>(true, "Email sent", "");
            }
            catch (Exception e)
            {
                return new Result<String>(false, "Could'nt sent email", e.Message);
            }

        }
    }
}
