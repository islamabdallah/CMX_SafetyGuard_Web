using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Services.Models.APIModels;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Contracts
{
    public interface IOutlookSenderService
    {
        public string PrapareMailBody(ViolationModel model, string violationMessage);
        public Task SendEmail(string body, List<string> toMails, string subject, string[] ccMails = null);


    }
}
