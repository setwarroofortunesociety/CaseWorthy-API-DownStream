using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CW.ClientLibrary.DbContexts;
using CW.ClientLibrary.Data;
using CW.ClientLibrary.Utility;
using Newtonsoft.Json;
using CW.ClientLibrary.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace CW.ClientLibrary.Services.MessageTracker
{
    public class Interval : IInterval
    {
        private readonly FixContext _context;


        public Interval(FixContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));

        }

        #region Interval

        /// <summary>
        ///Get all interval
        /// </summary>
        /// <returns>list intervals</returns>
        public IEnumerable<MSG_Interval> GetAll()
        {
            return _context.MSG_Intervals.ToList<MSG_Interval>();
        }

        /// <summary>
        /// Get an interval
        /// </summary>
        /// <parm>Interval ID</parm>
        /// <returns> An Interval</returns>
        public MSG_Interval GetInterval(Int64 intervalID)
        {
            if (intervalID == 0)
            {
                throw new ArgumentNullException(nameof(intervalID));
            }

            return _context.MSG_Intervals
                    .Where(i => i.IntervalID == intervalID).FirstOrDefault();
        }

        /// <summary>
        /// Get a list of Intervals by status
        /// </summary>
        /// <parm>Status</parm>
        /// <returns>list of intervals</returns>
        public IEnumerable<MSG_Interval> GetIntervalByStatusType(SearchParameters intervalParmaters)
        {
            if (intervalParmaters == null)
            {
                throw new ArgumentNullException(nameof(intervalParmaters));
            }

            var collection = _context.MSG_Intervals as IQueryable<MSG_Interval>;


            var parmStatus = intervalParmaters.Status;

            collection = collection
                   .Where(c => c.Status == parmStatus);


            return collection
                    .OrderByDescending(c => c.EndDateTime)
                    .ToList();

        }

        /// <summary>
        /// Create a new interval
        /// </summary>
        /// <returns> </returns>
        public void AddInterval(MSG_Interval interval)
        {
            try
            {
                if (interval == null)
                {
                    throw new ArgumentNullException(nameof(interval));
                }

                _context.MSG_Intervals.Add(interval);

                Save();
            }
            catch (Exception ex)
            {
                //_logger.LogError("Internal server error {0}", ex.Message);
                throw new ArgumentException("Error message {1} inner exception {2}", ex.Message, ex.InnerException);
            }

        }

        /// <summary>
        /// Update an interval status
        /// </summary>
        /// <parm> Interval ID and Status </parm>
        /// <returns>list of interval with a pending status</returns>
        public void UpdateStatus(Int64 intervalID, int intervalStatus)
        {
            if (intervalID == Int64.MinValue)
            {
                throw new ArgumentNullException(nameof(UpdateStatus));
            }

            //convert enum to a string
            Enums.Status intervalStatusString = (Enums.Status)intervalStatus;

            MSG_Interval col = _context.MSG_Intervals
                                    .Where(c => c.IntervalID == intervalID)
                                    .FirstOrDefault();

            col.Status = intervalStatus;
            col.Comments = intervalStatusString.ToString();
            _context.MSG_Intervals.Update(col);

            Save();

        }

        /// <summary>
        /// Add a pending interval with a calculated start/end date time
        /// base on an interval
        /// </summary>
        /// <returns></returns>
        private void AddPendingInterval()
        {


            //get max end date
            DateTime maxEnDateTime = GetMaxEndDate();

            //get interval start date
            DateTime startDateTime = IntervalHelper.ValidateIntervalStartDate(maxEnDateTime);

            //end date is defaulted to now
            DateTime endDateTime = DateTime.UtcNow;


            //if the start/end date are not null
            if (!((startDateTime == DateTime.MinValue) && (endDateTime == DateTime.MinValue)))
            {
                MSG_Interval col = new MSG_Interval
                {
                    StartDateTime = startDateTime,
                    EndDateTime = endDateTime,
                    Status = (int)Enums.Status.Pending,
                    Comments = Enums.Status.Pending.ToString()
                };

                AddInterval(col);
            }

        }

        /// <summary>
        /// Get or Create pending intervals
        /// </summary>
        /// <returns>list of interval with a pending status</returns>
        public IEnumerable<MSG_Interval> GetIntervalsToProcess()
        {

            //check if pending status exist
            bool hasPendingStatus = PendingStautsExists();

            if (hasPendingStatus == false)
            {
                //Add a new interval to be processing with a pending status
                AddPendingInterval();
            }

            var result = _context.MSG_Intervals
                      .Where(c => c.Status == (int)(Enums.Status.Pending))
                      .OrderBy(c => c.StartDateTime)
                      .ToList();

            return result;
        }

        /// <summary>
        /// Get the Max End Date Time of all intervals
        /// </summary>
        /// <returns>Max End Date Time</returns>
        private DateTime GetMaxEndDate()
        {

            try
            {
                //return exception if no max end date
                var result = _context.MSG_Intervals.AsQueryable()
                                    .Select(m => m.EndDateTime)
                                    .Max();

                return result;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error message {1} inner exception {2}", ex.Message, ex.InnerException);
            }

        }

        /// <summary>
        /// Check if a pending interval exists
        /// </summary>
        /// <returns>true, false</returns>
        private bool PendingStautsExists()
        {

            var result = _context.MSG_Intervals.Any(i => i.Status == (int)Enums.Status.Pending);

            return result;

        }

        #endregion

        public bool Save()
        {
            //Adding an entry
            var AddedEntities = _context.ChangeTracker.Entries()
                             .Where(E => E.State == EntityState.Added)
                             .ToList();

            AddedEntities.ForEach(E =>
            {
                E.Property("DateTimeStamp").CurrentValue = DateTime.UtcNow;
                E.Property("UserStamp").CurrentValue = "AdminCreate";
            });

            //Updating an entry
            var EditedEntities = _context.ChangeTracker.Entries()
                .Where(E => E.State == EntityState.Modified)
                .ToList();

            EditedEntities.ForEach(E =>
            {
                E.Property("DateTimeStamp").CurrentValue = DateTime.UtcNow;
                E.Property("UserStamp").CurrentValue = "AdminUpdate";
            });

            return (_context.SaveChanges() >= 0);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}

