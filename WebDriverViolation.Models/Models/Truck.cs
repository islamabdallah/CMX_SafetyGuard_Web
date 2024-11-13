using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Models.Models.Entity;

namespace WebDriverViolation.Models.Models
{
    public class Truck:Entity<string>
    {
        [Required]
        public string Company { get; set; }

        public string MailList { get; set; }

        public bool SendMail { get; set; }

        [Required]
        public string TruckName { get; set; }
        public string? CameraIp { get; set; }
        public string? Category { get; set; }
    }
}
