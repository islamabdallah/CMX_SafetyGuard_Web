using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Models.Models.MasterModels;

namespace WebDriverViolation.Services.Models.MasterModels
{
    public class UserViolationNotificationModel
    {
        [Required]
        public string userId { get; set; }

        [Required]
        public long ViolationNotificationId { get; set; }
        [Required]
        public ViolationNotification ViolationNotification { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool IsDelted { get; set; }
        [Required]
        [DefaultValue(true)]
        public bool IsVisible { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        [Required]
        public bool Seen { get; set; }
    }
}
