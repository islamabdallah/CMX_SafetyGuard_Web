
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using WebDriverViolation.Service.Models.Email;
using WebDriverViolation.Services.Contracts.Email;

namespace WebDriverViolation.Service.Implementation.Email
{
   public class MailKitEmailSenderService : IEmailSender
    {
        public MailKitEmailSenderService(IOptions<MailKitEmailSenderOptions> options
           )
        {
            this.Options = options.Value;
        }

        public MailKitEmailSenderOptions Options { get; set; }

        public Task SendEmailAsync(string body, List<string> toMails, string subject, string[] ccMails = null)
        {
            var message = new MimeMessage();
            foreach (string to in toMails)
            {

                    message.To.Add(MailboxAddress.Parse(to));
            }
            if (ccMails != null)
            {
                foreach (string cc in ccMails)
                {
                    message.To.Add(MailboxAddress.Parse(cc));
                }
                message.To.Add(MailboxAddress.Parse("doaa.abdel@ext.cemex.com"));
            }
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html) { Text = body };
            return Execute(message);
        }
        public Task<bool> Execute(MimeMessage email)
        {
            // create message
            try
            {
                email.Sender = MailboxAddress.Parse(Options.Sender_EMail);
                if (!string.IsNullOrEmpty(Options.Sender_Name))
                    email.Sender.Name = Options.Sender_Name;
                email.From.Add(email.Sender);

                // send email
                using (var smtp = new SmtpClient())
                {
                    smtp.Connect(Options.Host_Address, Options.Host_Port, Options.Host_SecureSocketOptions);
                   // smtp.Authenticate(Options.Username, Options.Host_Password);
                    smtp.Send(email);
                    smtp.Disconnect(true);
                    smtp.Dispose();
                }

                return Task.FromResult(true);
            }
            catch(Exception e)
            {
                return Task.FromResult(false);
            }
        }
    }
}
