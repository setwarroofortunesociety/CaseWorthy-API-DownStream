using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CW.ClientLibrary.Data;
using CW.ClientLibrary.Models;
using CW.ClientLibrary.Utility;

namespace CW.ClientLibrary.Services.MessageTracker
{
    public interface ITracker
    {
    
        IEnumerable<MSG_Tracker> GetAll();
        IEnumerable<MSG_Tracker> GetByActionType(SearchParameters trackerParameters);

        void AddTracker(MSG_Tracker trackerObj, Int64 intervalID);

        void UpdateTracker(MSG_Tracker trackerObj);

        void UpdateActionStatus(Int64 msgID, int actionStatus, string errorMessage);

        // bool Save();
    }
}
