using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Take5.Models.Models;
using Take5.Services.Contracts;
using Take5.Services.Implementation;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Controllers.Violations
{
    [Authorize]
    public class ViolationController : BaseController
    {
        private readonly IViolationTypeService _violationTypeService;
        private readonly IViolationService _violationService;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IEmployeeService _employeeService;
        private readonly IViolationTypeAccuracyLavelService _violationTypeAccuracyLavelService;
        public ViolationController(IViolationTypeService violationTypeService,
            IViolationService violationService,
            UserManager<AspNetUser> userManager,
            IEmployeeService employeeService,
            IViolationTypeAccuracyLavelService violationTypeAccuracyLavelService)
        {
            _violationTypeService= violationTypeService;
            _violationService = violationService;
            _userManager= userManager;
            _employeeService= employeeService;
            _violationTypeAccuracyLavelService = violationTypeAccuracyLavelService;
        }
        public ActionResult SearchViolation()
        
        {
            try
            {
                SearchViolationModel searchViolationModel = new SearchViolationModel();
                searchViolationModel = _violationService.InitiateViolationSearchModel(searchViolationModel).Result;

                return View(searchViolationModel);
            }
            catch(Exception ex)
            {
                return RedirectToAction("ERROR404");
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
                            searchViolationModel.ViolationModels = violatioModels;
                        }
                        else
                        {
                            searchViolationModel.ViolationModels = new List<ViolationModel>();
                        }
                        searchViolationModel = _violationService.InitiateViolationSearchModel(searchViolationModel).Result;
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

        public async Task<bool> AddConfirmationData(Confirmdata confirmdata)
        {
            try
            {
                var loginUser = await _userManager.GetUserAsync(User);
                if(loginUser != null)
                {
                    bool updateResult = _violationService.AddViolationConfirmationDetails(confirmdata.selectedTypeId, confirmdata.Id, loginUser.Id).Result;
                    return updateResult;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
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

    }
}
