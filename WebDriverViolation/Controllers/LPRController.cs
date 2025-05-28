using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using Take5.Services.Contracts;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Models.Models.MasterModels;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models;
using WebDriverViolation.Services.Models.APIModels;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Controllers
{
    public class LPRController : Controller
    {
        private readonly IViolationService _violationService;
        private readonly ITruckRunningTrackingService _truckRunningTrackingService;
        private readonly ITruckDetailsService _truckDetailsService;
        private readonly ILPRNotificationService _violationNotificationService;
        private readonly IUserLPRNotificationService _userLPRNotificationService;
        private readonly IOutlookSenderService _outlookSenderService;
        private readonly ITruckService _truckService;
        private readonly IObjectMappingService _objectMappingService;
        private readonly ILPRlogService _rlogService;
        private readonly IEmployeeService _employeeService;
        private readonly IViolationTypeAccuracyLavelService _violationTypeAccuracyLavelService;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly SignInManager<AspNetUser> _signInManager;

        public LPRController(IViolationService violationService, SignInManager<AspNetUser> signInManager,
            UserManager<AspNetUser> userManager,
             ITruckRunningTrackingService truckRunningTrackingService, ITruckDetailsService truckDetailsService,
             ILPRNotificationService violationNotificationService, IUserLPRNotificationService userLPRNotificationService,
             IOutlookSenderService outlookSenderService,
            ITruckService truckService,
            IObjectMappingService objectMappingService, IEmployeeService employeeService,
            IViolationTypeAccuracyLavelService violationTypeAccuracyLavelService, ILPRlogService rlogService
            )
        {
            _violationService = violationService;
            _truckRunningTrackingService = truckRunningTrackingService;
            _violationNotificationService = violationNotificationService;
            _outlookSenderService = outlookSenderService;
            _truckService = truckService;
            _objectMappingService = objectMappingService;
            _violationTypeAccuracyLavelService = violationTypeAccuracyLavelService;
            _truckDetailsService = truckDetailsService;
            _employeeService = employeeService;
            _rlogService = rlogService;
            _signInManager = signInManager;
            _userManager = userManager;
            _userLPRNotificationService = userLPRNotificationService;
        }
        public IActionResult Index()
        {
            SearchLPRModel model = new SearchLPRModel();
            model=_rlogService.InitiateLPRSearchModel(model).Result;
            if(model.LPRlogs != null)
            {
                if(model.LPRlogs.Count > 0)
                {
                    foreach(var l in model.LPRlogs)
                    {
                        l.ImageName= "http://20.86.97.165/DriverViolation/" + CommanData.LPRFolder + l.ImageName;
                    }
                }
            }
            //LPRlogAPIModel result = _rlogService.GetAllPendingLPRlogs();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(SearchLPRModel searchViolationModel)
        {
            SearchLPRModel model = new SearchLPRModel();
            model = _rlogService.InitiateLPRSearchModel(model).Result;
            try
            {
                AspNetUser result1 = _userManager.GetUserAsync(User).Result;
                if (result1 == null)
                    return RedirectToAction("ERROR404");
                if (!(result1.Company == "Security"))
                    return RedirectToAction("ERROR404");
                List<LPRlogs> result2 = _rlogService.SearchForViolation(searchViolationModel).Result;
                searchViolationModel.LPRlogs = result2 == null ? new List<LPRlogs>() : result2;
                if (searchViolationModel.LPRlogs != null)
                {
                    if(searchViolationModel.LPRlogs.Count>0)
                    { 
                        foreach(var l in searchViolationModel.LPRlogs)
                        {
                            l.ImageName = "http://20.86.97.165/DriverViolation/" + CommanData.LPRFolder + l.ImageName;
                        }
                    }
                }
                searchViolationModel.Trucks = model.Trucks;
                searchViolationModel.FeedBacks = model.FeedBacks;
                searchViolationModel.ConfirmationStatusModel = model.ConfirmationStatusModel;
                return View("Index", searchViolationModel);
            }
            catch (Exception ex)
            {
                return (ActionResult)this.RedirectToAction("ERROR404");
            }
           
        }

        [HttpPost]
        public async Task<bool> AddConfirmationData(LPRsResponseAPIModel violation)
        {
            try
            {
                var loginUser = await _userManager.GetUserAsync(User);

                if (loginUser != null)
                {
                    Employee employee =await _employeeService.GetEmployeeByUserId(loginUser.Id);
                    if (employee != null)
                    {
                        violation.userNumber = employee.EmployeeNumber;
                    }
                    var result = _rlogService.confiurmLPRlog(violation).Result;
                    return result;
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
