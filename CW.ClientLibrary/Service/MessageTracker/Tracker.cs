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
using Microsoft.EntityFrameworkCore;

namespace CW.ClientLibrary.Services.MessageTracker
{
    public class Tracker : ITracker
    {
        private readonly FixContext _context;

        public Tracker(FixContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }

        #region Tracker

        public IEnumerable<MSG_Tracker> GetAll()
        {
            return _context.MSG_Trackers.ToList<MSG_Tracker>();
        }

        public IEnumerable<MSG_Tracker> GetByActionType(SearchParameters trackerParameters)
        {
            if (trackerParameters == null)
            {
                throw new ArgumentNullException(nameof(trackerParameters));
            }

            var collection = _context.MSG_Trackers as IQueryable<MSG_Tracker>;

            if (!(trackerParameters.ActionID == null))
            {
                var srchQryActionId = trackerParameters.ActionID;
                collection = collection
                        .Where(c => c.CW_ActionID == srchQryActionId);
            }
            return collection.ToList();
        }

        public void UpdateTracker(MSG_Tracker trackerObj)
        {
            if (trackerObj == null)
            {
                throw new ArgumentNullException(nameof(trackerObj));
            }

            _context.MSG_Trackers.Update(trackerObj);

            Save();
        }

        public void UpdateActionStatus(Int64 msgID, int actionStatus,string errorMsg)
        {
            if (msgID == Int64.MinValue)
            {
                throw new ArgumentNullException(nameof(UpdateActionStatus));
            }

            MSG_Tracker col = _context.MSG_Trackers
                                  .Where(c => c.MSGID == msgID)
                                  .FirstOrDefault();
           
           
           col.CW_ActionID = actionStatus;
           col.CW_ErrorMessage = errorMsg;
           

           _context.MSG_Trackers.Update(col);

                Save();
        }
        public void AddTracker(MSG_Tracker trackerData, Int64 intervalID)
        {
            if (trackerData == null)
            {
                throw new ArgumentNullException(nameof(trackerData));
            }

            trackerData.CW_ActionID = (int)Utility.Enums.Action.PickedUp;
            trackerData.IntervalID = intervalID;

            _context.MSG_Trackers.Add(trackerData);

            Save();

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

       