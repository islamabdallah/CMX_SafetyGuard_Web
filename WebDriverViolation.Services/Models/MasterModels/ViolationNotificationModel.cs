using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Models.Models.MasterModels;

namespace WebDriverViolation.Services.Models.MasterModels
{
    public class ViolationNotificationModel
    {
        public long Id { get; set; }

        [DefaultValue(false)]
        public bool IsDelted { get; set; }

        [DefaultValue(true)]
        public bool IsVisible { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedDateText { get; set; }
        public string CreatedTimeText { get; set; }
        [Required]
        public string Message { get; set; }

        [Required]
        public Violation Violation { get; set; }

        [Required]
        public long ViolationID { get; set; }
    }
}
