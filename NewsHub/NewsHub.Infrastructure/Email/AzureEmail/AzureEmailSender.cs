using Azure.Communication.Email;
using Microsoft.Extensions.Options;
using NewsHub.Application.Interfaces.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Infrastructure.Email.AzureEmail
{
    public class AzureEmailSender : IEmailSender
    {
        private readonly EmailClient _client;
        private readonly string _from;

        public AzureEmailSender(IOptions<AzureEmailOptions> options)
        {
            var opt = options.Value;

            _client = new EmailClient(opt.ConnectionString);
            _from = opt.From;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            var message = new EmailMessage(
                senderAddress: _from,
                content: new EmailContent(subject) { Html = htmlBody },
                recipients: new EmailRecipients(
                    new List<EmailAddress> { new EmailAddress(to) }
                )
            );

            await _client.SendAsync(Azure.WaitUntil.Completed, message);
        }
    }
}
