using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebDriverViolation.Models.Models;

namespace WebDriverViolation.Services.Models.MasterModels
{
    public class ViolationModel
    {
        public string Code { get; set; }
        public long Id { get; set; }
        [DefaultValue(false)]
        public bool IsDelted { get; set; }

        [DefaultValue(true)]
        public bool IsVisible { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string TruckID { get; set; }

        public Truck Truck  { get; set; }
        [Required]
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string? imageName { get; set; }

        public ViolationType ViolationType { get; set; }

        public int ViolationTypeID { get; set; }

        public ConfirmationStatus ConfirmationStatus { get; set; }

        public int ConfirmationStatusId { get; set; }
        public bool IsTrue { get; set; }

        public int ConfirmationViolationTypeID { get; set; }

        public string ConfirmationViolationTypeName { get; set; }

        public string? ConfirmedByUserId { get; set; }

        public string? ConfirmedByUserName { get; set; }
        public DateTime? ConfirmationDate { get; set; }

        public string ConfirmationDateText { get; set; }

        public Double Probability { get; set; }

        public Double AverageProbability { get; set; }
 
        public string AllClassessProbability { get; set; }

        public List<string> images { get; set; }

        public long ViolationTypeAccuracyLavelId { get; set; }

        public string ViolationAccuracyLevelDescription { get; set; }

        public int MailSent { get; set; }

        public double TotalTime { get; set; }

        public bool IsTruckMoving { get; set; }

        public List<double> PriobabilityOfViolationsWithSameCode { get; set; }
        public string? Category { get; set; }
    }
}
