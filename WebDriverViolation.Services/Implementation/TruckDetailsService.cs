using AutoMapper;
using Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models.APIModels;

namespace WebDriverViolation.Services.Implementation
{
    public class TruckDetailsService : ITruckDetailsService
    {
        private readonly IRepository<TruckDetail, long> _repository;
        private readonly ITruckService _truckService;
        private readonly IViolationTypeService _violationTypeService;
        private readonly IMapper _mapper;

        public TruckDetailsService(IRepository<TruckDetail, long> repository,
            ITruckService truckService,
            IMapper mapper,
            IViolationTypeService violationTypeService)
        {
            _repository = repository;
            _truckService = truckService;
            _mapper = mapper;
            _violationTypeService = violationTypeService;
        }
        public Task<bool> CreateTruckDetails(TruckDetailsApiModel truckDetails)
        {
            try
            {
                if (truckDetails != null)
                {
                    TruckDetail truck = new TruckDetail
                    {
                        TruckId = truckDetails.TruckId,
                        Speed = truckDetails.Speed,
                        Date = truckDetails.Date,
                        Rbm = truckDetails.Rbm,
                        StartTime = truckDetails.StartTime,
                        EndTime = truckDetails.EndTime,
                        LastSpeed = truckDetails.LastSpeed,
                        Duration = truckDetails.Duration,
                        Fuel_Level = truckDetails.Fuel_Level,
                        IsDelted = false,
                        IsVisible = true,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    };
                    var item = _repository.Add(truck);
                    if (item != null)
                    {
                        return Task<bool>.FromResult(true);
                    }
                    else
                    {
                        return Task<bool>.FromResult(false);
                    }
                }
                else
                {
                    return Task<bool>.FromResult(false);

                }

            }
            catch (Exception e)
            {
                return Task<bool>.FromResult(false);
            }
        }
    }
}
