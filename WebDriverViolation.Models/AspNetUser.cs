using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebDriverViolation.Models.Models
{
    public class AspNetUser: IdentityUser
    {
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        //[Required]
        //public Company Company { get; set; }

        //[Required]
        //public int CompanyId { get; set; }
        [Required]
        public string Company { get; set; }

    }
}
