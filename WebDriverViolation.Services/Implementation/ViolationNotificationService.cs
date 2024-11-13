using AutoMapper;
using Data.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Models.Models.MasterModels;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models;
using WebDriverViolation.Services.Models.hub;
using WebDriverViolation.Services.Models.MasterModels;
using WebDriverViolation.Services.Models.SpecificModel;

namespace WebDriverViolation.Services.Implementation
{
    public class ViolationNotificationService: IViolationNotificationService
    {
        private readonly IRepository<ViolationNotification, long> _repository;
        private readonly IMapper _mapper;
        private readonly IUserViolationNotificationService _userViolationNotificationService;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IViolationService _violationService;
        private readonly IViolationTypeService _violationTypeService;

        public ViolationNotificationService(IRepository<ViolationNotification, long> repository,
            IMapper mapper,
            IUserViolationNotificationService userViolationNotificationService,
            UserManager<AspNetUser> userManager,
            IUserConnectionManager userConnectionManager,
            IHubContext<NotificationHub> hub,
            IViolationService violationService,
            IViolationTypeService violationTypeService)
        {
            _repository = repository;
            _mapper = mapper;
            _userViolationNotificationService = userViolationNotificationService;
            _userManager = userManager;
            _userConnectionManager = userConnectionManager;
            _hub = hub;
            _violationService = violationService;
            _violationTypeService = violationTypeService;
        }


        public async Task<ViolationNotificationModel> CreateViolationNotification(string message, long violationId, string? category)
        {
            try
            {
                ViolationNotificationModel violationNotificationModel = new ViolationNotificationModel();
                ViolationNotification notification = new ViolationNotification();
                notification.Message = message;
                notification.CreatedDate = DateTime.Now;
                notification.UpdatedDate = DateTime.Now;
                notification.IsVisible = true;
                notification.IsDelted = false;
                notification.ViolationID = violationId;
                notification.Category = category;
                ViolationNotification addedNotification = _repository.Add(notification);
                if(addedNotification != null)
                {
                    violationNotificationModel = _mapper.Map<ViolationNotificationModel>(addedNotification);
                    return violationNotificationModel;
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

        public List<ViolationNotificationModel> GetAllUnseenViolationNotificationsForUser(string userId)
        {
            throw new NotImplementedException();
        }

        public ViolationNotificationModel GetViolationNotification(long id)
        {
            try
            {
                var notification = _repository.Find(n => n.IsVisible == true && n.Id == id, false, n => n.Violation).FirstOrDefault();
                ViolationNotificationModel notificationModel = _mapper.Map<ViolationNotificationModel>(notification);
                return notificationModel;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<ViolationNotificationModel> GetUnseenViolationNotifications()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateViolationNotification(ViolationNotificationModel model)
        {
            throw new NotImplementedException();
        }


        public async Task<bool> PushRealTimeViolationNotificationToRole(ViolationNotificationModel notification, string roleName, string companyName, ViolationModel violationModel)
        {
            try
            {
                RealTimeDashBoardUpdateModel dashboardModel = new RealTimeDashBoardUpdateModel();
                dashboardModel.PendingViolationCount = _violationService.GetPendingViolationCount().Result;
                dashboardModel.RealTimeViolationModel = violationModel;
                dashboardModel.ViolationNotificationModel = notification; 
                dashboardModel.ViolationNotificationModel.CreatedDateText = notification.CreatedDate.ToString("yyyy-MM-dd");
                dashboardModel.ViolationNotificationModel.CreatedTimeText = notification.CreatedDate.ToString("hh-mm-ss");
                dashboardModel.ViolationTypeModels = _violationTypeService.GetAllViolationTypes().Result;
                var dashBoadrModelSeralized = JsonSerializer.Serialize(dashboardModel);
                var aspNetUsers = _userManager.GetUsersInRoleAsync(roleName).Result.Where(u => u.Company == companyName).ToList();
                var aspNetUsersAdmin = _userManager.GetUsersInRoleAsync("Admin").Result.Where(u => u.Company == companyName).ToList();
                List<AspNetUser> users = new List<AspNetUser>();
                if (aspNetUsers.Count > 0)
                {
                    users.AddRange(aspNetUsers);
                }
                if (aspNetUsersAdmin.Count > 0)
                {
                    users.AddRange(aspNetUsersAdmin);
                }
                if (users.Count > 0)
                {
                    foreach (var user in users)
                    {
                        var connections = _userConnectionManager.GetUserConnections(user.Id);
                        if (connections != null && connections.Count > 0)
                        {
                            foreach (var connectionId in connections)
                            {
                                await _hub.Clients.Client(connectionId).SendAsync("sendToUser",             dashBoadrModelSeralized);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> HandleViolationNotificationToRole(string notificationMessage, long ViolationnotificationId, string roleName, string companyName)
        {
            int NotificationSendToCount = 0;
            bool result = false;
            try
            {
                    NotificationSendToCount = _userViolationNotificationService.AssignUserViolationNotificationForRoleUsers(ViolationnotificationId, roleName, companyName).Result;
                    int supervisorUsers = _userManager.GetUsersInRoleAsync(roleName).Result.Where(u=>u.Company =="Security").Count();
                int adminUsers = _userManager.GetUsersInRoleAsync("Admin").Result.Where(u => u.Company == "Security").Count();
                int users = supervisorUsers + adminUsers;
                if (users == NotificationSendToCount)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                }
                return result;
            }
            catch (Exception e)
            {
               
                return false;
            }
        }

        //public async Task<string> HandleViolationNotificationToRoleSecurity(string notificationMessage, int notificationTypeId, string roleName, long tripId, long jobsiteId, string companyName)
        //{
        //    int NotificationSendToCount = 0;
        //    string message = "";
        //    try
        //    {
        //        ViolationNotification addedNotification = CreateViolationNotification(notificationMessage, notificationTypeId, tripId, jobsiteId);
        //        if (addedNotification != null)
        //        {
        //            NotificationSendToCount = _userViolationNotificationService.AssignUserNotificationForRoleUsers(addedNotification.Id, roleName, companyName).Result;
        //            var users = _userManager.GetUsersInRoleAsync(roleName);
        //            int count = users.Result.Where(u => u.Company == companyName).Count();
        //            if (count == NotificationSendToCount)
        //            {
        //                message = "Successful Process";
        //                ViolationNotificationModel notificationModel = GetViolationNotification(addedNotification.Id);
        //                bool notifyResult = PushViolationNotificationToRole(notificationModel, roleName, null, companyName).Result;
        //                if (notifyResult == false)
        //                {
        //                    message = "Failed Process, can not push notification to supervisor";
        //                }
        //            }
        //            else
        //            {
        //                message = "Failed Process, There is an error. Message sent to " + NotificationSendToCount + " from " + count;
        //            }
        //        }
        //        else
        //        {
        //            message = "Failed Process, can not create ViolationNotification";
        //        }
        //        return message;
        //    }
        //    catch (Exception e)
        //    {
        //        message = "Failed process, contact your system support";
        //        return message;
        //    }
        //}

        // }
    }

}
