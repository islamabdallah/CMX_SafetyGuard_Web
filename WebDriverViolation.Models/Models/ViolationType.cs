using System.ComponentModel.DataAnnotations;
using WebDriverViolation.Models.Models.Entity;

namespace WebDriverViolation.Models.Models
{
    public class ViolationType:Entity<int>
    {
        [Required]
        public string Name { get; set; }


        public int MinutesNumberToSendNewMail { get; set; }
    }
}
