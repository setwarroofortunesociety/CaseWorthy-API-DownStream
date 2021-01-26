using System;
using System.Collections.Generic;
using System.Text;

namespace CW.ClientLibrary.Utility
{
    internal static class IntervalHelper
    {

        public static DateTime CalculateMinuteInterval(DateTime dateValue,int minuteValue)
        {
            return dateValue.AddMinutes(minuteValue);
        }

        /// <summary>
        /// Validate the interval 
        /// base on an interval
        /// </summary>
        /// <returns></returns>
        public static DateTime ValidateIntervalStartDate(DateTime dateTime)
        {
            // compare start date time to current date time 
            int isCurrentdate = DateTime.Compare(dateTime, DateTime.UtcNow);

            // checking 
            //date is later then current date or equal current date
            if (isCurrentdate >= 0)
            {
                //start date should not be greater or equal to end date
                dateTime = (DateTime.UtcNow).AddSeconds(-1);
            }
            else
            {

                //add a seconds to start date
                dateTime = dateTime.AddSeconds(1);
            }

            return dateTime;
        }

    }
}
