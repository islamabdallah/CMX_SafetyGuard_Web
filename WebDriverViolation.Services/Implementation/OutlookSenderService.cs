using WebDriverViolation.Services.Contracts.Email;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models.APIModels;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Implementation
{
    public class OutlookSenderService : IOutlookSenderService
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmailSender _emailSender;

        public OutlookSenderService(IEmployeeService employeeService,
            IEmailSender emailSender)
        {
            _employeeService = employeeService;
            _emailSender = emailSender;
        }


        public string PrapareMailBody(ViolationModel model, string violationMessage)
        {
            string body = string.Empty;
            //string filePath = "D:/Cemex Project/gitHubProject/Cemex Backulaing/DevArea/Core/CoreServices/MailService/EmailTemplate.html";

            string fileName = "EmailBody.html";
            string path = Path.Combine(@"D:\_cemex\_projects\_driverviolation\WebDriverViolation\WebDriverViolation.Services\MailTemplate\", fileName);
            string imagePath = "http://20.86.97.165/DriverViolation/images/ViolationImages/" + model.imageName;

            using (StreamReader reader = new StreamReader(path))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Title}", "New Violation");
            body = body.Replace("{imgSrc}", imagePath);
            //body = body.Replace("{Violation Type}", "  " + model.TypeID.ToString());
            //body = body.Replace("{Truck Number}", "  " + model.TruckID.ToString());
            //body = body.Replace("{Date}", "  " + model.Date.ToString("yyyy-MM-dd"));
            //body = body.Replace("{Time}", "  " + model.Date.ToString("hh:mm:ss"));
            body = body.Replace("{Message}", violationMessage);
            return body;
        }

        public Task SendEmail(string body, List<string> toMails, string subject, string[] ccMails = null)
        {
            try
            {
                _emailSender.SendEmailAsync(body, toMails, subject, null);
                return Task<bool>.FromResult(true);
            }
            catch(Exception ex)
            {
                return Task<bool>.FromResult(false);
            }
        }

    }
}
