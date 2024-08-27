using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Models.Models.MasterModels;
using WebDriverViolation.Services.Models.APIModels;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Models.Utiltise
{
    public static class AutoMapperExtension
    {
        public static void ConfigAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AssembleType));
        }

        public class AutoMapperProfiles : Profile
        {
            public AutoMapperProfiles()
            {
                CreateMap<Violation, ViolationModel>();
                CreateMap<ViolationModel, Violation>();


                CreateMap<ViolationTypeModel, ViolationType>();
                CreateMap<ViolationType, ViolationTypeModel>();

                CreateMap<TruckModel, Truck>();
                CreateMap<Truck, TruckModel>();
                CreateMap<Employee, EmployeeModel>();
                CreateMap<EmployeeModel, Employee>();

                CreateMap<ViolationNotification, ViolationNotificationModel>();
                CreateMap<ViolationNotificationModel, ViolationNotification>();

                CreateMap<UserViolationNotification, UserViolationNotificationModel>();
                CreateMap<UserViolationNotificationModel, UserViolationNotification>();

                CreateMap<Employee, EmployeeModel>();
                CreateMap<EmployeeModel, Employee>();

                CreateMap<ConfirmationStatus, ConfirmationStatusModel>();
                CreateMap<ConfirmationStatusModel, ConfirmationStatus>();

                CreateMap<TruckRunningTracking, TruckRunningTrackingAPIModel>();
                CreateMap<TruckRunningTrackingAPIModel, TruckRunningTracking>();

            }
        }
    }
    public class AssembleType
    {
    }
}