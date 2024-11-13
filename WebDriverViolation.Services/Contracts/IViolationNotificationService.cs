using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Models.Models.MasterModels;
using WebDriverViolation.Services.Models.MasterModels;
using WebDriverViolation.Services.Models.SpecificModel;

namespace WebDriverViolation.Services.Contracts
{
    public interface IViolationNotificationService
    {
        Task<ViolationNotificationModel> CreateViolationNotification(string message, long violationId, string? category);

        ViolationNotificationModel GetViolationNotification(long id);

        Task<bool> UpdateViolationNotification(ViolationNotificationModel model);

        List<ViolationNotificationModel> GetUnseenViolationNotifications();
        List<ViolationNotificationModel> GetAllUnseenViolationNotificationsForUser(string userId);
        Task<bool> PushRealTimeViolationNotificationToRole(ViolationNotificationModel notification, string roleName, string companyName, ViolationModel violationModel);
        Task<bool> HandleViolationNotificationToRole(string notificationMessage, long ViolationnotificationId, string roleName, string companyName);

        // Task<string> HandleViolationNotificationToRoleSecurity(string notificationMessage, int notificationTypeId, string roleName, long tripId, long jobsiteId, string companyName);

    }
}
