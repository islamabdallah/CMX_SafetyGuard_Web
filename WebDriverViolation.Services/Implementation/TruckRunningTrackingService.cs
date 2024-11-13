using AutoMapper;
using Data.Repository;
using Take5.Services.Contracts;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models.APIModels;
using WebDriverViolation.Services.Models.MasterModels;

namespace Take5.Services.Implementation.Violations
{
    public class TruckRunningTrackingService : ITruckRunningTrackingService
    {
        private readonly IRepository<TruckRunningTracking, long> _repository;
        private readonly ITruckService _truckService;
        private readonly IViolationTypeService _violationTypeService;
        private readonly IMapper _mapper;

        public TruckRunningTrackingService(IRepository<TruckRunningTracking, long> repository,
            ITruckService truckService,
            IMapper mapper,
            IViolationTypeService violationTypeService)
        {
            _repository = repository;
            _truckService = truckService;
            _mapper = mapper;
            _violationTypeService = violationTypeService;
        }
        public Task<bool> CreateTruckRunningTracking(TruckRunningTrackingAPIModel truckRunningTrackingAPIModel)
        {
            try
            {
                if(truckRunningTrackingAPIModel != null)
                {
                    TruckRunningTracking truckRunningTracking = new TruckRunningTracking
                    {
                        //TruckStatusID = truckRunningTrackingAPIModel.TruckStatusID,
                        TruckID = truckRunningTrackingAPIModel.TruckID,
                        StartDate = truckRunningTrackingAPIModel.StartDate,
                        LastStoppedDate = truckRunningTrackingAPIModel.LastStoppedDate,
                        IsDelted = false,
                        IsVisible = true,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    };
                    var item = _repository.Add(truckRunningTracking);
                   if(item != null)
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
                    return Task<bool>.FromResult(false) ;

                }

            }
            catch(Exception e)
            {
                return Task<bool>.FromResult(false);
            }
        }

        public CameraTrackingModel InitiateCameraSearchModel(CameraTrackingModel searchViolationModel)
        {
            try
            {
                searchViolationModel.Trucks = this._truckService.GetAllTrucks().Where<TruckModel>((Func<TruckModel, bool>)(t => t.Category == "camera")).ToList<TruckModel>();
                if (searchViolationModel.SelectedTruckID == string.Empty)
                    searchViolationModel.SelectedTruckID = "-1";
                List<ViolationTypeModel> list =_violationTypeService.GetAllViolationTypes().Result.Where<ViolationTypeModel>((Func<ViolationTypeModel, bool>)(t => t.Category == "camera" || t.Category == "all")).ToList<ViolationTypeModel>();
                searchViolationModel.ViolationTypeModels = list;
                if (searchViolationModel.ViolationTypeID == 0)
                    searchViolationModel.ViolationTypeID = -1;
                return searchViolationModel;
            }
            catch (Exception ex)
            {
                return (CameraTrackingModel)null;
            }
        }

        public SearchTruckTrackingModel InitiateTruckTrackingSearchModel(SearchTruckTrackingModel searchTruckTrackingModel)
        {
            try
            {
                searchTruckTrackingModel.Trucks = _truckService.GetAllTrucks();
                if (searchTruckTrackingModel.SelectedTruckID == string.Empty)
                {
                    searchTruckTrackingModel.SelectedTruckID = "-1";
                }
                searchTruckTrackingModel.FromDate = DateTime.Now;
                searchTruckTrackingModel.ToDate = DateTime.Now;
                return searchTruckTrackingModel;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public List<TruckRunningTrackingAPIModel> SearchTruckRunningTracking(SearchTruckTrackingModel searchTruckTrackingModel)
        {
            try
            {
                if (searchTruckTrackingModel != null)
                {
                    try
                    {
                        var truckRunningTrackings = _repository.Find(v => v.IsVisible == true && v.TruckID == searchTruckTrackingModel.SelectedTruckID && v.StartDate.Date <= searchTruckTrackingModel.FromDate.Date && v.LastStoppedDate.Date >= searchTruckTrackingModel.ToDate.Date).ToList();
                        if (truckRunningTrackings.Count > 0)
                        {
                                
                            List<TruckRunningTrackingAPIModel> TruckRunningTrackingAPIModels = _mapper.Map<List<TruckRunningTrackingAPIModel>>(truckRunningTrackings);
                            return TruckRunningTrackingAPIModels;
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

    }
}
