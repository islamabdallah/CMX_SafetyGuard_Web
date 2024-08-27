using WebDriverViolation.Services.Contracts; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebDriverViolation.Services.Implementation
{
    public class UserInputValidationService: IUserInputValidationService
    {
        public UserInputValidationService() { }

        public bool ContainsOnlyAlphaNumericCharacters(string inputString) 
        { 
            var regexItem = new Regex("^[a-zA-Z0-9,.-]*$"); 
            return regexItem.IsMatch(inputString); 
        }

        public bool IsValidDateFormat(string inputDate)
        {
            var regexItem = new Regex("^((?:19|20)[0-9][0-9])-(0?[1-9]|1[012])-(0?[1-9]|[12][0-9]|3[01])$");
            return regexItem.IsMatch(inputDate);
        }
    }
}
