using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebDriverViolation.Services.Contracts.Email
{
    public interface IEmailSender
    {
        public Task SendEmailAsync(string body, List<string> toMails, string subject, string[] ccMails = null);
    }
}
