using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Services.Models.APIModels;

namespace WebDriverViolation.Services.Models.MasterModels
{
    public class CameraTrackingModel
    {
        public List<TruckModel> Trucks { get; set; }

        public string SelectedTruckID { get; set; }

        public int ViolationTypeID { get; set; }

        public List<ViolationTypeModel> ViolationTypeModels { get; set; }

        public List<int> SelectedTypes { get; set; }

        public List<CameraViolationModel> cameraViolationModels { get; set; }
    }
}
