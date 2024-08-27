using System.Threading.Tasks;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Contracts
{
    public interface IViolationTypeService
    {
        Task<List<ViolationTypeModel>> GetAllViolationTypes();
        ViolationTypeModel GetViolationType(int id);

    }
}
