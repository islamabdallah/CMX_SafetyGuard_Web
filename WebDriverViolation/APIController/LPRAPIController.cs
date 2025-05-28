using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Take5.Services.Contracts;
using WebDriverViolation.Models.Models.MasterModels;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models.APIModels;
using WebDriverViolation.Services.Models.MasterModels;
using WebDriverViolation.Services.Models;
using WebDriverViolation.Models.Models;
using Microsoft.AspNetCore.Identity;
using Take5.Services.Implementation;
using WebDriverViolation.Services.Models.Messages;
using System;

namespace WebDriverViolation.APIController
{
    [Route("api/[controller]")]
    [ApiController]
    public class LPRAPIController : ControllerBase
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

        public LPRAPIController(IViolationService violationService, SignInManager<AspNetUser> signInManager,
            UserManager<AspNetUser> userManager,
             ITruckRunningTrackingService truckRunningTrackingService, ITruckDetailsService truckDetailsService,
             ILPRNotificationService violationNotificationService,IUserLPRNotificationService userLPRNotificationService,
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

        [HttpPost("AddLPR")]
        public async Task<ActionResult> AddLPR(LPRsAPIModell violation, int? LanguageId)
        {
            if(LanguageId==null) { LanguageId = 1; }

            if (violation == null)
                return (ActionResult)BadRequest((object)new
                {
                    Message = UserMessage.FailedProcess[(int)LanguageId],
                    Data = false
                });
            TruckModel truck = _truckService.GetTruck(violation.camera_name);
            if (truck == null)
                return (ActionResult)BadRequest((object)new
                {
                    Message = UserMessage.FailedProcess[(int)LanguageId],
                    Data = false
                });
            //if (!_truckService.GetCameraHasViolation(violation.camera_name, violation.violType))
            //    return (ActionResult)Ok((object)new
            //    {
            //        Message = "Successful process",
            //        Data = truck
            //    });
            //string name = Enum.GetName(typeof(CommanData.ViolationTypes), (object)violation.violType);
            var result = _rlogService.addLPRlog(violation).Result;
            if (result == null)
                return (ActionResult)BadRequest((object)new
                {
                    Message = UserMessage.FailedProcess[(int)LanguageId],
                    Data = false
                });
            //ViolationModel violationModel = addedViolationModels.FirstOrDefault<ViolationModel>();
            string[] strArray = new string[6]
            {
        " LPR(log) for ",
        truck.TruckName,
        " at ",
        null,
        null,
        null
            };
            strArray[3] = result.Date.ToString();
            strArray[4] = "&&&  , expeted Plate number= ";
            strArray[5] = result.OCR;
            string str2 = string.Concat(strArray);
            LPRNotification result3 = _violationNotificationService.CreateViolationNotification(str2, result.Id, "LPR").Result;
            if (result3 != null)
            {
                bool result4 = _violationNotificationService.HandleViolationNotificationToRole(str2, result3.Id, "Supervisor", "Security").Result;
                //bool result5 = _violationNotificationService.PushRealTimeViolationNotificationToRole(result3, "Supervisor", "Security", violationModel).Result;
            }


            return (ActionResult)Ok((object)new
            {
                Message = UserMessage.SuccessfulProcess[(int)LanguageId],
                Data = 0
            });
        }

        [HttpPost("pendingLPR")]
        public async Task<ActionResult> pendingLPR(long UserNumber, int LanguageId)
        {
            EmployeeModel employee = _employeeService.GetEmployee(UserNumber);
            if (employee != null)
            {

                AspNetUser aspNetUser = await _userManager.FindByIdAsync(employee.UserId);
                if (aspNetUser != null)
                {
                    var result = _rlogService.GetAllPendingLPRlogs();
                    result.notificationCount= _userLPRNotificationService.GetUserUnseenViolationNotificationCount(employee.UserId);
                        return Ok(new { Message = UserMessage.SuccessfulProcess[LanguageId], Data = result });                   
                }
                else
                {
                    return BadRequest(new { Message = UserMessage.FailedProcess[LanguageId], Data = 0 });
                }

            }
            else
            {
                return BadRequest(new { Message = UserMessage.LoginInvalidNumber[LanguageId], Data = 0 });
            }

        }

        [HttpPost("ConfirmLPR")]
        public async Task<ActionResult> ConfirmLPR(LPRsResponseAPIModel violation)
        {
            if (violation == null)
                return (ActionResult)BadRequest((object)new
                {
                    Message = UserMessage.FailedProcess[violation.LanguageId],
                    Data = false
                });
            EmployeeModel employee = _employeeService.GetEmployee(violation.userNumber);
            if (employee == null)
            {
                return BadRequest(new { Message = UserMessage.LoginInvalidNumber[violation.LanguageId], Data = 0 });
            }
                //TruckModel truck = _truckService.GetTruck(violation.camera_name);
                //if (truck == null)
                //    return (ActionResult)BadRequest((object)new
                //    {
                //        Message = "Failed Process,Invalid Camera Data",   
                //        Data = false
                //    });
                //if (!_truckService.GetCameraHasViolation(violation.camera_name, violation.violType))
                //    return (ActionResult)Ok((object)new
                //    {
                //        Message = "Successful process",
                //        Data = truck
                //    });
                //string name = Enum.GetName(typeof(CommanData.ViolationTypes), (object)violation.violType);
                
                var result = _rlogService.confiurmLPRlog(violation).Result;
            if (result == false)
                return (ActionResult)BadRequest((object)new
                {
                    Message = UserMessage.FailedProcess[violation.LanguageId],
                    Data = false
                });

            return (ActionResult)Ok((object)new
            {
                Message = UserMessage.SuccessfulProcess[violation.LanguageId],
                Data = 0
            });
        }


        [HttpPost("notifySammary")]
        public async Task<ActionResult> notifySammary(long UserNumber, int LanguageId)
        {
            EmployeeModel employee = _employeeService.GetEmployee(UserNumber);
            if (employee != null)
            {

                AspNetUser aspNetUser = await _userManager.FindByIdAsync(employee.UserId);
                if (aspNetUser != null)
                {
                    var result = _userLPRNotificationService.GetUserViolationNotification(employee.UserId);
                    //result.notificationCount = _userLPRNotificationService.GetUserUnseenViolationNotificationCount(employee.UserId);
                    return Ok(new { Message = UserMessage.SuccessfulProcess[LanguageId], Data = result });
                }
                else
                {
                    return BadRequest(new { Message = UserMessage.FailedProcess[LanguageId], Data = 0 });
                }

            }
            else
            {
                return BadRequest(new { Message = UserMessage.LoginInvalidNumber[LanguageId], Data = 0 });
            }

        }


        [HttpPost("LogFiles")]
        public async Task<IActionResult> LogFiles([FromForm] logfilesVM logfilesVM)//, IFormFile file)
        {
            try
            {
                string folderName= logfilesVM.date.ToString("yyyy-MM-dd");
                string folderPath = Path.Combine(@"C:\inetpub\wwwroot\_safetyGuard\wwwroot/LogsFiles/", logfilesVM.truckId);

                // Create the folder if it doesn't exist
                if (!Directory.Exists(folderPath))
                {
                    // Create the directory if it does not exist
                    Directory.CreateDirectory(folderPath);
                }
                // var uniqueFileName = string.Format(@"{0}", Guid.NewGuid());
                string uniqueFileName = null;
                string filePath = null;

                if (logfilesVM.file != null)
                {
                    string extension = Path.GetExtension(logfilesVM.file.FileName);
                    string uploadsFolderTest = Path.Combine(folderPath, "/");
                    string uploadsFolder = folderPath + "/" + folderName; //Path.Combine(uploadsFolderTest, folderName);
                    if (!Directory.Exists(uploadsFolder))
                    {
                        // Create the directory if it does not exist
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    // uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    uniqueFileName = logfilesVM.file.FileName;//DateTime.Now.ToString("yyyy-MM-dd HHmmtt") + "_logs" + extension;
                    uniqueFileName = uniqueFileName.Replace(" ", "");
                    // uniqueFileName = uniqueFileName.Replace("/ ", "-");
                    // uniqueFileName = uniqueFileName.Replace("\\ ", "-");
                    filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await logfilesVM.file.CopyToAsync(fileStream);
                    }
                    return Ok(new { Message = "Success Process", Data = uniqueFileName });
                }
                else
                {
                    return BadRequest(new { Message = "Failed Process", Data = 0 });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Failed Process", Data = false });
            }

        }


    }
}
