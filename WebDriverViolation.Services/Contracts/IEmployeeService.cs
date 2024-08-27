
using WebDriverViolation.Models.Models.MasterModels;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Contracts
{
    public interface IEmployeeService
    {
        List<EmployeeModel> GetAllEmployees(string companyName);
        Task<Employee> CreateEmployee(EmployeeModel model);
        Task<bool> UpdateEmployee(EmployeeModel model);
        bool DeleteEmployee(long id);
        EmployeeModel GetEmployee(long id);
        Task<Employee> GetEmployeeByUserId(string userId);
    }
}
