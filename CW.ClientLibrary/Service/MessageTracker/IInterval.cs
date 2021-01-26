using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CW.ClientLibrary.Data;
using CW.ClientLibrary.Models;
using CW.ClientLibrary.Utility;

namespace CW.ClientLibrary.Services.MessageTracker
{
    public interface IInterval
    {
    
        IEnumerable<MSG_Interval> GetAll();

        MSG_Interval GetInterval(Int64 intervalID);

        IEnumerable<MSG_Interval> GetIntervalByStatusType(SearchParameters intervalParameters);

        IEnumerable<MSG_Interval> GetIntervalsToProcess();
        void AddInterval(MSG_Interval interval);

        void UpdateStatus(Int64 intervalId,int intervalStatus);

       // bool Save();
    }
}
