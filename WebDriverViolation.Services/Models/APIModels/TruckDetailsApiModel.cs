using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDriverViolation.Services.Models.APIModels
{
    public class TruckDetailsApiModel
    {
        public string TruckId { get; set; }
        public string? Speed { get; set; }
        public DateTime Date { get; set; }
        public string? Rbm { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? LastSpeed { get; set; }
        public string? Duration { get; set; }
        public string? Fuel_Level { get; set; }
    }
}
