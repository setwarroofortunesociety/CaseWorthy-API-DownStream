using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CW.ClientLibrary.Utility
{

    public class Enums { 


       
        public enum MSGType
        {
            Client,
            ClientPhoto,
            ClientEnrollment,
            ClientTrackUsage,
            Other,
        }
        public enum Action
        {
            PickedUp = 0,
            Processed = 1,
            UnProcessed = 2,
            Error = -1,
        }

        public enum Status
        {   
            Pending = 0,
            Completed = 1,
            Completed_No_Data = 2,
            Error = -1,
        }

    } 
}
