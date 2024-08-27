
using WebDriverViolation.Services.Models.APIModels;
using WebDriverViolation.Services.Models.MasterModels;

namespace Take5.Services.Contracts
{
    public interface ITruckRunningTrackingService
    {
        Task<bool> CreateTruckRunningTracking(TruckRunningTrackingAPIModel truckRunningTrackingAPIModel);

        SearchTruckTrackingModel InitiateTruckTrackingSearchModel(SearchTruckTrackingModel searchTruckTrackingModel);
        List<TruckRunningTrackingAPIModel> SearchTruckRunningTracking(SearchTruckTrackingModel searchTruckTrackingModel);


    }
}
