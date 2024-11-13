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
            Calling = 2,
            CallingLeft = 3,
            Drinking = 4,
            Distracted = 5,
            Camera_Cant_Read_ID = 6,
            Camera_Cant_Open_ID = 8,
            Camera_Coverd_ID = 9,
            Sleeping = 7,
            NoViolation = 10,
            NoHardhat = 11, 
            NoVest = 12, 
            NoGloves = 13, 
            NoMask = 14, 
            NoGlasses = 15, 
            Fall_Down = 16, 
            Forklift = 17,
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
