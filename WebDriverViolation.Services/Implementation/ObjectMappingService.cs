using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Services.Contracts;

namespace WebDriverViolation.Services.Implementation
{
    public class ObjectMappingService:IObjectMappingService
    {
        private readonly IUserInputValidationService _userInputValidationService;
        public ObjectMappingService(IUserInputValidationService userInputValidationService) 
        { 
            _userInputValidationService= userInputValidationService;
        }   

        public bool ValidateObjectProperities(object myObject)
        {
            bool result = false;
            foreach (var prop in myObject.GetType().GetProperties())
            {
                if(prop.PropertyType.Name.ToString() == "String")
                {
                    if(prop.GetValue(myObject, null) != null)
                    {
                        result = _userInputValidationService.ContainsOnlyAlphaNumericCharacters(prop.GetValue(myObject, null).ToString());
                        if (!result)
                        {
                            break;
                        }
                    }
                }
                else if (prop.PropertyType.Name.ToString() == "DateTime")
                {
                    string input =Convert.ToDateTime(prop.GetValue(myObject, null)).ToString("yyyy-MM-dd");
                    result = _userInputValidationService.IsValidDateFormat(input);
                    if (!result)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        public bool ValidateString(string stringInput)
        {
          try
            {
                bool result = _userInputValidationService.ContainsOnlyAlphaNumericCharacters(stringInput);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
           
        }

    }
}
