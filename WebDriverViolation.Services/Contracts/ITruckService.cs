
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Contracts
{
    public interface ITruckService
    {
        List<TruckModel> GetAllTrucks(string companyName);
        Task<bool> CreateTruck(TruckModel model);
        Task<bool> UpdateTruck(TruckModel model);
        bool DeleteTruck(string id);
        TruckModel GetTruck(string id);
        List<TruckModel> GetAllActiveTrucks(string companyName);

        List<TruckModel> GetAllTrucks();

        Task<long> GetAllActiveTrucksCount(string companyName);

    }
}
