using AutoMapper;
using CW.ClientAPI.Models;
using CW.ClientAPI.Services.General;
using CW.ClientAPI.Services.MsgContent;
using CW.ClientAPI.Utility;
using CW.ClientLibrary.Data;
using CW.ClientLibrary.Models;
using CW.ClientLibrary.Services.MessageTracker;
using CW.ClientLibrary.Utility;
using CW.Library.Controllers;
using CW.Library.Models;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace CW.ClientAPI.Controllers
{
    [Route("")]
    [Route("api/Tracker")]
    [ApiController]
    public class TrackerController : Controller
    {

        private readonly ITracker _tracker;
        private readonly IInterval _interval;
        private readonly EntityContent _entityContent;
        private readonly EntityTrackerContent _entityTrackerContent;
        private readonly ClientImageContent _clientimageContent;
        private readonly IMapper _mapper;
        //private readonly IHttpClientFactory _factory;
        private readonly ILogger<Task> _logger;
        private readonly IEmailService _emailService;
        private readonly ICaseWorthyService _caseworthyService;
        private readonly IRecurringJobManager _recurringJobManager;
        private HttpClient ClientFactory { get; }
        private CaseWorthySettingsModel CaseWorthyModel { get; }
        private String message = string.Empty;

        private Int64 intervalID;
        public TrackerController(IInterval interval,
                                 ITracker tracker,
                                 EntityContent entityContent,
                                 EntityTrackerContent entityTrackerContent,
                                 ClientImageContent clientimageContent,
                                 IMapper mapper,
                                 ILogger<Task> logger,
                                 IEmailService emailService,
                                 ICaseWorthyService caseworthyService,
                                 //IHttpClientFactory factory,
                                 IConfiguration configuration,
                                 IRecurringJobManager recurringJobManager)
        {

            _interval = interval ?? throw new ArgumentNullException(nameof(interval));

            _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));

            _entityContent = entityContent ?? throw new ArgumentNullException(nameof(entityContent));

            _entityTrackerContent = entityTrackerContent ?? throw new ArgumentNullException(nameof(entityTrackerContent));

            _clientimageContent = clientimageContent ?? throw new ArgumentNullException(nameof(clientimageContent));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); ;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

            _caseworthyService = caseworthyService ?? throw new ArgumentNullException(nameof(caseworthyService));

            _recurringJobManager = recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager));

            ClientFactory = _caseworthyService.GetHttpClient();
            CaseWorthyModel = _caseworthyService.GetCaseWorthySettings();

        }


        // <summary>
        ///Get a list of entities that has been modified from Caseworthy
        /// </summary>
        /// <returns>list of entitites i.e. client, enrollment photo etc</returns>
        [Route("api/tracker/caseworthytracker")]
        [HttpGet()]
        // public async Task<IActionResult> CaseworthyTracker()
        public async Task<ActionResult<string>> CaseworthyTracker()
        {


            try
            {
                string hasProcessed = "Fail to Process";

                _logger.LogInformation("FETCHING tracker data from Caseworthy....");

                //get start/end date time interval
                var pendingIntervals = _interval.GetIntervalsToProcess().ToList();

                if (pendingIntervals == null)
                {
                    _logger.LogWarning("{0} Interal values are null.", nameof(CaseworthyTracker));
                    return NotFound(hasProcessed);
                }

                //for each interval process
                for (int i = 0; i < pendingIntervals.Count; i++)
                {
                    var pendingItem = pendingIntervals[i];

                    intervalID = pendingItem.IntervalID;


                    //Step 1 - Get entity tracker url with parameters          
                    string url = EntityTrackerProcessor.GetEntityTrackerUrl(CaseWorthyModel.BaseAddress, pendingItem.StartDateTime, pendingItem.EndDateTime, pendingItem.EntityType);
                    if (string.IsNullOrEmpty(url))
                    {

                        //error message
                        String message = String.Format("{0}.  Could not retrieve Entity Tracker content. Url not found. Interval ID: {0} . ",hasProcessed, intervalID);

                        //email warning  
                        _emailService.Send("Client API - Caseworthy Tracker Failed", message);

                        //log warning
                        _logger.LogWarning("{0} - Tracker status was updated with errors. Interval Id: {0} Error Message: {1}", nameof(CaseworthyTracker), intervalID, message);

                        _interval.UpdateStatus(intervalID, (int)Enums.Status.Error);

                        return NotFound(hasProcessed);
                    }


                    //Step 2 get the header key for url;
                    var EntityHeaderKey = EntityTrackerProcessor.AuthorizationHeaderKey(CaseWorthyModel.AccessKey, CaseWorthyModel.SecretKey, url, "GET");
                    if ((string.IsNullOrEmpty(EntityHeaderKey)) || (EntityHeaderKey.Contains("Invalid")))
                    {
                        //error message
                        String message = String.Format("{0}.  Could not retrieve authorization key for Entity Tracker url header.  {1}. IntervalID: {2} Url: {3}", hasProcessed,EntityHeaderKey, intervalID, url);

                        //email warning  
                        _emailService.Send("Client API - Caseworthy Tracker Failed", message);

                        //log warning
                        _logger.LogWarning("{0} - Tracker status was updated with errors. Interval Id: {0} Error Message: {1}", nameof(CaseworthyTracker), intervalID, message);

                        _interval.UpdateStatus(intervalID, (int)Enums.Status.Error);

                        return NotFound(hasProcessed);

                    }


                    //Step 3 request the entity tracker data content
                    _logger.LogInformation("REQUESTING data for Interval Id: {0} \n Url: {1}", intervalID, url);

                    // add header to http client
                    //remove authorization before adding because of error
                    this.ClientFactory.DefaultRequestHeaders.Remove("Authorization");
                    this.ClientFactory.DefaultRequestHeaders.Add("Authorization", EntityHeaderKey);



                    bool hasSaved = await _entityTrackerContent.AddEntityTrackerContentAsync(ClientFactory, url, intervalID).ConfigureAwait(false);

                    if (hasSaved == true)
                    {
                        hasProcessed = "Processed Successfully";
                    }
                    else
                    {
                        string message = String.Format(CultureInfo.CurrentCulture, "{0}. Interval Id {1}", hasProcessed, intervalID);
                        _emailService.Send("Client API - Caseworthy Tracker Failed {0}", message);
                    }


                }// loop end


                _logger.LogInformation($"Successfully Retrieved data for Interval Id: {0}", intervalID);
                return hasProcessed;

            }
            catch (Exception ex)
            {

                string message = String.Format(CultureInfo.CurrentCulture, "{0} \n\n Exception:\n {1}.\n\n StackTrace:\n{2}", nameof(CaseworthyTracker), ex.Message, ex.StackTrace);
                _emailService.Send("Client API - Caseworthy Tracker Failed", message);

                _logger.LogError($"{0} \n\n Error:\n {1}", nameof(CaseworthyTracker), ex);
                throw new ArgumentException("Error message {1} \n inner exception {2}", ex.Message, ex.InnerException);
            }

        }

        // <summary>
        ///Get individual entity data from Caseworthy
        /// </summary>
        /// <returns>entity</returns>
        [Route("api/Tracker/ProcessContent")]
        [HttpGet()]
        public async Task<ActionResult<string>> ProcessTrackerContent()
        {
            string hasProcessed = "Fail to Process Content";

            try
            {
                //set the cw action type for processing
                var actionType = new SearchParameters { ActionID = (int)Enums.Action.PickedUp };

                //get a list of the tracker records that have been picked up
                var pickedUpRec = _tracker.GetByActionType(actionType).ToList();

                if (pickedUpRec == null)
                {
                    //message
                    message = String.Format("{0}. No records found", hasProcessed, pickedUpRec.Count);
                    //log  
                    _logger.LogInformation("{0} - Message: {1}", nameof(ProcessTrackerContent), message);

                    return NotFound(message);
                }


                bool isEmpty = pickedUpRec.Any();

                if (isEmpty == false)
                {
                    //message
                    message = String.Format("{0} records to process", pickedUpRec.Count);

                    //log  
                    _logger.LogInformation("{0} - Message: {1}", nameof(ProcessTrackerContent), message);

                    return Ok(message);

                }

                //for each interval process
                for (int i = 0; i < pickedUpRec.Count; i++)
                {
                    var itemPickedUp = pickedUpRec[i];

                    var msgType = (Enums.MSGType)Enum.Parse(typeof(Enums.MSGType), itemPickedUp.EntityType);

                    _logger.LogInformation("REQUESTING content for  MsgID: {0}, msgType: {1}, EntityId: {2}, SubEntity: {3}, SubEntity2: {4}", itemPickedUp.MSGID, msgType, itemPickedUp.EntityID, itemPickedUp.SubEntityID, itemPickedUp.SubEntityID2);


                    switch (msgType)
                    {
                        case Enums.MSGType.ClientPhoto:
                            bool hasImageProcessed = await _clientimageContent.AddClientImageContentAsync(itemPickedUp).ConfigureAwait(false);
                            if (hasImageProcessed == true)
                            {
                                hasProcessed = "Processed Successfully";
                            }
                            //else
                            //{
                            //    string message = String.Format(CultureInfo.CurrentCulture, "{0}. Interval Id {1}", hasProcessed, intervalID);
                            //    _emailService.Send("Client API - Caseworthy Tracker Failed {0}", message);
                            //}

                            //  hasProcessed = hasImageProcessed;
                            break;
                        default:
                            bool hasContentProcessed = await _entityContent.AddEntityContentAsync(itemPickedUp).ConfigureAwait(false);
                            if (hasContentProcessed == true)
                            {
                                hasProcessed = "Processed Successfully";
                            }
                            break;
                    }
                }// loop end

                _logger.LogError("Successfully Retrieved data. {0}", intervalID);
                return hasProcessed;
            }
            catch (Exception ex)
            {
                string message = String.Format(CultureInfo.CurrentCulture, "{0} \n\n Exception:\n {1}.\n\n Inner Exception: {2}. \n\nStackTrace:\n{3}", nameof(ProcessTrackerContent), ex.Message, ex.InnerException, ex.StackTrace);

                _emailService.Send("Client API - Process Tracker Content Failed", message);

                _logger.LogError("{0} \n\n Exception:\n {1}.\n StackTrace:\n{2}", nameof(ProcessTrackerContent), ex.InnerException, ex.StackTrace);

                throw new ArgumentException("Error message {1} \n inner exception {2}", ex.Message, ex.InnerException);

            }

        }

        #region Hangfire Job

        [Route("api/Tracker/Hangfire/AddUpdateRecurringJob")]
        [HttpGet()]
        public ActionResult<bool> AddUpdateJob_CaseworthyTracker([FromQuery] HelperParameters jobParameters)
        {
            bool hasStarted = false;

            try
            {
                if ((jobParameters.JobName == null) && (jobParameters.Expression == null) && (jobParameters.MethodType == null))
                {
                    _logger.LogError("Invalid Job Name {0}, Invalid cron expression {1}, Invalid Method Name Type {3}", jobParameters.JobName, jobParameters.Expression, jobParameters.MethodType);

                    return hasStarted;
                }

                switch (jobParameters.MethodType)
                {
                    case "CaseworthyTracker":

                        _recurringJobManager.AddOrUpdate<TrackerController>(jobParameters.JobName, x => x.CaseworthyTracker(), jobParameters.Expression);

                        hasStarted = true;

                        break;

                    case "TrackerContent":

                        _recurringJobManager.AddOrUpdate<TrackerController>(jobParameters.JobName, x => x.ProcessTrackerContent(), jobParameters.Expression);

                        hasStarted = true;

                        break;
                }

                _logger.LogInformation("Successfully add recurring job Id: {0}", jobParameters.JobName);

                return hasStarted;

            }
            catch (Exception ex)
            {
                _logger.LogError("Internal server error {0}", ex.Message);
                throw new ArgumentException("Error message {1} inner exception {2}", ex.Message, ex.InnerException);
            }

        }

        [Route("api/Tracker/Hangfire/RemoveJob/{jobName}")]
        [HttpGet()]
        public bool RemoveJob(string jobName)
        {
            bool hasStopped = false;

            try
            {
                if (jobName == null)
                {
                    _logger.LogError("Invalid Job Name {0}, Invalid cron expression {1}", jobName);

                    return hasStopped;
                }

                _recurringJobManager.RemoveIfExists(jobName);

                hasStopped = true;

                _logger.LogInformation("Successfully remove recurring job Id: {0}", jobName);

                return hasStopped;

            }
            catch (Exception ex)
            {
                _logger.LogError("Internal server error {0}", ex.Message);
                throw new ArgumentException("Error message {1} inner exception {2}", ex.Message, ex.InnerException);
            }

        }

        #endregion

    }

}