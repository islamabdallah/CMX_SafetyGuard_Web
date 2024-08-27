using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebDriverViolation.Models.Auth
{
    public class UserModel
    {
         public string id { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        [Required]
        public long EmployeeNumber { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        [Required] [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public List<RoleModel> roles { get; set; }
        public RoleModel role { get; set; }
        public string roleId { get; set; }

        [NotMapped]
        public string[] AsignedRolesNames { get; set; }

    }
}
