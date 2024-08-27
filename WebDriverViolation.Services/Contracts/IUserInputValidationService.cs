using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDriverViolation.Services.Contracts
{
    public interface IUserInputValidationService
    {
        public bool ContainsOnlyAlphaNumericCharacters(string inputString);
        public bool IsValidDateFormat(string inputDate);

    }
}
