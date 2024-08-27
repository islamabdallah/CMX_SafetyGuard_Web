using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Take5.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Models.APIModels
{
    [Bind(nameof(TruckRunningTrackingAPIModel.TruckID), nameof(TruckRunningTrackingAPIModel.StartDate), 
        nameof(TruckRunningTrackingAPIModel.LastStoppedDate))]
    public class TruckRunningTrackingAPIModel
    {
        [Required]
        public string TruckID { get; set; }


        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime LastStoppedDate { get; set; }

    }
}
