using System;
using System.Collections.Generic;
using System.Text;

namespace WebDriverViolation.Services.Models
{
    public class CommanData
    {
        public static string ViolationFolder = "images/ViolationImages/";
        public static string NotificationIconFolder = "https://localhost:44321/images/notificationIcons/";
        public static string UploadMainFolder = "wwwroot/";
        public static int numberofInstancesPerViolation = 3;
        public enum ViolationTypes
        {
            Texting = 1,
            CallingRight = 2,
            CallingLeft = 3,
            Drinking = 4,
            GettingBack = 5,
            Camera_Cant_Read_ID = 6,
            Camera_Cant_Open_ID = 8,
            Camera_Coverd_ID = 9,
            Sleeping = 7,
            NoViolation = 10,
        };

        public enum ConfirmationStatus
        {
            Pending = 1,
            Confirmed = 2,
        };

        public enum RejectStatus
        {
            Rejected = 10,
        };

    }
}
