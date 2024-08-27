using AutoMapper;
using Data.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using Take5.Models.Models;
using Take5.Services.Contracts;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Models.Models.MasterModels;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models;
using WebDriverViolation.Services.Models.APIModels;
using WebDriverViolation.Services.Models.MasterModels;    

namespace WebDriverViolation.Services.Implementation.Violations
{
    public class ViolationService : IViolationService
    {
        private readonly IRepository<Violation, long> _repository;
        private readonly ILogger<ViolationService> _logger;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IViolationTypeService _violationTypeService;
        private readonly ITruckService _truckService;
        private readonly IViolationTypeAccuracyLavelService _violationTypeAccuracyLavelService;
        private readonly IConfirmationStatusService _confirmationStatusService;

        public ViolationService(IRepository<Violation, long> repository,
          ILogger<ViolationService> logger, IMapper mapper,
          IWebHostEnvironment hostEnvironment, IViolationTypeService violationTypeService, 
          ITruckService truckService,
          IViolationTypeAccuracyLavelService violationTypeAccuracyLavelService,
          IConfirmationStatusService confirmationStatusService)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
            _violationTypeService = violationTypeService;
            _truckService = truckService;
            _confirmationStatusService = confirmationStatusService;
            _violationTypeAccuracyLavelService = violationTypeAccuracyLavelService;
        }
        //public async Task<ViolationModel> CreateViolation(ViolationAPIModel model, int TypeID, double AverageProbability, string TruckID, double totalTime)
        //{
        //    try
        //    {
        //        if (model != null)
        //        {
        //            Violation violation  =  ConvertFromViolationAPIToViolation(model, TypeID, AverageProbability, TruckID, totalTime);
        //            ViolationModel violationModel = new ViolationModel();
        //            if(violation != null)
        //            {
        //              var addedViolation =  _repository.Add(violation);
        //                if(addedViolation != null)
        //                {
        //                    violationModel = _mapper.Map<ViolationModel>(addedViolation);
        //                    return violationModel;
        //                }
        //                else
        //                {
        //                    return null;
        //                }
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public async Task<List<ViolationModel>> CreateViolation(List<Violation> models)
        {
            try
            {
                if (models != null)
                {

                    var addedViolations = _repository.AddRange(models);
                    if( addedViolations != null )
                    {
                      List<ViolationModel> violationModels =  _mapper.Map<List<ViolationModel>>(addedViolations);
                        return violationModels;
                    }
                    else
                    {
                        return null;
                    }
   
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> SaveImage(string strm, string uploadFolder)
        {

            //this is a simple white background image
            var myfilename = string.Format(@"{0}", Guid.NewGuid());
            //Generate unique filename
            string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, uploadFolder);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + "violationImg.jpg";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            var bytess = Convert.FromBase64String(strm);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                fileStream.Write(bytess, 0, bytess.Length);
                fileStream.Flush();
            }

            return uniqueFileName;
        }


        public async Task<List<ViolationModel>> GetAllViolations()
        {
            try
            {
                var violations = _repository.Find(v => v.IsVisible == true, false, v => v.ViolationType);
                if (violations.Any() == true)
                {
                    List<ViolationModel> violationModels = _mapper.Map<List<ViolationModel>>(violations);
                    return violationModels;
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

        public async Task<List<ViolationModel>> SearchForViolation(SearchViolationModel searchViolationModel)
        {
            try
            {
                if (searchViolationModel != null)
                {
                    try
                    {
                        var violations =  _repository.Find(v => v.IsVisible == true, false, t=>t.ViolationType, t=>t.ConfirmationStatus, x=>x.Truck);
                        if (violations != null)
                        {
                            if (searchViolationModel.From.HasValue)
                            {
                                violations = violations.Where(v => v.Date >= searchViolationModel.From.Value);
                            }
                            if (searchViolationModel.To.HasValue)
                            {
                                violations = violations.Where(v => v.Date <= searchViolationModel.To.Value);
                            }
                            if (searchViolationModel.SelectedTruckID != "-1")
                            {
                                violations = violations.Where(v=>v.TruckID == searchViolationModel.SelectedTruckID);
                            }
                            if (searchViolationModel.TruckStatusId == 1)
                            {
                                violations = violations.Where(v => v.IsTruckMoving == false);
                            }
                            if (searchViolationModel.TruckStatusId == 0)
                            {
                                violations = violations.Where(v => v.IsTruckMoving == true);
                            }
                            if (searchViolationModel.SelectedConfirmationStatusId != -1)
                            {
                                violations = violations.Where(v => v.ConfirmationStatusId ==            searchViolationModel.SelectedConfirmationStatusId);
                            }
                            if (searchViolationModel.SelectedTypes != null)
                            {
                                violations = violations.Where(v => searchViolationModel.SelectedTypes.Contains(v.ViolationTypeID));
                            }
                            List<ViolationModel> violationModels = new List<ViolationModel>();
                            List<ViolationModel> uniqueViolationModels = new List<ViolationModel>();
                            if (violations != null)
                            {
                                violationModels = _mapper.Map<List<ViolationModel>>(violations.ToList());
                                var violationGroups = violationModels.GroupBy(x => x.Code);
                                foreach (var violationGroup in violationGroups)
                                {
                                    ViolationModel highestViolation = violationGroup.Where(g => g.Probability == violationGroup.Max(g => g.Probability)).FirstOrDefault();
                                    if (highestViolation != null)
                                    {
                                        var imgs = violationGroup.Select(g => g.imageName).ToList();
                                        if (imgs.Count > 0)
                                        {
                                            highestViolation.images = new List<string>();
                                            highestViolation.images.AddRange(imgs);
                                        }
                                        uniqueViolationModels.Add(highestViolation);

                                    }
                                }
                                uniqueViolationModels = uniqueViolationModels.OrderByDescending(x => x.Date).ToList();
                            }

                            // var violationModelGroups = violationModels.GroupBy(v => v.Code).ToList();
                            return uniqueViolationModels;
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

        public async Task<List<ViolationModel>> GetViolationByDate(DateTime date)
        {
            try
            {
                var violations = _repository.Find(v=>v.Date == date&& v.IsVisible == true, false, v => v.ViolationType);
                if (violations.Any() == true)
                {
                    List<ViolationModel> violationModels = _mapper.Map<List<ViolationModel>>(violations);
                    return violationModels;
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


        public async Task<string> GetLastViolationCodeForToday()
        {
            try
            {
                var LastTodayViolation = _repository.Find(v => v.CreatedDate.Date == DateTime.Now.Date && v.IsVisible == true).OrderByDescending(v=>v.Id).FirstOrDefault();
                if(LastTodayViolation == null)
                {
                    return string.Empty;
                }
                else
                {
                    return LastTodayViolation.Code;
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }
        public async Task<List<ViolationModel>> GetViolationByType(int type)
        {
            try
            {
                var violations = _repository.Find(v => v.ViolationTypeID == type && v.IsVisible == true, false, v=>v.ViolationType);
                if (violations.Any() == true)
                {
                    List<ViolationModel> violationModels = _mapper.Map<List<ViolationModel>>(violations);
                    return violationModels;
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

        public Violation ConvertFromViolationAPIToViolation(ViolationAPIModel model, int TypeID, double AvgProbability, string TruckID, double TotalTime, bool IsTruckMoving, bool ModeLight)
        {
            try
            {
       
                Violation violation = new Violation
                {
                    TruckID = TruckID,
                    ViolationTypeID = TypeID,
                    imageName = model.image,
                    Date = model.Date,
                    TotalTime = TotalTime,
                    ConfirmationStatusId = (int)CommanData.ConfirmationStatus.Pending,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    IsVisible = true,
                    IsDelted = false,
                    Probability = Math.Round(model.Probability, 2, MidpointRounding.AwayFromZero),
                    AverageProbability= Math.Round(AvgProbability, 2, MidpointRounding.AwayFromZero),
                    Code = model.Code,
                    AllClassessProbability= model.AllClassessProbability,
                    IsTruckMoving = IsTruckMoving,
                };
                int mode = 0;

                if (ModeLight == true)
                {
                    mode = 1;
                }
                else
                {
                    mode = 2;
                }
                var violationTypeAccuracyLavel = _violationTypeAccuracyLavelService.GetViolationAccuracyLavelForViolation(TypeID, AvgProbability, model.Date, mode);
                if (violationTypeAccuracyLavel != null)
                {
                    violation.ViolationTypeAccuracyLavelId = violationTypeAccuracyLavel.Id;
                }
                return violation;
            }
            catch(Exception ex)
            {
                return null;
            }
        }


        public async Task<SearchViolationModel> InitiateViolationSearchModel(SearchViolationModel searchViolationModel)
        {
            try
            {
                searchViolationModel.Trucks = _truckService.GetAllTrucks();
                if (searchViolationModel.SelectedTruckID == string.Empty)
                {
                    searchViolationModel.SelectedTruckID = "-1";
                }
                var violationTypes = _violationTypeService.GetAllViolationTypes().Result;
                searchViolationModel.ViolationTypeModels = violationTypes;
                if (searchViolationModel.ViolationTypeID == 0)
                {
                    searchViolationModel.ViolationTypeID = -1;
                }
                var confirmationStatuses = _confirmationStatusService.GetAllConfirmationStatuses();
                if(confirmationStatuses != null)
                {
                    searchViolationModel.ConfirmationStatusModel = confirmationStatuses;
                    if (searchViolationModel.SelectedConfirmationStatusId == 0)
                    {
                        searchViolationModel.SelectedConfirmationStatusId = -1;
                    }
                }
                else
                {
                    searchViolationModel.ConfirmationStatusModel = new List<ConfirmationStatusModel>();
                }
               
                searchViolationModel.From = DateTime.Now;
                searchViolationModel.To = DateTime.Now;
                return searchViolationModel;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public ViolationModel GetViolation(long violationId)
        {
            try
            {
                var violation = _repository.Find(v => v.Id == violationId && v.IsVisible == true, false, vi => vi.ViolationType, vi=>vi.ConfirmationStatus).FirstOrDefault();
                if (violation != null)
                {
                    ViolationModel violationModel = _mapper.Map<ViolationModel>(violation);
                    return violationModel;
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

        public async Task<bool> AddViolationConfirmationDetails(int violationConfirmedTypeId, long violationId, string userId)
        {
            try
            {
                bool updateResult = false;
                var violation = _repository.Find(v => v.Id == violationId && v.IsVisible == true).FirstOrDefault();
                if (violation != null)
                {
                    List<Violation> SameInstanceViolations = await _repository.Find(v => v.Code==  violation.Code && v.IsVisible == true).ToListAsync();
                    if(SameInstanceViolations.Count > 0)
                    {
                        foreach(var violationInstance in SameInstanceViolations)
                        {
                            violationInstance.ConfirmationViolationTypeID = violationConfirmedTypeId;
                            violationInstance.ConfirmationDate = DateTime.Now;
                            violationInstance.ConfirmationStatusId = (int)CommanData.ConfirmationStatus.Confirmed;
                            violationInstance.ConfirmedByUserId = userId;
                            if (violationInstance.ViolationTypeID == violationConfirmedTypeId)
                            {
                                violationInstance.IsTrue = true;
                            }
                            else
                            {
                                violationInstance.IsTrue = false;
                            }
                            updateResult = _repository.Update(violationInstance);
                        }
                    }
                }
               return updateResult;
            }
            catch (Exception e)
            {
                return false;
            }

        }


        public async Task<Dictionary<string, long>> GetActualViolationCountsperType()
        {
            try
            {
                Dictionary<string, long> ViolationTypeActualcounts = new Dictionary<string, long>();
                string violationTypeName = "";

                var violationsCounts = _repository.Find(v => v.IsVisible == true && v.ConfirmationStatusId == (int)CommanData.ConfirmationStatus.Confirmed && v.ConfirmationViolationTypeID != (int)CommanData.ViolationTypes.NoViolation).GroupBy(v => v.ConfirmationViolationTypeID)
                      .Select(g => new { g.Key, Count = g.Count() }).ToList();
                if (violationsCounts.Count() > 0)
                {
                   foreach (var group in violationsCounts)
                    {
                        violationTypeName = Enum.GetName(typeof(CommanData.ViolationTypes), group.Key);
                        long x = group.Count / 3;
                        ViolationTypeActualcounts.Add(violationTypeName, x);
                    }
                }
                return ViolationTypeActualcounts;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public async Task<long> GetPendingViolationCount()
        {
            try 
            { 
               long pendingViolationCount = _repository.Find(v=>v.IsVisible == true && v.ConfirmationStatusId == (int)CommanData.ConfirmationStatus.Pending).Count();

                return pendingViolationCount;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public async Task<Dictionary<string, long>> GetDashBoardViolationStatusCount()
        {
            try
            {
                Dictionary<string, long> ViolationTypeActualcounts = new Dictionary<string, long>();
                string violationTypeName = "";

                var violationsCounts = _repository.Find(v => v.IsVisible == true && v.ConfirmationStatusId == (int)CommanData.ConfirmationStatus.Confirmed && v.ConfirmationViolationTypeID != (int)CommanData.ViolationTypes.NoViolation).GroupBy(v => v.ViolationTypeID)
                      .Select(g => new { g.Key, Count = g.Count() }).ToList();
                if (violationsCounts.Count() > 0)
                {
                    foreach (var group in violationsCounts)
                    {
                        violationTypeName = Enum.GetName(typeof(CommanData.ViolationTypes), group.Key);
                        ViolationTypeActualcounts.Add(violationTypeName, group.Count);
                    }
                }
                return ViolationTypeActualcounts;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public async Task<long> GetAllActualViolationCount()
        {
            try
            {
                long totalActualViolationCount = _repository.Find(v => v.IsVisible == true && v.ConfirmationStatusId == (int)CommanData.ConfirmationStatus.Confirmed && v.ConfirmationViolationTypeID != (int)CommanData.ViolationTypes.NoViolation).Count();
                return totalActualViolationCount;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public async Task<List<ViolationModel>> GetAllPendingViolations()
        {
            try
            {
                List<ViolationModel> violationModels = new List<ViolationModel>();
                List<ViolationModel> uniqueViolationModels = new List<ViolationModel>();
                List<Violation> pendingViolations = await _repository.Find(v => v.IsVisible == true &&v.IsTruckMoving == true&& v.ConfirmationStatusId == (int)CommanData.ConfirmationStatus.Pending, false,
                    x => x.ConfirmationStatus, x => x.ViolationType, x=>x.Truck).OrderByDescending(x => x.CreatedDate).Take(100).ToListAsync();
                if (pendingViolations != null)
                {
                    violationModels = _mapper.Map<List<ViolationModel>>(pendingViolations);
                    var violationGroups = violationModels.GroupBy(x => x.Code);
                    foreach (var violationGroup in violationGroups)
                    {
                        ViolationModel highestViolation = violationGroup.Where(g => g.Probability == violationGroup.Max(g => g.Probability)).FirstOrDefault();
                        if (highestViolation != null)
                        {
                            var imgs = violationGroup.Select(g => g.imageName).ToList();
                            if (imgs.Count > 0)
                            {
                                highestViolation.images = new List<string>();
                                highestViolation.images.AddRange(imgs);
                            }
                            uniqueViolationModels.Add(highestViolation);

                        }
                    }
                    uniqueViolationModels = uniqueViolationModels.OrderByDescending(x => x.Date).ToList();
                }
                return uniqueViolationModels;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<long> GetCurrentMonthActualViolationCount()
        {
            try
            {
                long totalActualViolationCount = _repository.Find(v => v.IsVisible == true && v.ConfirmationStatusId == (int)CommanData.ConfirmationStatus.Confirmed && v.ConfirmationViolationTypeID != (int)CommanData.ViolationTypes.NoViolation && v.Date.Month == DateTime.Today.Month).Count();
                return totalActualViolationCount;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public List<Violation> ConvertFromViolationAPIToViolation(List<ViolationAPIModel> models, string code)
        {
            try
            {
                List<Violation> violations = new List<Violation>();
                foreach(var model in models)
                {
                    Violation violation = new Violation
                    {
                        //TruckID = model.TruckID,
                        //ViolationTypeID = model.TypeID,
                        imageName = model.image,
                        Date = model.Date,
                        ConfirmationStatusId = (int)CommanData.ConfirmationStatus.Pending,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        IsVisible = true,
                        IsDelted = false,
                        Probability = model.Probability,
                       
                        Code = code,
                        AllClassessProbability = model.AllClassessProbability,
                    };
                    violations.Add(violation);
                }
            
                return violations;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ViolationModel>> GetViolationByCode(string code)
        {
            try
            {
                List<string> violationImages = new List<string>();
                List<ViolationModel> violationModels = new List<ViolationModel>();
               var violations =  await _repository.Find(v=>v.IsVisible == true && v.Code == code, false, v=>v.ViolationType).ToListAsync();
                if(violations != null)
                {
                    violationModels = _mapper.Map<List<ViolationModel>>(violations);
                }
                return violationModels;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<ViolationModel> GetLastViolationForSpecificTypeAndTruck(int typeID, string truckID)
        {
            try
            {
                ViolationModel violationModel = null;
              var lastViolation =  _repository.Find(v=>v.IsVisible == true && v.TruckID == truckID && v.ViolationTypeID == typeID).OrderByDescending(v=>v.Date).FirstOrDefault();
                if(lastViolation != null)
                {
                    violationModel = _mapper.Map<ViolationModel>(lastViolation);

                }
                return violationModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<ViolationModel> GetLastViolationForSpecificTypeAndTruckWithMail(int typeID, string truckID)
        {
            try
            {
                ViolationModel violationModel = null;
                var lastViolation = _repository.Find(v => v.IsVisible == true && v.TruckID == truckID && v.ViolationTypeID == typeID && v.MailSent ==1).OrderByDescending(v => v.Date).FirstOrDefault();
                if (lastViolation != null)
                {
                    violationModel = _mapper.Map<ViolationModel>(lastViolation);

                }
                return violationModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
