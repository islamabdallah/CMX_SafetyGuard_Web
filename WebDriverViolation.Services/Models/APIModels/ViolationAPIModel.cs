using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebDriverViolation.Services.Models.APIModels
{

    [Bind( 
        nameof(ViolationAPIModel.Date),
        nameof(ViolationAPIModel.image),
        nameof(ViolationAPIModel.Probability))]
    public class ViolationAPIModel
    {
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public string? image { get; set; }

        [Required]
        public Double Probability { get; set; }

        [Required]
        public string AllClassessProbability { get; set; }
    }

    public class ViolationCollection
    {
        [Required]

        public string TruckID { get; set; }

        [Required]
        public int TypeID { get; set; }

        [Required]
        public Double AverageProbability { get; set; }

         public bool ModeLight { get; set; }

        public double TotalTime { get; set; }
        public bool ModeMoving { get; set; }


        public List<ViolationAPIModel> ViolationAPIModels { get; set; }

    }
}
