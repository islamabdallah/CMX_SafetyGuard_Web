using WebDriverViolation.Models.Models;

namespace WebDriverViolation.Services.Models.MasterModels
{
    public class SearchViolationModel
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public int ViolationTypeID { get; set; }

        public List<ViolationTypeModel> ViolationTypeModels { get; set; }

        public List<TruckModel> Trucks { get; set; }
        public string SelectedTruckID { get; set; }

        public List<ViolationModel> ViolationModels { get; set; }

        public List<int> SelectedTypes { get; set; }

        public List<ConfirmationStatusModel> ConfirmationStatusModel { get; set; }

        public int SelectedConfirmationStatusId { get; set; }

        public int TruckStatusId { get; set; }

    }
}
