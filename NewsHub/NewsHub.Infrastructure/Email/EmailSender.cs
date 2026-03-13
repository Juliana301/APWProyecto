using System;
using System.Collections.Generic;
using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;

namespace NewsHub.Infrastructure.Email
{
    public class EmailSender
    {
        private readonly EmailClient _client;
        private readonly string _from;

        public EmailSender(IConfiguration config)
        {
            _client = new EmailClient(config["AzureEmail:ConnectionString"])
                ?? throw new ArgumentNullException("AzureEmail:ConnectionString");

            _from = config["AzureEmail:From"] 
                ?? throw new ArgumentNullException("AzureEmail:From");
        }

        public async Task<bool> SendAsync(string to, string subject, string htmlbody)
        {
            var message = new EmailMessage(
                senderAddress: _from,
                content: new EmailContent(subject)
                {
                    Html = htmlbody
                },
                recipients: new EmailRecipients(
                    new List<EmailAddress> { new EmailAddress(to) }
                )
            );

            var response = await _client.SendAsync(Azure.WaitUntil.Completed, message);

            if (response.Value.Status != EmailSendStatus.Succeeded)
            {
                throw new Exception($"Email failed. Status: {response.Value.Status}");
            }

            return true;
        }
    }
}
