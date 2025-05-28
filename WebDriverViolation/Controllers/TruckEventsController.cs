using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Take5.Services.Contracts;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Controllers
{
    public class TruckEventsController : Controller
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
        //public readonly ITruckDetailsService TruckDetailsService;

        public TruckEventsController(IViolationService violationService, SignInManager<AspNetUser> signInManager,
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
            SearchTruckEventModel model = new SearchTruckEventModel();
            model = _truckDetailsService.InitiateTruckEventSearchModel(model).Result;
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(SearchTruckEventModel searchTruckEventModel, string submitButton)
        {
            SearchTruckEventModel model = new SearchTruckEventModel();
            model = _truckDetailsService.InitiateTruckEventSearchModel(model).Result;
            searchTruckEventModel.Trucks=model.Trucks;
            searchTruckEventModel.TruckViolationTypes=model.TruckViolationTypes;
            searchTruckEventModel.TruckDetails = _truckDetailsService.SearchForTruckEvent(searchTruckEventModel).Result;
            if (submitButton == "Export")
            {
                if (searchTruckEventModel.TruckDetails != null)
                {
                    if (searchTruckEventModel.TruckDetails.Count > 0)
                    {
                        MemoryStream memoryStream = _truckDetailsService.ExportTruckEvent(searchTruckEventModel.TruckDetails);

                        return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TruckEventsReport.xlsx");

                    }
                    else
                    {
                        TempData["Error"] = "Error in file exporting, There is no Truck Events";
                    }
                }
            }
            return View(searchTruckEventModel);
        }

        public IActionResult logs()
        {
            SearchlogfilesModel model = new SearchlogfilesModel();
            model = _truckDetailsService.InitiatelogfilesSearchModel(model).Result;
            return View(model);
        }

        [HttpPost]
        public IActionResult logs(SearchlogfilesModel searchTruckEventModel, string submitButton)
        {
            SearchlogfilesModel model = new SearchlogfilesModel();
            model = _truckDetailsService.InitiatelogfilesSearchModel(model).Result;
            searchTruckEventModel.Trucks = model.Trucks;
            //searchTruckEventModel.TruckViolationTypes = model.TruckViolationTypes;
            searchTruckEventModel.logfiles = _truckDetailsService.SearchForTruckLogfiles(searchTruckEventModel).Result;
            return View(searchTruckEventModel);
        }

        public async Task<IActionResult> DownloadFile(string path)
        {
            TruckEventsController truckEventsController = this;
            MemoryStream memory = new MemoryStream();
            using (FileStream stream = new FileStream(path, FileMode.Open))
                await stream.CopyToAsync((Stream)memory);
            memory.Position = 0L;
            string contentType = "APPLICATION/octet-stream";
            string fileName = Path.GetFileName(path);
            IActionResult actionResult = (IActionResult)truckEventsController.File((Stream)memory, contentType, fileName);
            path = (string)null;
            memory = (MemoryStream)null;
            return actionResult;
        }
    }
}
