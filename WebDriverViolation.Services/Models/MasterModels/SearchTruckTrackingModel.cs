using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Services.Models.APIModels;

namespace WebDriverViolation.Services.Models.MasterModels
{
    public class SearchTruckTrackingModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public List<TruckModel> Trucks { get; set; }
        public string SelectedTruckID { get; set; }

        public List<TruckRunningTrackingAPIModel> TruckRunningTrackingAPIModels { get; set; }
    }
}
