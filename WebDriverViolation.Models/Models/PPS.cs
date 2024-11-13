using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Models.Models.Entity;

namespace WebDriverViolation.Models.Models
{
    public class PPS : EntityWithIdentityId<long>
    {
        public string PPSName { get; set; }

        public List<WebDriverViolation.Models.Models.CameraPPS> CameraPPS { get; set; }
    }
}
