using AutoMapper;
using Data.Repository;
using Microsoft.EntityFrameworkCore;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Implementation.Violations
{
    public class ViolationTypeService : IViolationTypeService
    {
        private readonly IRepository<ViolationType, int> _repository;
        private readonly IMapper _mapper;

        public ViolationTypeService(IRepository<ViolationType, int> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ViolationTypeModel>> GetAllViolationTypes()
        {
            try
            {
                var violationTypes = await _repository.Find(v => v.IsVisible == true).ToListAsync();
                List<ViolationTypeModel> violationTypeModels = _mapper.Map<List<ViolationTypeModel>>(violationTypes);
                return violationTypeModels;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public ViolationTypeModel GetViolationType(int id)
        {
            try
            {
                var violationType =  _repository.Find(v => v.IsVisible == true && v.Id == id).FirstOrDefault();
                if(violationType != null)
                {
                   ViolationTypeModel violationTypeModel = _mapper.Map<ViolationTypeModel>(violationType);
                    return violationTypeModel;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
