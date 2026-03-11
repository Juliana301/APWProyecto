using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Interfaces.Notifications
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string htmlBody);
    }
}
