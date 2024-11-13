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
            HomeController homeController = this;
            try
            {
                if (await homeController._userManager.GetUserAsync(homeController.User) == null)
                    return (IActionResult)homeController.RedirectToAction("ERROR404");
                DashBoardModel dashBoardModel1 = new DashBoardModel();
                List<ViolationModel> pendingViolations = await _violationService.GetAllPendingViolations();
                Dictionary<string, long> totalviolationPerType = await homeController._violationService.GetActualViolationCountsperType();
               // DashBoardModel dashBoardModel2 = dashBoardModel1;
                dashBoardModel1.CurrentMonthActualViolationCount = await homeController._violationService.GetCurrentMonthActualViolationCount();
               // dashBoardModel2 = (DashBoardModel)null;
                //dashBoardModel2 = dashBoardModel1;
                dashBoardModel1.TotalTrucksCount = await _truckService.GetAllActiveTrucksCount("Cement");
               // dashBoardModel2 = (DashBoardModel)null;
               // dashBoardModel2 = dashBoardModel1;
                dashBoardModel1.ViolationTypeModels = await homeController._violationTypeService.GetAllViolationTypes();
             //   dashBoardModel2 = (DashBoardModel)null;
                if (pendingViolations != null)
                {
                    dashBoardModel1.LatestPendingViolationModels = pendingViolations.ToList<ViolationModel>();
                    dashBoardModel1.PendingViolationCount = (long)pendingViolations.ToList<ViolationModel>().Where<ViolationModel>((Func<ViolationModel, bool>)(t => t.Category == "camera")).Count<ViolationModel>();
                    dashBoardModel1.TotalActualViolationCount = (long)pendingViolations.Where<ViolationModel>((Func<ViolationModel, bool>)(t => t.Category != "camera")).Count<ViolationModel>();
                }
                else
                {
                    dashBoardModel1.LatestPendingViolationModels = new List<ViolationModel>();
                    dashBoardModel1.PendingViolationCount = 0L;
                    dashBoardModel1.TotalActualViolationCount = 0L;
                }
                if (totalviolationPerType != null)
                {
                    List<ViolationTypeCount> source = new List<ViolationTypeCount>();
                    foreach (ViolationTypeModel violationTypeModel in dashBoardModel1.ViolationTypeModels)
                    {
                        ViolationTypeModel type = violationTypeModel;
                        source.Add(new ViolationTypeCount()
                        {
                            ViolationTypeId = type.ID,
                            ViolationTypeName = type.Name,
                            Count = totalviolationPerType.Where<KeyValuePair<string, long>>((Func<KeyValuePair<string, long>, bool>)(t => t.Key == type.Name)).FirstOrDefault<KeyValuePair<string, long>>().Value
                        });
                    }
                    dashBoardModel1.ViolationTypeCounts = source.OrderBy<ViolationTypeCount, int>((Func<ViolationTypeCount, int>)(t => t.ViolationTypeId)).ToList<ViolationTypeCount>();
                }
                return (IActionResult)homeController.View((object)dashBoardModel1);
            }
            catch (Exception ex)
            {
                return (IActionResult)homeController.RedirectToAction("ERROR404");
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