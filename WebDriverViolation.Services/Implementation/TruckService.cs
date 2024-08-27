using AutoMapper;
using Data.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Implementation
{
    public class TruckService : ITruckService
    {
        private readonly IRepository<Truck, string> _repository;
        private readonly ILogger<TruckService> _logger;
        private readonly IMapper _mapper;

        public TruckService(IRepository<Truck, string> repository,
          ILogger<TruckService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        public Task<bool> CreateTruck(TruckModel model)
        {

            try
            {
                model.CreatedDate = DateTime.Now;
                model.UpdatedDate = DateTime.Now;
                model.IsDelted = false;
                model.IsVisible = true;
                var truck = _mapper.Map<Truck>(model);
                var result = _repository.Add(truck);

                if (result != null)
                {
                    return Task<bool>.FromResult<bool>(true);
                }
                else
                {
                    return Task<bool>.FromResult<bool>(false);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return Task<bool>.FromResult<bool>(false);
        }

        public bool DeleteTruck(string id)
        {
            try
            {
                var truck = _repository.Find(t => t.Id == id).FirstOrDefault();
                if (truck != null)
                {
                    truck.IsDelted = true;
                    truck.IsVisible = false;
                    bool result = _repository.Update(truck);
                    return result;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return false;
        }

        public List<TruckModel> GetAllActiveTrucks(string companyName)
        {
            try
            {
                var trucks = _repository.Find(t => t.IsVisible == true && t.Company == companyName).ToList();
                var models = new List<TruckModel>();
                models = _mapper.Map<List<TruckModel>>(trucks);
                return models;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return null;
        }

        public async Task<long> GetAllActiveTrucksCount(string companyName)
        {
            try
            {
                long trucksCount = _repository.Find(t => t.Company == companyName && t.IsVisible == true).Count();
                return trucksCount;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return 0;
        }

        public List<TruckModel> GetAllTrucks(string companyName)
        {
            try
            {
                var trucks = _repository.Find(t=>t.Company == companyName && t.IsVisible == true).ToList();
                var models = new List<TruckModel>();
                models = _mapper.Map<List<TruckModel>>(trucks);
                return models;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return null;
        }

        public List<TruckModel> GetAllTrucks()
        {
            try
            {
                var trucks = _repository.Find(t => t.IsVisible == true).ToList();
                var models = new List<TruckModel>();
                models = _mapper.Map<List<TruckModel>>(trucks);
                return models;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return null;
        }


        public TruckModel GetTruck(string id)
        {
            try
            {
                Truck truck = _repository.Find(t => t.IsVisible == true && t.Id == id).First();
                TruckModel truckModel = _mapper.Map<TruckModel>(truck);
                return truckModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
        }

        public Task<bool> UpdateTruck(TruckModel model)
        {
            var truck = _mapper.Map<Truck>(model);

            try
            {
               bool result = _repository.Update(truck);

                return Task<bool>.FromResult<bool>(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return Task<bool>.FromResult<bool>(false);
        }
    }
}
