using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebDriverViolation.Models.Auth;

namespace WebDriverViolation.Services.Models.MasterModels
{
   public class EmployeeModel
    {
        [Required]
        public long EmployeeNumber { get; set; }

        [Required]
        public string EmployeeName { get; set; }

        [Required]
        public string UserId { get; set; }


        [Required]
        [MaxLength(11)]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } 

        [Required]
        public DateTime UpdatedDate { get; set; } 

        [Required]
        [DefaultValue(false)]
        public bool IsDelted { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsVisible { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public List<RoleModel> roles { get; set; }
        [Required]
        [NotMapped]
        public string roleId { get; set; }

        [NotMapped]
        public string[] AsignedRolesNames { get; set; }
        public string roleName { get; set; }


        [Required]
        public string Company { get; set; }


    }
}
