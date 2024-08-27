using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebDriverViolation.Services.Models.MasterModels
{
    public class TruckModel
    {
        public string Id { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool IsDelted { get; set; }

        [DefaultValue(true)]
        public bool IsVisible { get; set; }
        public DateTime CreatedDate { get; set; } 
        public DateTime UpdatedDate { get; set; }

        public string MailList { get; set; }

        public bool SendMail { get; set; }

        public DateTime LastSendTime { get; set; }


        public string Company { get; set; }

        [Required]
        public string TruckName { get; set; } = string.Empty;

    }
}
