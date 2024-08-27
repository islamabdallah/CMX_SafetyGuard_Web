using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WebDriverViolation.Models.Models.Entity;
using WebDriverViolation.Models.Models.MasterModels;

namespace WebDriverViolation.Models.Models.MasterModels
{
    public class ViolationNotification:EntityWithIdentityId<long>
    {

        [Required]
        public string Message { get; set; }

        [Required]
        public Violation Violation { get; set; }

        [Required]
        public long ViolationID  { get; set; }


    }
}
