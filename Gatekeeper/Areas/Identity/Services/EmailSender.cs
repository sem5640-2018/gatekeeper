using Gatekeeper.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gatekeeper.Areas.Identity.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ICommsApiClient client;

        public EmailSender(ICommsApiClient commsClient)
        {
            client = commsClient;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var payload = new
            {
                EmailAddress = email,
                Subject = subject,
                Content = htmlMessage
            };
            await client.PostAsync("api/Email/ToEmail", payload);
        }
    }
}
