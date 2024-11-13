using AutoMapper;
using Data.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json;
using Take5.Models.Models;
using Take5.Services.Contracts;
using Take5.Services.Implementation;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Models.Models.MasterModels;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Implementation.Violations;
using WebDriverViolation.Services.Models;
using WebDriverViolation.Services.Models.MasterModels;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace WebDriverViolation.Controllers.Violations
{
    [Authorize]
    public class ViolationController : BaseController
    {
        private readonly IViolationTypeService _violationTypeService;
        private readonly IViolationService _violationService;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IEmployeeService _employeeService;
        private readonly IRepository<UserViolationNotification, long> _repository;
        private readonly IViolationTypeAccuracyLavelService _violationTypeAccuracyLavelService;
        private readonly IRepository<Violation, long> _vRepository;
        private readonly IMapper _mapper;
        public ViolationController(IViolationTypeService violationTypeService,
            IViolationService violationService, IRepository<Violation, long> vRepository,IMapper mapper,
            UserManager<AspNetUser> userManager,
            IEmployeeService employeeService,
             IRepository<UserViolationNotification, long> repository,
            IViolationTypeAccuracyLavelService violationTypeAccuracyLavelService)
        {
            _violationTypeService= violationTypeService;
            _violationService = violationService;
            _userManager= userManager;
            _employeeService= employeeService;
            _repository= repository;
            _violationTypeAccuracyLavelService = violationTypeAccuracyLavelService;
            _mapper= mapper;
            _vRepository= vRepository;
        }
        public ActionResult SearchViolation()        
        {
            try
            {
                SearchViolationModel searchViolationModel = new SearchViolationModel();
                searchViolationModel = _violationService.InitiateViolationSearchModel(searchViolationModel).Result;
                searchViolationModel.Trucks = searchViolationModel.Trucks.Where<TruckModel>((Func<TruckModel, bool>)(t => t.Category != "camera")).ToList<TruckModel>();
                searchViolationModel.ViolationTypeModels = searchViolationModel.ViolationTypeModels.Where<ViolationTypeModel>((Func<ViolationTypeModel, bool>)(t => t.Category != "camera")).ToList<ViolationTypeModel>();

                return View(searchViolationModel);
            }
            catch(Exception ex)
            {
                return RedirectToAction("ERROR404");
            }
        }
        public ActionResult SearchPPEViolation()
        {
            try
            {
                SearchViolationModel result = _violationService.InitiateViolationSearchModel(new SearchViolationModel()).Result;
                result.Trucks = result.Trucks.Where<TruckModel>((Func<TruckModel, bool>)(t => t.Category == "camera")).ToList<TruckModel>();
                result.ViolationTypeModels = result.ViolationTypeModels.Where<ViolationTypeModel>((Func<ViolationTypeModel, bool>)(t => t.Category == "camera" || t.Category == "all")).ToList<ViolationTypeModel>();
                return View(result);
            }
            catch (Exception ex)
            {
                return (ActionResult)this.RedirectToAction("ERROR404");
            }
        }


        [HttpPost]
        public ActionResult SearchViolation(SearchViolationModel searchViolationModel)
        {
            try
            {
                var loginUser = _userManager.GetUserAsync(User).Result;
                if(loginUser != null)
                {
                    if(loginUser.Company == "Security")
                    {
                        List<ViolationModel> violatioModels = _violationService.SearchForViolation(searchViolationModel).Result;
                        if (violatioModels != null)
                        {
                            searchViolationModel.ViolationModels = violatioModels.Where<ViolationModel>((t => t.Category != "camera")).ToList<ViolationModel>(); ;
                        }
                        else
                        {
                            searchViolationModel.ViolationModels = new List<ViolationModel>();
                        }
                        searchViolationModel = _violationService.InitiateViolationSearchModel(searchViolationModel).Result;
                        searchViolationModel.Trucks = searchViolationModel.Trucks.Where<TruckModel>((Func<TruckModel, bool>)(t => t.Category != "camera")).ToList<TruckModel>();
                        searchViolationModel.ViolationTypeModels = searchViolationModel.ViolationTypeModels.Where<ViolationTypeModel>((Func<ViolationTypeModel, bool>)(t => t.Category != "camera")).ToList<ViolationTypeModel>();
                        return View(searchViolationModel);
                    }
                    else
                    {
                        return RedirectToAction("ERROR404");
                    }
                }
                else
                {
                    return RedirectToAction("ERROR404");
                }
   
            }
            catch (Exception ex)
            {
                return RedirectToAction("ERROR404");
            }
        }
        [HttpPost]
        public ActionResult SearchPPEViolation(SearchViolationModel searchViolationModel)
        {
            try
            {
                AspNetUser result1 = _userManager.GetUserAsync(User).Result;
                if (result1 == null)
                    return RedirectToAction("ERROR404");
                if (!(result1.Company == "Security"))
                    return RedirectToAction("ERROR404");
                List<ViolationModel> result2 = _violationService.SearchForViolation(searchViolationModel).Result;
                searchViolationModel.ViolationModels = result2 == null ? new List<ViolationModel>() : result2.Where<ViolationModel>((t => t.Category == "camera")).ToList<ViolationModel>();
                searchViolationModel = _violationService.InitiateViolationSearchModel(searchViolationModel).Result;
                searchViolationModel.Trucks = searchViolationModel.Trucks.Where<TruckModel>((Func<TruckModel, bool>)(t => t.Category == "camera")).ToList<TruckModel>();
                searchViolationModel.ViolationTypeModels = searchViolationModel.ViolationTypeModels.Where<ViolationTypeModel>((Func<ViolationTypeModel, bool>)(t => t.Category == "camera" || t.Category == "all")).ToList<ViolationTypeModel>();
                return View("SearchPPEViolation",searchViolationModel);
            }
            catch (Exception ex)
            {
                return (ActionResult)this.RedirectToAction("ERROR404");
            }
        }


        public async Task<string> getViolationDetails(long violationId)
        {
            try
            {
               ViolationModel violationModel = _violationService.GetViolation(violationId);
                List<string> images = new List<string>();
               ViolationTypeAccuracyLavel  violationTypeAccuracyLavel = await _violationTypeAccuracyLavelService.GetViolationAccuracyLavelById(violationModel.ViolationTypeAccuracyLavelId);
                if(violationTypeAccuracyLavel != null)
                {
                    violationModel.ViolationAccuracyLevelDescription = violationTypeAccuracyLavel.Description;
                }
                else
                {
                    violationModel.ViolationAccuracyLevelDescription = "High";
                }
                if (violationModel != null)
                {
                   List<ViolationModel> violationsWithSameCode = await _violationService.GetViolationByCode(violationModel.Code);
                    if(violationsWithSameCode != null)
                    {
                        violationModel.images = new List<string>();
                        violationModel.PriobabilityOfViolationsWithSameCode = new List<double>();
                        foreach (var violation in violationsWithSameCode)
                        {
                           violationModel.images.Add("http://20.86.97.165/DriverViolation/images/ViolationImages/" + violation.imageName);
                            violationModel.PriobabilityOfViolationsWithSameCode.Add(violation.Probability);
                        }
                    }
                    else
                    {
                        violationModel.images.Add("http://20.86.97.165/DriverViolation/images/ViolationImages/test.jpg");
                        violationModel.PriobabilityOfViolationsWithSameCode.Add(0);
                    }
                    //if (violationModel.imageName != null)
                    //{
                    //    violationModel.imageName = "http://20.86.97.165/DriverViolation/images/ViolationImages/" + violationModel.imageName;
                    //}
                    //else
                    //{
                    //    violationModel.imageName = "http://20.86.97.165/DriverViolation/images/ViolationImages/test.jpg";
                    //}
                    if(violationModel.ConfirmationStatus.Id == (int)CommanData.ConfirmationStatus.Confirmed)
                    {
                        violationModel.ConfirmedByUserName = _employeeService.GetEmployeeByUserId(violationModel.ConfirmedByUserId).Result.EmployeeName.ToString();
                        violationModel.ConfirmationDateText = violationModel.ConfirmationDate.HasValue ? violationModel.ConfirmationDate.Value.ToString("yyyy-MM:dd, hh:mm:ss") : "not available";
                        if(violationModel.ConfirmationViolationTypeID != (int)CommanData.ViolationTypes.NoViolation)
                        {
                            violationModel.ConfirmationViolationTypeName = Enum.GetName(typeof(CommanData.ViolationTypes), violationModel.ConfirmationViolationTypeID);
                        }
                        else
                        {
                            violationModel.ConfirmationViolationTypeName = Enum.GetName(typeof(CommanData.RejectStatus), violationModel.ConfirmationViolationTypeID);
                        }

                    }
                    return JsonSerializer.Serialize(violationModel);
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
        public async Task<ActionResult> testAsync()
        {
            List<ViolationModel> violationModels = new List<ViolationModel>();
            var violations = await _vRepository.Find(v => v.IsVisible == true && v.Category == "camera" && v.ConfirmationViolationTypeID==10 && v.imageName.Contains("_2024-1"), false, v => v.ViolationType).ToListAsync();
            if (violations != null)
            {
                violationModels = _mapper.Map<List<ViolationModel>>(violations);
            }
            if(violationModels.Count > 0)
            {
                foreach(var violationModel in violationModels)
                {
                    string image = violationModel.imageName;
                    string uploadsFolder = Path.Combine(@"C:\inetpub\wwwroot\_driver\wwwroot/", "images/NoViolationImages/");
                    string filePath = Path.Combine(uploadsFolder, image);
                    string uploadsFolderr = Path.Combine(@"C:\inetpub\wwwroot\_driver\wwwroot/", "images/TempImage/");
                    string filePathh = Path.Combine(uploadsFolderr, image);
                    if (System.IO.File.Exists(filePath))
                    {
                        if (System.IO.File.Exists(filePathh) == false)
                        {
                            System.IO.File.Copy(filePath, filePathh);
                           
                        }
                        System.IO.File.Delete(filePath);

                    }
                      
                }
            }
           

            return View();
        }
        public async Task<bool> AddConfirmationData(Confirmdata confirmdata)
        {
            try
            {
                var loginUser = await _userManager.GetUserAsync(User);
                if (loginUser != null)
                {
                    List<string> images = new List<string>();
                    ViolationModel violationModel = _violationService.GetViolation(confirmdata.Id);
                    if (violationModel != null)
                    {
                        if (violationModel.Category == "camera" && confirmdata.selectedTypeId == 10)
                        {
                            List<ViolationModel> violationsWithSameCode = await _violationService.GetViolationByCode(violationModel.Code);
                            if (violationsWithSameCode != null)
                            {
                                foreach (var violation in violationsWithSameCode)
                                {
                                    images.Add(violation.imageName);
                                }
                            }
                            if (images.Count > 0)
                            {
                                foreach (var image in images)
                                {
                                    string uploadsFolder = Path.Combine(@"C:\inetpub\wwwroot\_driver\wwwroot/", "images/ViolationImages/");
                                    string filePath = Path.Combine(uploadsFolder, image);
                                    string uploadsFolderr = Path.Combine(@"C:\inetpub\wwwroot\_driver\wwwroot/", "images/NoViolationImages/");
                                    string filePathh = Path.Combine(uploadsFolderr, image);
                                   // System.IO.File.Copy(filePath, filePathh);
                                    if (System.IO.File.Exists(filePath))
                                    {
                                        if (System.IO.File.Exists(filePathh) == false)
                                        {
                                            System.IO.File.Copy(filePath, filePathh);
                                        }

                                    }
                                }
                            }

                        }
                    }

                    bool updateResult = _violationService.AddViolationConfirmationDetails(confirmdata.selectedTypeId, confirmdata.Id, loginUser.Id).Result;
                    return updateResult;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> RejectViolation(Confirmdata confirmdata)
        {
            try
            {
                var loginUser = _userManager.GetUserAsync(User).Result;
                if (loginUser != null)
                {
                    confirmdata.selectedTypeId = (int)CommanData.ViolationTypes.NoViolation;
                    bool updateResult = _violationService.AddViolationConfirmationDetails(confirmdata.selectedTypeId, confirmdata.Id, loginUser.Id).Result;
                    return updateResult;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public string ResetAll(string id)
        {
            try
            {
                IQueryable<UserViolationNotification> source = _repository.Find((Expression<Func<UserViolationNotification, bool>>)(un => un.Seen == false));
                if (source.Count<UserViolationNotification>() > 0)
                {
                    foreach (UserViolationNotification violationNotification in source.ToList<UserViolationNotification>())
                    {
                        violationNotification.Seen = true;
                        _repository.Update(violationNotification);
                    }
                }
                return "true";
            }
            catch (Exception ex)
            {
                return "false";
            }
        }

    }
}
