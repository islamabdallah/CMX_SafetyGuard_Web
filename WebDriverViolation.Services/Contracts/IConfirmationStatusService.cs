using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Contracts
{
    public interface IConfirmationStatusService
    {
        List<ConfirmationStatusModel> GetAllConfirmationStatuses();
    }
}
