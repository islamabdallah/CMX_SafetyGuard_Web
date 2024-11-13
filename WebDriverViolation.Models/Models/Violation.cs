using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using WebDriverViolation.Models.Models.Entity;

namespace WebDriverViolation.Models.Models.MasterModels
{
    public class Violation:EntityWithIdentityId<long>
    {
        [Required]
        public string Code { get; set; }

        public string? CarMovingStatus { get; set; }

        public Truck Truck { get; set; }

        [Required]
        public string TruckID { get; set; }

        public ViolationType ViolationType { get; set; }

        [Required]
        public int ViolationTypeID { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string? imageName { get; set; }

        public int? ConfirmationViolationTypeID { get; set; }

        public string? CameraPosition { get; set; }

        public ConfirmationStatus ConfirmationStatus { get; set; }

        [Required]
        public int ConfirmationStatusId { get; set; }

        public bool? IsTrue { get; set; }

        public string? ConfirmedByUserId { get; set; }

        public DateTime? ConfirmationDate { get; set; }

        [Required]
        public double Probability { get; set; }

        [Required]
        public double AverageProbability { get; set; }

        [Required]
        public string? AllClassessProbability { get; set; }

        public long? ViolationTypeAccuracyLavelId { get; set; }

        public int MailSent { get; set; }

        public double? TotalTime { get; set; }

        public bool? IsTruckMoving { get; set; }

        public string? Category { get; set; }
    }
}
