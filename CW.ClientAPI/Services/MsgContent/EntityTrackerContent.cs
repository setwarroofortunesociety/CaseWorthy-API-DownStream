using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using CW.ClientAPI.Models;
using CW.ClientAPI.Services.General;
using CW.ClientLibrary.Data;
using CW.ClientLibrary.Models;
using CW.ClientLibrary.Services.MessageTracker;
using CW.ClientLibrary.Utility;
using CW.Library.Controllers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CW.ClientAPI.Services.MsgContent
{
    public class EntityTrackerContent
    {
        private readonly ITracker _tracker;
        private readonly IInterval _interval;
        private readonly IMapper _mapper;
        private readonly ILogger<EntityTrackerContent> _logger;
        private readonly ICaseWorthyService _caseWorthyService;

        private bool hasSaved = false;
        private string message = string.Empty;
        private CaseWorthySettingsModel caseWorthyModel { get; }

        public EntityTrackerContent(
                      ITracker tracker,
                      IInterval interval,
                      ICaseWorthyService caseWorthyService,
                      IMapper mapper,
                      ILogger<EntityTrackerContent> logger)
        {

            _tracker = tracker;
            _interval = interval;
            _caseWorthyService = caseWorthyService;
            _mapper = mapper;
            _logger = logger;

            caseWorthyModel = _caseWorthyService.GetCaseWorthySettings();

        }

        public async Task<bool> AddEntityTrackerContentAsync(HttpClient clientFactory, string url, Int64 intervalID)
        {

            _logger.LogInformation("{0} - PROCESSING data for IntervalID: {1} ....", nameof(AddEntityTrackerContentAsync),intervalID);

            //restful call to get the Entity
            var trackerContent = await EntityTrackerProcessor.GetEntityContent(clientFactory, url);

             //Null or empty
            if ((String.IsNullOrWhiteSpace(trackerContent)) || (trackerContent.Contains("Error"))) 
            {
                //error message
                 message = String.Format("Fail to retrieve entity tracker content. {0} , Url: {1} ",trackerContent , url);
                _logger.LogError("{0} - Interval status was updated with errors. IntervalID: {1} Message: {2},",nameof(AddEntityTrackerContentAsync),intervalID, message);
                _interval.UpdateStatus(intervalID, (int)Enums.Status.Error);

                 hasSaved = false;

            }
            else
            {
                //Add to content table
                trackerContent = trackerContent.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });

                //Deserialize json string
                var jsonMessage = JsonConvert.DeserializeObject<RootObjectTracker>(trackerContent);

                if (jsonMessage.entityTracker.Count() == 0)
                {

                    //error message
                    message = String.Format("0 records to process. \n Url: {0} ", url);
                    _logger.LogInformation("{0} - Interval status was updated. IntervalID: {1} {2}", nameof(AddEntityTrackerContentAsync),intervalID, message);
                    _interval.UpdateStatus(intervalID, (int)Enums.Status.Completed_No_Data);

                    hasSaved = true;

                }
                else
                {
                    var trackerModel = jsonMessage.entityTracker;
                    var trackerDB = _mapper.Map<IEnumerable<MSG_Tracker>>(trackerModel);

                    hasSaved =  AddToFixTracker(trackerDB, intervalID);

                }


            };


            return hasSaved;
        }

        private bool AddToFixTracker(IEnumerable<MSG_Tracker> trackerData, Int64 intervalID)
        {
            try
            {

                var trackerProcessing = trackerData.ToList();

                //save Caseworthy tracker data to fix tracker data
                for (int i = 0; i < trackerProcessing.Count; i++)
                {
                    var itemTracker = trackerProcessing[i];

                    //map to table
                    var trackerToDb = _mapper.Map<MSG_Tracker>(itemTracker);

                    _tracker.AddTracker(trackerToDb, intervalID);

                }

                _interval.UpdateStatus(intervalID, (int)Enums.Status.Completed);

                _logger.LogInformation("{0} - Tracker was successfully updated. Interval Id:{1}", nameof(AddToFixTracker), intervalID);

                return this.hasSaved = true;

            }
            catch (Exception ex)
            {
                _interval.UpdateStatus(intervalID, (int)Enums.Status.Error);
                _logger.LogError("{0} - Error {1} stack trace {2}", nameof(AddToFixTracker), ex.InnerException, ex.StackTrace);

                return this.hasSaved= false;
            }
        }


    }
}
