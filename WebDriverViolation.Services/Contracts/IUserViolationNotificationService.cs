using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Take5.Services.Models.MasterModels;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Contracts
{
    public interface IUserViolationNotificationService
    {
        Task<UserViolationNotificationModel> CreateUserViolationNotification(long notificationId, string userId);

        List<UserViolationNotificationModel> GetUserViolationNotification(string userId);

        List<UserViolationNotificationModel> GetFiftyUserViolationNotification(string userId);
        bool UpdateUserViolationNotification(UserViolationNotificationModel model);

        int GetUserUnseenViolationNotificationCount(string userId);
        UserViolationNotificationModel GetUserViolationNotificationByUserIdAndNotificationId(string userId, long notificationId);
        Task<int> AssignUserViolationNotificationForRoleUsers(long NotificationId, string RoleName, string CompanyId);

        Task<bool> UpdateUserUnseenViolationNotification(string userId);

    }
}
