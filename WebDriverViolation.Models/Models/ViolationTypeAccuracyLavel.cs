using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Models.Models.Entity;

namespace WebDriverViolation.Models.Models
{
    public class ViolationTypeAccuracyLavel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        public int LevelId { get; set; }

        public ViolationType ViolationType { get; set; }

        public int ViolationTypeId { get; set; }

        public float LowestPercent { get; set; }

        public float HighestPercent { get; set; }

        [Required]
        public string Description { get; set; }

        public int Mode { get; set; }

        [DefaultValue(false)]
        public bool IsDelted { get; set; }

        [DefaultValue(true)]
        public bool IsVisible { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public DateTime EndDate { get; set; }


        public bool SendMail { get; set; }

    }
}
