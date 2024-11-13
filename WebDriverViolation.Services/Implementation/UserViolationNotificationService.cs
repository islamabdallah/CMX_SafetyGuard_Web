using AutoMapper;
using Data.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.Design;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Models.Models.MasterModels;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Implementation
{
    public class UserViolationNotificationService : IUserViolationNotificationService
    {
        private readonly UserManager<AspNetUser> _userManager;

        private readonly IRepository<UserViolationNotification, long> _repository;
        private readonly IRepository<Violation, long> _vrepository;
        private readonly ILogger<UserViolationNotificationService> _logger;
        private readonly IMapper _mapper;

        public UserViolationNotificationService(IRepository<UserViolationNotification, long> repository,
            IRepository<Violation, long> vrepository,
            ILogger<UserViolationNotificationService> logger,
            IMapper mapper,
            UserManager<AspNetUser> userManager)
        {
            _repository = repository;
            _vrepository = vrepository;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<UserViolationNotificationModel> CreateUserViolationNotification(long notificationId, string userId)
        {
            try
            {
                UserViolationNotification userNotification = new UserViolationNotification();
                userNotification.ViolationNotificationId = notificationId;
                userNotification.userId = userId;
                userNotification.CreatedDate = DateTime.Now;
                userNotification.UpdatedDate = DateTime.Now;
                userNotification.IsDelted = false;
                userNotification.IsVisible = true;
                UserViolationNotification addedUserNotification = _repository.Add(userNotification);
                if(addedUserNotification != null)
                {
                    UserViolationNotificationModel userNotificationModel = _mapper.Map<UserViolationNotificationModel>(addedUserNotification);
                    return userNotificationModel;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public async Task<int> AssignUserViolationNotificationForRoleUsers(long NotificationId, string roleName, string CompanyId)
        {
            int addedUserNotificationCount = 0;
            try
            {
                var aspNetUsers = _userManager.GetUsersInRoleAsync(roleName).Result.Where(u=>u.Company == CompanyId).ToList();
                var aspNetUsersAdmin = _userManager.GetUsersInRoleAsync("Admin").Result.Where(u => u.Company == CompanyId).ToList();
                List<AspNetUser> users = new List<AspNetUser>();
                if (aspNetUsers.Count > 0)
                {
                    users.AddRange(aspNetUsers);
                }
                    users.AddRange(aspNetUsersAdmin);
                if (users.Count > 0)
                {
                    
                    foreach(var user in users)
                    {
                        UserViolationNotification userNotification= new UserViolationNotification();
                        userNotification.userId = user.Id;
                        userNotification.ViolationNotificationId = NotificationId;
                        userNotification.IsVisible = true;
                        var addedUserNotification = _repository.Add(userNotification);
                        if (addedUserNotification != null)
                        {
                            addedUserNotificationCount = addedUserNotificationCount + 1;
                        }
                    }
                }
                return addedUserNotificationCount;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public List<UserViolationNotificationModel> GetFiftyUserViolationNotification(string userId)
        {
            throw new NotImplementedException();
        }

        public List<UserViolationNotificationModel> GetUserViolationNotification(string userId)
        {
            try
            {
                List<UserViolationNotificationModel> userNotificationModels = new List<UserViolationNotificationModel>();
                List<UserViolationNotification> userNotifications = _repository.Find(UN => UN.userId == userId && UN.IsVisible == true && UN.Seen == false, false, UN => UN.ViolationNotification.Violation, UN => UN.ViolationNotification.Violation.ViolationType).ToList();
                if (userNotifications.Count > 0)
                {
                    userNotificationModels = _mapper.Map<List<UserViolationNotificationModel>>(userNotifications);
                    //userNotificationModels.ForEach(un => un.NotificationImageUrl = un.ViolationNotification.Violation.ImgUrl);
                    if(userNotificationModels.Count > 300)
                    {
                        userNotificationModels = userNotificationModels.TakeLast(300).OrderByDescending(n=>n.CreatedDate).ToList();
                    }
                }
                return userNotificationModels;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
        }

        public UserViolationNotificationModel GetUserViolationNotificationByUserIdAndNotificationId(string userId, long notificationId)
        {
            throw new NotImplementedException();
        }

        public int GetUserUnseenViolationNotificationCount(string userId)
        {
            try
            {
                var notifications = _repository.Find(un => un.userId == userId && un.Seen == false);
                if (notifications != null)
                {
                    return notifications.Count();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return 0;
        }

        public bool UpdateUserViolationNotification(UserViolationNotificationModel model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserUnseenViolationNotification(string userId)
        {
            try
            {
                var notifications = _repository.Find(un => un.userId == userId && un.Seen == false);
                if (notifications.Count() > 0)
                {
                    List<UserViolationNotification> userNotifications = notifications.ToList();
                    foreach (var notification in userNotifications)
                    {
                        notification.Seen = true;
                        _repository.Update(notification);
                    }
                }
                return Task<bool>.FromResult<bool>(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Task<bool>.FromResult<bool>(false);
            }
        }

    }
}
