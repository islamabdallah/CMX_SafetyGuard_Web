using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Models.Models;

namespace WebDriverViolation.Services.Models.MasterModels
{
    public class CameraViolationModel
    {
        public string CameraId { get; set; }

        private List<CameraViolation> cameraViolations { get; set; }
    }
}
