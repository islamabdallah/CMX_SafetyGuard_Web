using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDriverViolation.Services.Contracts
{
    public interface IObjectMappingService
    {
        public bool ValidateObjectProperities(object myObject);

        public bool ValidateString(string stringInput);

    }
}
