using AutoMapper;
using Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Models.Models.MasterModels;
using WebDriverViolation.Services.Contracts;

namespace WebDriverViolation.Services.Implementation
{
    public class ViolationTypeAccuracyLavelService : IViolationTypeAccuracyLavelService
    {
        private readonly IRepository<ViolationTypeAccuracyLavel, long> _repository;
        private readonly IMapper _mapper;

        public ViolationTypeAccuracyLavelService(IRepository<ViolationTypeAccuracyLavel, long> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<List<ViolationTypeAccuracyLavel>> GetAllViolationTypeAccuracyLavels()
        {
            try
            {
                var ViolationAccuracyLavels = _repository.Find(v => v.IsVisible == true , false, v => v.ViolationType);
                return ViolationAccuracyLavels.ToList();
            }
            catch (Exception exp)
            {
                return null;
            }
        }

        public async  Task<List<ViolationTypeAccuracyLavel>> GetViolationAccuracyLavel(long levelId)
        {
            try
            {
               var ViolationAccuracyLavels =  _repository.Find(v => v.IsVisible == true && v.LevelId == levelId, false, v => v.ViolationType);
                return ViolationAccuracyLavels.ToList();
            }
            catch(Exception exp)
            {
                return null;
            }
        }

        public async Task<ViolationTypeAccuracyLavel> GetViolationAccuracyLavelById(long Id)
        {
            try
            {
                var ViolationAccuracyLavel = _repository.Find(v => v.IsVisible == true && v.Id == Id).FirstOrDefault();
                return ViolationAccuracyLavel;
            }
            catch (Exception exp)
            {
                return null;
            }
        }

        public async Task<List<ViolationTypeAccuracyLavel>> GetViolationAccuracyLavelsByType(int type)
        {
            try
            {
                var ViolationAccuracyLavels = _repository.Find(v => v.IsVisible == true && v.ViolationTypeId == type, false, v => v.ViolationType);
                return ViolationAccuracyLavels.ToList();
            }
            catch (Exception exp)
            {
                return null;
            }
        }


        public async Task<List<ViolationTypeAccuracyLavel>> GetViolationAccuracyLavelsByType(int type, DateTime ViolationDate, int mode)
        {
            try
            {
                var ViolationAccuracyLavels = _repository.Find(v => v.IsVisible == true && v.ViolationTypeId == type&&v.Mode == mode && v.CreatedDate.CompareTo(ViolationDate) < 0 && ViolationDate.CompareTo(v.EndDate) < 0, false, v => v.ViolationType);
                return ViolationAccuracyLavels.ToList();
            }
            catch (Exception exp)
            {
                return null;
            }
        }

        public ViolationTypeAccuracyLavel GetViolationAccuracyLavelForViolation(int type, double AverageProbability, DateTime ViolationDate, int mode   )
        {
            try
            {
               List<ViolationTypeAccuracyLavel> violationTypeAccuracyLavels = GetViolationAccuracyLavelsByType(type,ViolationDate, mode).Result;
                foreach(var level in violationTypeAccuracyLavels)
                {
                    if(AverageProbability >= level.LowestPercent && AverageProbability <= level.HighestPercent)
                    {
                        return level;
                    }
                }
                return null;
            }
            catch (Exception exp)
            {
                return null;
            }
        }
    }
}
