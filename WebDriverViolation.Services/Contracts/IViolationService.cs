using System;
using WebDriverViolation.Models.Models.MasterModels;
using WebDriverViolation.Services.Models.APIModels;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Contracts
{
    public interface IViolationService
    {
        Task<List<ViolationModel>> GetAllViolations();
      //   Task<ViolationModel> CreateViolation(ViolationAPIModel model, int TypeID, double AverageProbability, string TruckID, double totalTime);
        Task<List<ViolationModel>> CreateViolation(List<Violation> models);
        Task<List<ViolationModel>> GetViolationByType(int type);
        Task<List< ViolationModel>> GetViolationByDate(DateTime date);
         Task<string> SaveImage(string strm, string uploadFolder);

        Task<List<ViolationModel>> SearchForViolation(SearchViolationModel searchViolationModel);

        Task<SearchViolationModel> InitiateViolationSearchModel(SearchViolationModel searchViolationModel);

        ViolationModel GetViolation(long violationId);

        Task<bool> AddViolationConfirmationDetails(int violationConfirmedTypeId, long violationId, string userId);

         Task<Dictionary<string, long>> GetActualViolationCountsperType();

         Task<long> GetPendingViolationCount();

        Task<long> GetAllActualViolationCount();

        Task<List<ViolationModel>> GetAllPendingViolations();

        Task<long> GetCurrentMonthActualViolationCount();

         Task<string> GetLastViolationCodeForToday();

       Violation ConvertFromViolationAPIToViolation(ViolationAPIModel model, int TypeID, double AverageProbability, string TruckID, double TotalTime, bool IsTruckMoving, bool ModeLight);

        List<Violation> ConvertFromViolationAPIToViolation(List<ViolationAPIModel> models, string code);

        Task<List<ViolationModel>> GetViolationByCode(string code);

        Task<ViolationModel> GetLastViolationForSpecificTypeAndTruck(int type, string TruckID);

       Task<ViolationModel> GetLastViolationForSpecificTypeAndTruckWithMail(int typeID, string truckID);
    }
}
