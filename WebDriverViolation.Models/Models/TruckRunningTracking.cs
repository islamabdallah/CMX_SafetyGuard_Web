using System.ComponentModel.DataAnnotations;
using WebDriverViolation.Models.Models.Entity;

namespace WebDriverViolation.Models.Models
{
    public class TruckRunningTracking:EntityWithIdentityId<long>
    {
        [Required]
        public Truck Truck { get; set; }

        [Required]
        public string TruckID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime LastStoppedDate { get; set; }
    }
}
