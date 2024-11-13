using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Models.Models.Entity;

namespace WebDriverViolation.Models.Models
{
    public class Camera : EntityWithIdentityId<long>
    {
        public string CameraName { get; set; }

        public string IP { get; set; }

        public string? User { get; set; }

        public string? Password { get; set; }

        public List<CameraPPS> CameraPPS { get; set; }
    }
}
