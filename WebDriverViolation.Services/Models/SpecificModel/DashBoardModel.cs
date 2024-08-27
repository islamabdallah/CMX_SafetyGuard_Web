using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverViolation.Services.Models.MasterModels;

namespace WebDriverViolation.Services.Models.SpecificModel
{
    public class DashBoardModel
    {
        public Dictionary<string, long> ViolationTypeActualcounts { get; set; }

        public Dictionary<string, long> DashboardViolationStatuscounts { get; set; }

        public List<ViolationModel> LatestPendingViolationModels { get; set; }

        public long PendingViolationCount { get; set; }

        public long TotalActualViolationCount { get; set; }

        public long CurrentMonthActualViolationCount { get; set; }

        public long TotalTrucksCount { get; set; }

        public List<ViolationTypeModel> ViolationTypeModels { get; set; }

        public List<ViolationTypeCount> ViolationTypeCounts { get; set; }

        public List<UserViolationNotificationModel> UserViolationNotificationModels { get; set; }


    }

    public class ViolationTypeCount
    {
        public int ViolationTypeId { get; set; }

        public string ViolationTypeName { get; set; }

        public long Count { get; set; }

    }

    public class RealTimeDashBoardUpdateModel
    {
        public ViolationModel RealTimeViolationModel { get; set; }

        public long PendingViolationCount { get; set; }

        public ViolationNotificationModel ViolationNotificationModel { get; set; }

        public List<ViolationTypeModel> ViolationTypeModels { get; set; }



    }


}
