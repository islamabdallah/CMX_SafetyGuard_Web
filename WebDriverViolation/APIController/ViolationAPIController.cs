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
        private readonly ITruckDetailsService _truckDetailsService;
        private readonly IViolationNotificationService _violationNotificationService;
        private readonly IOutlookSenderService _outlookSenderService;
        private readonly ITruckService _truckService;
        private readonly IObjectMappingService _objectMappingService;
        private readonly IViolationTypeService _violationTypeService;

        private readonly IViolationTypeAccuracyLavelService _violationTypeAccuracyLavelService;

        public ViolationAPIController(IViolationService violationService,
             ITruckRunningTrackingService truckRunningTrackingService,ITruckDetailsService truckDetailsService,
             IViolationNotificationService violationNotificationService,
             IOutlookSenderService outlookSenderService,
            ITruckService truckService,
            IObjectMappingService objectMappingService,
            IViolationTypeAccuracyLavelService violationTypeAccuracyLavelService, IViolationTypeService violationTypeService
            )
        {
            _violationService = violationService;
            _truckRunningTrackingService = truckRunningTrackingService;
            _violationNotificationService = violationNotificationService;
            _outlookSenderService= outlookSenderService;    
            _truckService= truckService;
            _objectMappingService= objectMappingService;
            _violationTypeAccuracyLavelService = violationTypeAccuracyLavelService;
            _truckDetailsService=truckDetailsService;
            _violationTypeService= violationTypeService;
        }

        // POST api/<ViolationAPIController>
        [HttpPost("AddViolationOld")]
        public async Task<ActionResult> AddViolationOld(List<ViolationCollection> violationCollections)
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
                                    var addedViolationNotification = _violationNotificationService.CreateViolationNotification(notificationMessage, addedViolationModel.Id,null).Result;
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
           

        [HttpPost("AddTruckStatusOld")]
        public async Task<ActionResult> AddTruckStatusOld(List<TruckRunningTrackingAPIModel> truckRunningTrackingAPIModels)
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

        [HttpPost("AddViolation")]
        public async Task<ActionResult> AddViolation(List<ViolationCollection> violationCollections)
        {
            ViolationAPIController violationApiController = this;
            bool result = false;
            string violationType = "";
            bool flag1 = true;
            double ViolationsTime = 0.0;
            double minutesDfiffForPreviousWithSendMail = 0.0;
            double AverageProbability = 0.0;
            string name = "";
            if (violationCollections == null)
                return (ActionResult)violationApiController.BadRequest((object)new
                {
                    Message = "Failed Process, invaild data",
                    Data = false
                });
            foreach (ViolationCollection violationAPIModels in violationCollections)
            {
                TruckModel truck = violationApiController._truckService.GetTruck(violationAPIModels.TruckID);
                ViolationsTime = Math.Abs(Math.Round(violationAPIModels.TotalTime, 2, MidpointRounding.AwayFromZero));
                violationType = Enum.GetName(typeof(CommanData.ViolationTypes), (object)violationAPIModels.TypeID);
                string result1 = violationApiController._violationService.GetLastViolationCodeForToday().Result;
                string code;
                DateTime date;
                if (result1 != "" && result1 != null)
                {
                    int num = Convert.ToInt32(result1.Substring(8)) + 1;
                    date = DateTime.Now.Date;
                    code = date.ToString("yyyyMMdd") + num.ToString();
                }
                else
                {
                    DateTime dateTime = DateTime.Now;
                    dateTime = dateTime.Date;
                    code = dateTime.ToString("yyyyMMdd") + "1";
                }
                List<Violation> newViolations = new List<Violation>();
                if (violationAPIModels == null)
                    return (ActionResult)violationApiController.BadRequest((object)new
                    {
                        Message = "Failed Process, invaild data",
                        Data = false
                    });
                if (violationAPIModels.ViolationAPIModels.Count != CommanData.numberofInstancesPerViolation && violationAPIModels.TypeID != 9 && violationAPIModels.TypeID != 8 && violationAPIModels.TypeID != 6)
                    return (ActionResult)violationApiController.BadRequest((object)new
                    {
                        Message = "Failed Process, invaild data",
                        Data = false
                    });
                if (truck == null)
                    return (ActionResult)violationApiController.BadRequest((object)new
                    {
                        Message = "Failed Process, invaild truck number",
                        Data = false
                    });
                ViolationModel lastPreviousAddedViolation = await violationApiController._violationService.GetLastViolationForSpecificTypeAndTruck(violationAPIModels.TypeID, violationAPIModels.TruckID);
                ViolationModel andTruckWithMail = await violationApiController._violationService.GetLastViolationForSpecificTypeAndTruckWithMail(violationAPIModels.TypeID, violationAPIModels.TruckID);
                int mode = !violationAPIModels.ModeLight ? 2 : 1;
                ViolationTypeAccuracyLavel typeAccuracyLavel;
                if (violationAPIModels.ViolationAPIModels.Count > 1)
                {
                    AverageProbability = violationAPIModels.AverageProbability;
                    typeAccuracyLavel = violationApiController._violationTypeAccuracyLavelService.GetViolationAccuracyLavelForViolation(violationAPIModels.TypeID, AverageProbability, violationAPIModels.ViolationAPIModels[1].Date, mode);
                }
                else
                    typeAccuracyLavel = (ViolationTypeAccuracyLavel)null;
                int num1;
                double num2;
                string str1;
                if (typeAccuracyLavel != null)
                {
                    if (typeAccuracyLavel.SendMail)
                    {
                        flag1 = true;
                        num1 = 1;
                    }
                    else
                    {
                        flag1 = false;
                        num1 = 2;
                    }
                    string[] strArray = new string[11];
                    strArray[0] = violationType;
                    strArray[1] = " Violation for ";
                    strArray[2] = truck.TruckName;
                    strArray[3] = " at ";
                    date = violationAPIModels.ViolationAPIModels[0].Date;
                    strArray[4] = date.ToString();
                    strArray[5] = "&&&  , Average Probability = ";
                    num2 = Math.Round(AverageProbability, 2, MidpointRounding.AwayFromZero);
                    strArray[6] = num2.ToString();
                    strArray[7] = "  time for 5 frames = ";
                    strArray[8] = ViolationsTime.ToString();
                    strArray[9] = "&&& , Violation Level = ";
                    strArray[10] = typeAccuracyLavel.Description.ToString();
                    str1 = string.Concat(strArray);
                }
                else
                {
                    flag1 = false;
                    num1 = 2;
                    string[] strArray = new string[9];
                    strArray[0] = violationType;
                    strArray[1] = " Violation for ";
                    strArray[2] = truck.TruckName;
                    strArray[3] = " at ";
                    date = violationAPIModels.ViolationAPIModels[0].Date;
                    strArray[4] = date.ToString();
                    strArray[5] = "&&&  , Average Probability = ";
                    num2 = Math.Round(AverageProbability, 2, MidpointRounding.AwayFromZero);
                    strArray[6] = num2.ToString();
                    strArray[7] = "  time for 5 frames = ";
                    strArray[8] = ViolationsTime.ToString();
                    str1 = string.Concat(strArray);
                }
                bool flag2;
                if (lastPreviousAddedViolation != null)
                {
                    double num3;
                    if (violationAPIModels.ViolationAPIModels.Count > 1)
                    {
                        if (andTruckWithMail != null)
                        {
                            date = violationAPIModels.ViolationAPIModels[2].Date;
                            TimeSpan timeSpan = date.Subtract(lastPreviousAddedViolation.Date);
                            num3 = Math.Abs(timeSpan.TotalSeconds);
                            date = violationAPIModels.ViolationAPIModels[2].Date;
                            timeSpan = date.Subtract(andTruckWithMail.Date);
                            minutesDfiffForPreviousWithSendMail = Math.Abs(timeSpan.TotalSeconds);
                        }
                        else
                        {
                            num3 = 0.0;
                            minutesDfiffForPreviousWithSendMail = 0.0;
                        }
                    }
                    else
                        num3 = 100.0;
                    //if (violationAPIModels.ViolationAPIModels[0].IsTruckMoving==false)
                    //{
                    //    flag2 = false;
                    //}
                     if (num1 == 1)
                    {
                        flag2 = lastPreviousAddedViolation.ViolationTypeID == 7 && num3 <= 60.0 && minutesDfiffForPreviousWithSendMail > 300.0 || num3 <= 180.0 && lastPreviousAddedViolation.ViolationTypeID != 7 || true;
                    }
                    else
                    {
                        flag2 = false;
                        num1 = 0;
                    }
                }
                else
                {
                    flag2 = false;
                    num1 = 0;
                }
                int lmIndex = 0;
                foreach (ViolationAPIModel violationApiModel in violationAPIModels.ViolationAPIModels)
                {
                    if (true)
                    {
                        if (violationApiModel.image != null)
                        {
                            lmIndex++;
                            string result2 = violationApiController._violationService.SaveImageDriver(violationApiModel.image, CommanData.ViolationFolder, lmIndex, violationType, violationAPIModels.TruckID).Result;
                           // string result2 = violationApiController._violationService.SaveImage(violationApiModel.image, CommanData.ViolationFolder).Result;
                            violationApiModel.image = result2;
                        }
                        else
                            violationApiModel.image = "test.jpg";
                    }
                    violationApiModel.Code = code;
                    violationApiModel.Category = "Driver";
                    Violation violation = violationApiController._violationService.ConvertFromViolationAPIToViolation(violationApiModel, violationAPIModels.TypeID, violationAPIModels.AverageProbability, violationAPIModels.TruckID, violationAPIModels.TotalTime, violationAPIModels.ModeMoving, violationAPIModels.ModeLight);
                    if (violation != null)
                    {
                        violation.MailSent = num1;
                        newViolations.Add(violation);
                    }
                }
                List<ViolationModel> result3 = violationApiController._violationService.CreateViolation(newViolations).Result;
                if (result3 == null)
                    return (ActionResult)violationApiController.BadRequest((object)new
                    {
                        Message = "Failed Process",
                        Data = false
                    });
                ViolationModel violationModel = result3.OrderBy<ViolationModel, double>((Func<ViolationModel, double>)(v => v.Probability)).Last<ViolationModel>();
                ViolationNotificationModel result4 = _violationNotificationService.CreateViolationNotification(str1, violationModel.Id, (string)null).Result;
                if (result4 != null)
                {
                    result = violationApiController._violationNotificationService.HandleViolationNotificationToRole(str1, result4.Id, "Supervisor", "Security").Result;
                    bool result5 = violationApiController._violationNotificationService.PushRealTimeViolationNotificationToRole(result4, "Supervisor", "Security", violationModel).Result;
                }
                List<string> list = ((IEnumerable<string>)truck.MailList.Split(";")).ToList<string>();
                if (flag2 && violationModel.IsTruckMoving == true)//(flag2 && list != null && list.Count > 0 && violationModel.IsTruckMoving==true)
                {
                    string str2 = (!violationModel.IsTruckMoving ? str1 + " , IsTruckMoving= false" : str1 + " , IsTruckMoving= true") + " ,Mode= " + mode.ToString();
                    num2 = violationModel.TotalTime;
                    string str3 = num2.ToString();
                    string violationMessage = str2 + " ,Total Time= " + str3;
                    string body = violationApiController._outlookSenderService.PrapareMailBody(violationModel, violationMessage);
                    List<string> toMails = new List<string>()
          {
            "mahmoud.saleh@ext.cemex.com",
            "eman.rasmy @cemex.com",
            "ahmedabdel.wahab@cemex.com",
            "amralaaeldin.saleh@ext.cemex.com"
          };
                    if(violationAPIModels.TypeID == 7)
                    {
                        //toMails.Add("lamia.mousa@ext.cemex.com");
                        toMails.Add("islammohamed.abdallah@cemex.com");
                    }
                    await violationApiController._outlookSenderService.SendEmail(body, toMails, " Driving Violation Alert | " + violationType);
                }
                lastPreviousAddedViolation = (ViolationModel)null;
                truck = (TruckModel)null;
                newViolations = (List<Violation>)null;
            }
            return !result ? (ActionResult)violationApiController.BadRequest((object)new
            {
                Message = "Failed Process",
                Data = false
            }) : (ActionResult)violationApiController.Ok((object)new
            {
                Message = "Successful process",
                Data = true
            });
        }

        [HttpPost("AddPPEViolation")]
        public async Task<ActionResult> AddPPEViolation(PPsViolationAPIModel violation)
        {
            ViolationAPIController violationApiController = this;
            if (violation == null)
                return (ActionResult)violationApiController.BadRequest((object)new
                {
                    Message = "Failed Process,Invalid Violation Data",
                    Data = false
                });
            TruckModel truck = violationApiController._truckService.GetTruck(violation.camera_name);
            if (truck == null)
                return (ActionResult)violationApiController.BadRequest((object)new
                {
                    Message = "Failed Process,Invalid Camera Data",
                    Data = false
                });
            if (!violationApiController._truckService.GetCameraHasViolation(violation.camera_name, violation.violType))
                return (ActionResult)violationApiController.Ok((object)new
                {
                    Message = "Successful process",
                    Data = truck
                });
            var typeModel = _violationTypeService.GetViolationType(violation.violType);

            string name = "";// Enum.GetName(typeof(CommanData.ViolationTypes), (object)violation.violType);
            string category= "camera";
            if (typeModel != null)
            {
                name = typeModel.Name;
                category = typeModel.Category;
            }
            string result1 = violationApiController._violationService.GetLastViolationCodeForToday().Result;
            string str1;
            DateTime dateTime1;
            if (result1 != "" && result1 != null)
            {
                int num = Convert.ToInt32(result1.Substring(8)) + 1;
                DateTime dateTime2 = DateTime.Now;
                dateTime2 = dateTime2.Date;
                str1 = dateTime2.ToString("yyyyMMdd") + num.ToString();
            }
            else
            {
                dateTime1 = DateTime.Now;
                dateTime1 = dateTime1.Date;
                str1 = dateTime1.ToString("yyyyMMdd") + "1";
            }
            List<Violation> models = new List<Violation>();
            ViolationAPIModel model = new ViolationAPIModel();
            model.Probability = violation.AvgProp;
            model.Code = str1;
            model.Date = violation.time;
            model.AllClassessProbability = "";
            model.Category = category;
            if (violation.image != null && violation.image.Count > 0)
            {
                int lmIndex = 0;
                foreach (string strm in violation.image)
                {
                    lmIndex++;
                    string result2 = violationApiController._violationService.SaveImagePPE(strm, CommanData.ViolationFolder,lmIndex,name,violation.camera_name).Result;
                    //                    string result2 = violationApiController._violationService.SaveImage(strm, CommanData.ViolationFolder).Result;
                    model.image = result2;
                    Violation violation1 = violationApiController._violationService.ConvertFromViolationAPIToViolation(model, violation.violType, violation.AvgProp, violation.camera_name, 0.0, false, false);
                    if (violation1 != null)
                    {
                        violation1.MailSent = 1;
                        models.Add(violation1);
                    }
                }
            }
            else
            {
                model.image = "test.jpg";
                Violation violation2 = violationApiController._violationService.ConvertFromViolationAPIToViolation(model, violation.violType, violation.AvgProp, violation.camera_name, 0.0, false, false);
                if (violation2 != null)
                {
                    violation2.MailSent = 1;
                    models.Add(violation2);
                }
            }
            List<ViolationModel> addedViolationModels = violationApiController._violationService.CreateViolation(models).Result;
            if (addedViolationModels == null)
                return (ActionResult)violationApiController.BadRequest((object)new
                {
                    Message = "Failed Process,Invalid adding violation Data",
                    Data = false
                });
            ViolationModel violationModel = addedViolationModels.FirstOrDefault<ViolationModel>();
            string[] strArray = new string[7]
            {
        name,
        " Violation for ",
        truck.TruckName,
        " at ",
        null,
        null,
        null
            };
            dateTime1 = violation.time;
            strArray[4] = dateTime1.ToString();
            strArray[5] = "&&&  , Average Probability = ";
            strArray[6] = Math.Round(violation.AvgProp, 2, MidpointRounding.AwayFromZero).ToString();
            string str2 = string.Concat(strArray);
            ViolationNotificationModel result3 = violationApiController._violationNotificationService.CreateViolationNotification(str2, violationModel.Id, "camera").Result;
            if (result3 != null)
            {
                bool result4 = violationApiController._violationNotificationService.HandleViolationNotificationToRole(str2, result3.Id, "Supervisor", "Security").Result;
                bool result5 = violationApiController._violationNotificationService.PushRealTimeViolationNotificationToRole(result3, "Supervisor", "Security", violationModel).Result;
            }
            List<string> list = ((IEnumerable<string>)truck.MailList.Split(";")).ToList<string>();
            if(violation.AvgProp >= 0.7) //(list != null && list.Count > 0 && violation.AvgProp >= 0.7)
            {
                string body = violationApiController._outlookSenderService.PrapareMailBody(violationModel, str2);
                List<string> toMails = new List<string>()
        {
          "mahmoud.saleh@ext.cemex.com",
         // "eman.rasmy @cemex.com",
          "amralaaeldin.saleh@ext.cemex.com"
         // "lamia.mousa@ext.cemex.com"
        };
               // await violationApiController._outlookSenderService.SendEmail(body, toMails, " PPEs Violation Alert | " + name);
            }
            return (ActionResult)violationApiController.Ok((object)new
            {
                Message = "Successful process",
                Data = addedViolationModels
            });
        }

        [HttpPost("AddTruckStatus")]
        public async Task<ActionResult> AddTruckStatus(
          List<TruckRunningTrackingAPIModel> truckRunningTrackingAPIModels)
        {
            ViolationAPIController violationApiController = this;
            bool flag = false;
            if (truckRunningTrackingAPIModels == null)
                return (ActionResult)violationApiController.BadRequest((object)new
                {
                    Message = "can not accept empty object",
                    Data = false
                });
            if (truckRunningTrackingAPIModels.Count > 0)
            {
                if (violationApiController._truckService.GetTruck(truckRunningTrackingAPIModels[0].TruckID) != null)
                {
                    foreach (TruckRunningTrackingAPIModel trackingApiModel in truckRunningTrackingAPIModels)
                        flag = violationApiController._objectMappingService.ValidateObjectProperities((object)trackingApiModel) && violationApiController._truckRunningTrackingService.CreateTruckRunningTracking(trackingApiModel).Result;
                }
                else
                    flag = false;
            }
            else
                flag = false;
            return !flag ? (ActionResult)violationApiController.BadRequest((object)new
            {
                Message = "Failed Process",
                Data = false
            }) : (ActionResult)violationApiController.Ok((object)new
            {
                Message = "Successful process",
                Data = true
            });
        }

        [HttpGet("test")]
        public ActionResult Test()
        {
            return (ActionResult)this.Ok((object)new
            {
                Message = "Done",
                Data = "heellllllllllllllllllllllllllllllo"
            });
        }

        [HttpPost("AddTruckDetails")]
        public async Task<ActionResult> AddTruckDetails(TruckDetailsApiModel truckApiModel)
        {
            //ViolationAPIController violationApiController = this;
            bool flag = false;
            if (truckApiModel == null)
                return (ActionResult)BadRequest((object)new
                {
                    Message = "can not accept empty object",
                    Data = false
                });
           
                if (_truckService.GetTruck(truckApiModel.TruckId) != null)
                {
                        flag = _truckDetailsService.CreateTruckDetails(truckApiModel).Result;
                }
                else
                    flag = false;
            
            
            return !flag ? BadRequest(new
            {
                Message = "Failed Process",
                Data = false
            }) : Ok(new
            {
                Message = "Successful process",
                Data = true
            });
        }


        [HttpPost("DriverBehaviour")]
        public async Task<ActionResult> DriverBehaviour(DriverModel driverModel)
        {

            if(driverModel == null)
                return (ActionResult)BadRequest((object)new
                {
                    Message = "Error , Driver not exist",
                    Data = false
                });

            var _driverBehaviour = await _violationService.GetDriverBehaviour(driverModel.DriverNumber);
            return (ActionResult)Ok((object)new
            {
                flag = true,
                Message = "Error , Driver not exist",
                Data = _driverBehaviour
            });
        }

    }
}
