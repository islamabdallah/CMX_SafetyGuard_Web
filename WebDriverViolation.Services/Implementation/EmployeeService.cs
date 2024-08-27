using AutoMapper;
using Data.Repository;
using WebDriverViolation.Models.Models.MasterModels;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models.MasterModels;

namespace Take5.Services.Implementation
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee, long> _repository;
        private readonly IMapper _mapper;

        public EmployeeService(IRepository<Employee, long> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Employee> CreateEmployee(EmployeeModel model)
        {
            try
            {

                model.IsVisible = true;
                Employee employee = _mapper.Map<Employee>(model);
                Employee addedEmployee = _repository.Add(employee);
                return addedEmployee;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public bool DeleteEmployee(long id)
        {
            throw new NotImplementedException();
        }

        public List<EmployeeModel> GetAllEmployees(string companyName)
        {
            try
            {
                List<Employee> employees = _repository.Find(e => e.IsVisible == true && e.Company == companyName).ToList();
                List<EmployeeModel> models = _mapper.Map<List<EmployeeModel>>(employees);
                return models;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public EmployeeModel GetEmployee(long id)
        {
            try
            {
                var employee = _repository.Find(e => e.IsVisible == true&&e.EmployeeNumber == id).FirstOrDefault();
                if(employee!= null)
                {
                    EmployeeModel model = _mapper.Map<EmployeeModel>(employee);
                    return model;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Employee> GetEmployeeByUserId(string userId)
        {
            try
            {
                var employee = _repository.Find(e => e.IsVisible == true && e.UserId == userId).FirstOrDefault();
                return employee;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task<bool> UpdateEmployee(EmployeeModel model)
        {
            throw new NotImplementedException();
        }
    }
}
