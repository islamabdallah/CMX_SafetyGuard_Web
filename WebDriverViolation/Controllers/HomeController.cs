using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using WebDriverViolation.Models;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models.MasterModels;
using WebDriverViolation.Services.Models.SpecificModel;

namespace WebDriverViolation.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IViolationService _violationService;
        private readonly IViolationTypeService _violationTypeService;
        private readonly ITruckService _truckService;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IUserViolationNotificationService _userViolationNotificationService;
        public HomeController(ILogger<HomeController> logger,
            IViolationService violationService,
            ITruckService truckService,
            IViolationTypeService violationTypeService,
            UserManager<AspNetUser> userManager,
            IUserViolationNotificationService userViolationNotificationService)
        {
            _logger = logger;
            _violationService = violationService;
            _truckService = truckService;
            _violationTypeService = violationTypeService;
            _userManager = userManager;
            _userViolationNotificationService = userViolationNotificationService;   
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    DashBoardModel dashBoardModel = new DashBoardModel();
                    var pendingViolations = await _violationService.GetAllPendingViolations();
                    var totalviolationPerType = await _violationService.GetActualViolationCountsperType();
                    dashBoardModel.PendingViolationCount = await _violationService.GetPendingViolationCount();
                    dashBoardModel.TotalActualViolationCount = await _violationService.GetAllActualViolationCount();
                    dashBoardModel.CurrentMonthActualViolationCount = await _violationService.GetCurrentMonthActualViolationCount();
                    dashBoardModel.TotalTrucksCount = await _truckService.GetAllActiveTrucksCount("Cement");
                    dashBoardModel.ViolationTypeModels =await _violationTypeService.GetAllViolationTypes();
                    if (pendingViolations != null)
                    {
                        dashBoardModel.LatestPendingViolationModels = pendingViolations.ToList();
                    }
                    else
                    {
                        dashBoardModel.LatestPendingViolationModels = new List<ViolationModel>();
                    }
                    if (totalviolationPerType != null)
                    {
                        List<ViolationTypeCount> violationTypeCounts = new List<ViolationTypeCount>();
                        foreach (var type in dashBoardModel.ViolationTypeModels)
                        {
                            violationTypeCounts.Add(new ViolationTypeCount
                            {
                                ViolationTypeId = type.ID,
                                ViolationTypeName = type.Name,
                                Count = totalviolationPerType
                            .Where(t => t.Key == type.Name).FirstOrDefault().Value
                            });
                        }
                        dashBoardModel.ViolationTypeCounts = violationTypeCounts.OrderBy(t => t.ViolationTypeId).ToList();
                    }
                    return View(dashBoardModel);

                }
                else
                {
                    return RedirectToAction("ERROR404");
                }
            }
            catch(Exception ex)
            {
                return RedirectToAction("ERROR404");
            }

        }


        public JsonResult getPendingViolationsCount()
        {
            try
            {
               long count = _violationService.GetPendingViolationCount().Result;
                return Json(count);
            }
            catch (Exception ex)
            {
                return Json(0);
            }
        }


        public string getViolationStats()
        {
            try
            {
                DashBoardModel dashBoardModel = new DashBoardModel();
                var totalviolationPerType = _violationService.GetActualViolationCountsperType().Result;
                dashBoardModel.PendingViolationCount = _violationService.GetPendingViolationCount().Result;
                dashBoardModel.TotalActualViolationCount = _violationService.GetAllActualViolationCount().Result;
                dashBoardModel.CurrentMonthActualViolationCount = _violationService.GetCurrentMonthActualViolationCount().Result;
                dashBoardModel.ViolationTypeModels = _violationTypeService.GetAllViolationTypes().Result;
                if (totalviolationPerType != null)
                {
                    List<ViolationTypeCount> violationTypeCounts = new List<ViolationTypeCount>();
                    foreach (var type in dashBoardModel.ViolationTypeModels)
                    {
                        violationTypeCounts.Add(new ViolationTypeCount
                        {
                            ViolationTypeId = type.ID,
                            ViolationTypeName = type.Name,
                            Count = totalviolationPerType
                        .Where(t => t.Key == type.Name).FirstOrDefault().Value
                        });
                    }
                    dashBoardModel.ViolationTypeCounts = violationTypeCounts.OrderBy(t => t.ViolationTypeId).ToList();
                }
                return JsonSerializer.Serialize(dashBoardModel);
            }
            catch(Exception ex) 
            { 
                return null;
            }

        }

        public bool updateSeenNotification()
        {
            var CurrentUser = _userManager.GetUserAsync(User).Result;
            if (CurrentUser != null)
            {
                bool result = _userViolationNotificationService.UpdateUserUnseenViolationNotification(CurrentUser.Id).Result;
                return result;
            }
            else
            {
                return false;
            }
        }

    }
}