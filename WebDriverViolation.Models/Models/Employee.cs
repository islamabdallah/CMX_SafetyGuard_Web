using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WebDriverViolation.Models.Models.MasterModels
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long EmployeeNumber { get; set; }

        [Required]
        public string EmployeeName { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        [Required]
        [DefaultValue(false)]
        public bool IsDelted { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsVisible { get; set; }

        [Required]
        public string UserId { get; set; }


        [Required]
        [MaxLength(11)]
        public string PhoneNumber { get; set; }


        [Required]
        public string Company { get; set; }

    }
}
