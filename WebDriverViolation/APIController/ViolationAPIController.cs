using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Take5.Services.Contracts;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Models.Models.MasterModels;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Implementation;
using WebDriverViolation.Services.Models;
using WebDriverViolation.Services.Models.APIModels;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.APIController
{

    [Route("api/[controller]")]
    [ApiController]
    public class ViolationAPIController : ControllerBase
    {
        private readonly IViolationService _violationService;
        private readonly ITruckRunningTrackingService _truckRunningTrackingService;
        private readonly IViolationNotificationService _violationNotificationService;
        private readonly IOutlookSenderService _outlookSenderService;
        private readonly ITruckService _truckService;
        private readonly IObjectMappingService _objectMappingService;

        private readonly IViolationTypeAccuracyLavelService _violationTypeAccuracyLavelService;

        public ViolationAPIController(IViolationService violationService,
             ITruckRunningTrackingService truckRunningTrackingService,
             IViolationNotificationService violationNotificationService,
             IOutlookSenderService outlookSenderService,
            ITruckService truckService,
            IObjectMappingService objectMappingService,
            IViolationTypeAccuracyLavelService violationTypeAccuracyLavelService
            )
        {
            _violationService = violationService;
            _truckRunningTrackingService = truckRunningTrackingService;
            _violationNotificationService = violationNotificationService;
            _outlookSenderService= outlookSenderService;    
            _truckService= truckService;
            _objectMappingService= objectMappingService;
            _violationTypeAccuracyLavelService = violationTypeAccuracyLavelService;
        }

        // POST api/<ViolationAPIController>
        [HttpPost("AddViolation")]
        public async Task<ActionResult> AddViolation(List<ViolationCollection> violationCollections)
        {
            bool result = false;
            string imageName = "";
            string notificationMessage = "";
            string mailBody = "";
            string violationType = "";
            bool validationResult = false;
            bool sendMail = true;
            double CallingProbabilitySum = 0;
            string code;
            int newCount;
            double sleepingAverageProbability = 0;
            double ViolationsTime = 0;
            int mode = 0;
            int sendMailStatus = -1;
            double minutesDfiffForPrevious = 0;
            double minutesDfiffForPreviousWithSendMail = 0;
            ViolationTypeAccuracyLavel violationTypeAccuracyLavel = null;
            double AverageProbability = 0;
            if (violationCollections != null)
            {
             
                foreach (var violationAPIModels in violationCollections)
                {
                    var truck = _truckService.GetTruck(violationAPIModels.TruckID);
                    ViolationsTime = Math.Abs(Math.Round(violationAPIModels.TotalTime, 2, MidpointRounding.AwayFromZero));
                    violationType = Enum.GetName(typeof(CommanData.ViolationTypes), violationAPIModels.TypeID);
                     
                    string lastTodayViolationCode = _violationService.GetLastViolationCodeForToday().Result;
                    
                    if (lastTodayViolationCode != "" && lastTodayViolationCode != null)
                    {
                        var count = lastTodayViolationCode.Substring(8);
                        newCount = Convert.ToInt32(count) + 1;
                        code = DateTime.Now.Date.ToString("yyyyMMdd") + newCount;
                    }
                    else
                    {
                        code = DateTime.Now.Date.ToString("yyyyMMDD") + "1";
                    }
                    List<Violation> newViolations = new List<Violation>();
                    if (violationAPIModels != null)
                    {
                        if ((violationAPIModels.ViolationAPIModels.Count == CommanData.numberofInstancesPerViolation )|| (violationAPIModels.TypeID == (int)CommanData.ViolationTypes.Camera_Coverd_ID)|| (violationAPIModels.TypeID == (int)CommanData.ViolationTypes.Camera_Cant_Open_ID)|| (violationAPIModels.TypeID == (int)CommanData.ViolationTypes.Camera_Cant_Read_ID))
                        {
                            if (truck != null)
                            {
                                var lastPreviousAddedViolation = await _violationService.GetLastViolationForSpecificTypeAndTruck(violationAPIModels.TypeID, violationAPIModels.TruckID);
                                var lastPreviousAddedViolationWithSendMail = await _violationService.GetLastViolationForSpecificTypeAndTruckWithMail(violationAPIModels.TypeID, violationAPIModels.TruckID);

                                if (violationAPIModels.ModeLight == true) 
                                {
                                    mode = 1;
                                }
                                else
                                {
                                    mode = 2;
                                }
                                if(violationAPIModels.ViolationAPIModels.Count > 1)
                                {
                                    AverageProbability = violationAPIModels.AverageProbability;//(violationAPIModels.ViolationAPIModels[0].Probability + violationAPIModels.ViolationAPIModels[1].Probability + violationAPIModels.ViolationAPIModels[2].Probability)/ 3;
                                    violationTypeAccuracyLavel = _violationTypeAccuracyLavelService.GetViolationAccuracyLavelForViolation(violationAPIModels.TypeID, AverageProbability, violationAPIModels.ViolationAPIModels[1].Date, mode);
                                }
                                else
                                {
                                    violationTypeAccuracyLavel = null;
                                }
                                if (violationTypeAccuracyLavel != null)
                                {
                                        if(violationTypeAccuracyLavel.SendMail == true)
                                        {
                                            sendMail = true;
                                            sendMailStatus = 1;
                                        }
                                        else
                                        {
                                            sendMail = false;
                                             sendMailStatus = 2;
                                        }
                                        notificationMessage = violationType + " Violation for " + truck.TruckName + " at " +
                                        violationAPIModels.ViolationAPIModels[0].Date + "&&&  , Average Probability = " +
                                        Math.Round(AverageProbability, 2, MidpointRounding.AwayFromZero) + "  time for 5 frames = " +  ViolationsTime + "&&& , Violation Level = " + violationTypeAccuracyLavel.Description.ToString();
                                }
                                else
                                    {
                                        sendMail = false;
                                        sendMailStatus = 2;
                                        notificationMessage = violationType + " Violation for " + truck.TruckName + " at " +
                                        violationAPIModels.ViolationAPIModels[0].Date + "&&&  , Average Probability = " +
                                        Math.Round(AverageProbability, 2, MidpointRounding.AwayFromZero) + "  time for 5 frames = " + ViolationsTime;
                                    }
                                if (lastPreviousAddedViolation != null)
                                {
                                    if(violationAPIModels.ViolationAPIModels.Count > 1)
                                    {
                                        if (lastPreviousAddedViolationWithSendMail != null)
                                        {
                                            minutesDfiffForPrevious = Math.Abs(violationAPIModels.ViolationAPIModels[2].Date.Subtract(lastPreviousAddedViolation.Date).TotalSeconds);
                                            minutesDfiffForPreviousWithSendMail = Math.Abs(violationAPIModels.ViolationAPIModels[2].Date.Subtract(lastPreviousAddedViolationWithSendMail.Date).TotalSeconds);
                                        }
                                        else
                                        {
                                            minutesDfiffForPrevious = 0;
                                            minutesDfiffForPreviousWithSendMail = 0;
                                        }
                                    }
                                    else
                                    {
                                        minutesDfiffForPrevious = 100;
                                    }
                                    //if (sendMailStatus == 1 && truck.SendMail == true && (lastPreviousAddedViolation.MailSent == 0 ) )
                                    if (sendMailStatus == 1 && truck.SendMail == true && violationAPIModels.ModeMoving == true)
                                    {
                                        if (lastPreviousAddedViolation.ViolationTypeID ==7 && minutesDfiffForPrevious <= 60 && minutesDfiffForPreviousWithSendMail > 300)
                                        {
                                            sendMail = true;
                                        }
                                        else if(minutesDfiffForPrevious <= 180 && lastPreviousAddedViolation.ViolationTypeID != 7)
                                        {
                                            sendMail = true;
                                        }
                                        else
                                        {
                                            sendMail = false;
                                            sendMailStatus = 0;
                                        }
                                    }
                                    else
                                    {
                                        sendMail = false;
                                        sendMailStatus = 0;
                                    }
                                }
                                else
                                {
                                    sendMail = false;
                                    sendMailStatus = 0;
                                }
                                foreach (var violation in violationAPIModels.ViolationAPIModels)
                                {
                                    //validationResult = _objectMappingService.ValidateObjectProperities(violation);
                                    validationResult = true;
                                    if (validationResult == true)
                                    {
                                        if (violation.image != null)
                                        {
                                            imageName = _violationService.SaveImage(violation.image, CommanData.ViolationFolder).Result;
                                            violation.image = imageName;
                                        }
                                        else
                                        {
                                            violation.image = "test.jpg";
                                        }
                                    }
                                    violation.Code = code;
                                    var violationObject =  _violationService.ConvertFromViolationAPIToViolation(violation, violationAPIModels.TypeID, violationAPIModels.AverageProbability, violationAPIModels.TruckID, violationAPIModels.TotalTime, violationAPIModels.ModeMoving, violationAPIModels.ModeLight);
                                    if (violationObject != null)
                                    {
                                        violationObject.MailSent = sendMailStatus;
                                        newViolations.Add(violationObject);
                                    }
                                }
                            var addedViolationModels = _violationService.CreateViolation(newViolations).Result;
                                if (addedViolationModels != null)
                                {
                                    //if(addedViolationModels.First().ViolationTypeID == (int)CommanData.ViolationTypes.Sleeping)
                                    //{
                                    //     //sleepingAverageProbability =(addedViolationModels.Select(x => x.Probability).Sum())/3;
                                    //    sleepingTime = addedViolationModels.Where(v => v.AverageProbability != 0.86).FirstOrDefault().AverageProbability;
                                    //}
                                    ViolationModel addedViolationModel = addedViolationModels.OrderBy(v => v.Probability).Last();
                                    //addedViolationModel.Probability = addedViolationModels.Select(v => v.Probability).Max();

                                    //if(addedViolationModel.ViolationTypeID == (int)CommanData.ViolationTypes.Sleeping)
                                    //{
                                         //violationTypeAccuracyLavel = _violationTypeAccuracyLavelService.GetViolationAccuracyLavelForViolation(addedViolationModel.ViolationTypeID, sleepingAverageProbability, addedViolationModel.Date, mode);
                                
                                    //}
                                    //else
                                    //{
                                    //     //violationTypeAccuracyLavel = _violationTypeAccuracyLavelService.GetViolationAccuracyLavelForViolation(addedViolationModel.ViolationTypeID, addedViolationModel.AverageProbability, addedViolationModel.Date, mode);

                                    //    notificationMessage = violationType + " Violation for Truck" + addedViolationModel.TruckID + " at " +
                                    //    addedViolationModel.Date + "&&&  , Average Probability = " +
                                    //    addedViolationModel.AverageProbability  + "&&& , Violation Level = " + violationTypeAccuracyLavel.Description.ToString();
                                    //}
                                    var addedViolationNotification = _violationNotificationService.CreateViolationNotification(notificationMessage, addedViolationModel.Id).Result;
                                    if (addedViolationNotification != null)
                                    {
                                        result = _violationNotificationService.HandleViolationNotificationToRole(notificationMessage, addedViolationNotification.Id, "Supervisor", "Security").Result;
                                        var test = _violationNotificationService.PushRealTimeViolationNotificationToRole(addedViolationNotification, "Supervisor", "Security", addedViolationModel).Result;
                                    }
                                    //if (addedViolationModel.ViolationTypeID == (int)CommanData.ViolationTypes.Sleeping && sleepingAverageProbability < 0.85)
                                    //{
                                    //    sendMail = false;
                                    //}
                                    //else if (addedViolationModel.ViolationTypeID == (int)CommanData.ViolationTypes.Texting && addedViolationModel.AverageProbability < 0.95)
                                    //{
                                    //    sendMail = false;
                                    //}
                                    //else if (addedViolationModel.ViolationTypeID == (int)CommanData.ViolationTypes.CallingLeft && addedViolationModel.AverageProbability < 0.92)
                                    //{
                                    //    //CallingProbabilitySum = addedViolationModel.AllClassessProbability[2] + addedViolationModel.AllClassessProbability[3];
                                    //    //if(CallingProbabilitySum < 0.80)
                                    //    //{
                                    //    sendMail = false;
                                    //    //}
                                    //}
                                    //else if (addedViolationModel.ViolationTypeID == (int)CommanData.ViolationTypes.CallingRight && addedViolationModel.AverageProbability < 0.70)
                                    //{
                                    //    sendMail = false;
                                    //}
                                    //else if (addedViolationModel.ViolationTypeID == (int)CommanData.ViolationTypes.GettingBack && addedViolationModel.AverageProbability < 0.88)
                                    //{
                                    //    sendMail = false;
                                    //}
                                    //else if (addedViolationModel.ViolationTypeID == (int)CommanData.ViolationTypes.Drinking && addedViolationModel.AverageProbability < 0.90)
                                    //{
                                    //    sendMail = false;
                                    //}
                                    //else if ((violationAPIModels.ViolationAPIModels.First().TypeID == (int)CommanData.ViolationTypes.Camera_Coverd_ID) || (violationAPIModels.ViolationAPIModels.First().TypeID == (int)CommanData.ViolationTypes.Camera_Cant_Open_ID) || (violationAPIModels.ViolationAPIModels.First().TypeID == (int)CommanData.ViolationTypes.Camera_Cant_Read_ID))
                                    //{
                                    //    sendMail = false;

                                    //}
                                    //else if(addedViolationModel.TruckID == "TestTruck")
                                    //{
                                    //    sendMail= false;
                                    //}
                                    // ***************write method to decide to send mail or not according to last send time ****************
                                    List<string> toMails = truck.MailList.Split(";").ToList();
                                    if (sendMail == true && toMails != null)
                                    {
                                        if(toMails.Count > 0)
                                        {
                                            mailBody = _outlookSenderService.PrapareMailBody(addedViolationModel, notificationMessage);
                                            //List<string> toMails = new List<string>
                                            //{
                                            //    "doaa.abdel@ext.cemex.com",
                                            //    "mahmoud.saleh@ext.cemex.com",
                                            //    "mohamed.ahmedz@cemex.com",
                                            //    "islammohamed.abdallah@cemex.com"
                                            //};

                                           await  _outlookSenderService.SendEmail(mailBody, toMails, violationType + " Driver violation", null);
                                        }
                                    }
                                }
                                else
                                {
                                    return BadRequest(new { Message = "Failed Process", Data = false });
                                }
                            }
                            else
                            {
                                return BadRequest(new { Message = "Failed Process, invaild truck number", Data = false });
                            }
                        }
                        else
                        {
                            return BadRequest(new { Message = "Failed Process, invaild data", Data = false });
                        }
                    }
                    else
                    {
                        return BadRequest(new { Message = "Failed Process, invaild data", Data = false });
                    }
                }
            }
            else
            {
                return BadRequest(new { Message = "Failed Process, invaild data", Data = false });
            }

            if (result == true)
                {
                    return Ok(new { Message = "Successful process", Data = true });
                }
                else
                {
                    return BadRequest(new { Message = "Failed Process", Data = false });
                }
            }
           

        [HttpPost("AddTruckStatus")]
        public async Task<ActionResult> AddTruckStatus(List<TruckRunningTrackingAPIModel> truckRunningTrackingAPIModels)
        {
            bool result = false;
            bool validationResult = false;
            if (truckRunningTrackingAPIModels != null)
            {
                if (truckRunningTrackingAPIModels.Count > 0)
                {
                    var truck = _truckService.GetTruck(truckRunningTrackingAPIModels[0].TruckID);
                    if (truck != null)
                    {
                        foreach (var item in truckRunningTrackingAPIModels)
                        {
                            validationResult = _objectMappingService.ValidateObjectProperities(item);
                            if(validationResult== true)
                            {
                                result = _truckRunningTrackingService.CreateTruckRunningTracking(item).Result;
                            }
                            else
                            {
                                result= false;
                            }
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
                if (result == true)
                {
                    return Ok(new { Message = "Successful process", Data = true });
                }
                else
                {
                    return BadRequest(new { Message = "Failed Process", Data = false });
                }
            }
            else
            {
                return BadRequest(new { Message = "can not accept empty object", Data = false });
            }
        }
    }
}
