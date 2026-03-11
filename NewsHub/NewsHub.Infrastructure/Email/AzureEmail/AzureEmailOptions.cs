using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Infrastructure.Email.AzureEmail
{
    public class AzureEmailOptions
    {
        public string ConnectionString { get; set; } = default!;
        public string From { get; set; } = default!;
    }
}
