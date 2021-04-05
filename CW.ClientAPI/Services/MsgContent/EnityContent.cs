using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
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
    public class EntityContent
    {
        private readonly IContent _content;
        private readonly IFortuneContent _fortuneContent;
        private readonly IMapper _mapper;
        private readonly ILogger<EntityContent> _logger;
        private readonly ICaseWorthyService _caseWorthyService;
        private readonly ITracker _tracker;
     

        //Initialized
        private bool hasSaved = false;
        private String message = string.Empty;
        private int entityID = 0;
        private string entityType = string.Empty;
        private string method = string.Empty;
        private CaseWorthySettingsModel caseWorthyModel { get; }
        private HttpClient clientFactory { get; }
     

        public EntityContent(IContent content,
                      IFortuneContent fortunecontent,
                      ITracker tracker,
                      ICaseWorthyService caseWorthyService,
                      IMapper mapper,
                      ILogger<EntityContent> logger)
        {
            _content = content;
            _fortuneContent = fortunecontent;
            _tracker = tracker;
            _caseWorthyService = caseWorthyService;
            _mapper = mapper;
            _logger = logger;

            caseWorthyModel = _caseWorthyService.GetCaseWorthySettings();
            clientFactory = _caseWorthyService.GetHttpClient();
        }

        public async Task<bool> AddEntityContentAsync(MSG_Tracker trackerRec)
        {
            
            try
            {
                //get the environment variable for CW
                var casworthyModel = _caseWorthyService.GetCaseWorthySettings(); //gets the access/secret key
                HttpClient clientFactory = _caseWorthyService.GetHttpClient(); //gets the httpclient with base address already set

                // assign values
               var msgType = (Enums.MSGType)Enum.Parse(typeof(Enums.MSGType), trackerRec.EntityType);
                method = "GET"; //needed for authorization header

                _logger.LogInformation("{0} msg Type {1} msg ID {2}", nameof(AddEntityContentAsync), msgType, trackerRec.MSGID);

                //Get the entity id for endpoint
                switch (msgType)
                {
                    case Enums.MSGType.Client:
                        //client
                        entityType = trackerRec.EntityType;
                        entityID = (int)trackerRec.EntityID;
                        break;

                    case Enums.MSGType.ClientEnrollment:
                        //client Enrollment
                        entityType = trackerRec.EntityType;
                        entityID = (int)trackerRec.SubEntityID;
                        break;

                    case Enums.MSGType.ClientTrackUsage:
                        //client TrackUsage
                        entityType = trackerRec.EntityType;
                        entityID = (int)trackerRec.SubEntityID2;
                        break;
                    case Enums.MSGType.User:
                        //User
                        entityType = trackerRec.EntityType;
                        entityID = (int)trackerRec.EntityID;
                        break;
                }


                //Step 1 - Get the url for the entity type
                string url = EntityTrackerProcessor.GetContentUrl(casworthyModel.BaseAddress, entityType, entityID);
                if (string.IsNullOrEmpty(url))
                {
                                       
                      //error message
                      message = String.Format("Fail to retrieve {0} content. Url not found.", msgType);

                    _logger.LogWarning("{0} - Tracker status was updated with errors. MsgID: {1} Message: {2}", nameof(AddEntityContentAsync), trackerRec.MSGID, message);
                    _tracker.UpdateActionStatus(trackerRec.MSGID, (int)Enums.Action.UnProcessed, message);

                    hasSaved = false;
                }
               

                 //Step 2 get the header key to send with url;
                var contentHeaderKey = EntityTrackerProcessor.AuthorizationHeaderKey(casworthyModel.AccessKey, casworthyModel.SecretKey, url, method);
                if  ((string.IsNullOrEmpty(contentHeaderKey)) || (contentHeaderKey.Contains("Invalid")))
                {                 

                        //error message
                         message = String.Format("Fail to retrieve {0} authorization key header. {1} Url: {2}",msgType,contentHeaderKey,url  );

                        _logger.LogInformation("{0} - Tracker status was updated with errors. MsgID: {1} Message: {2}", nameof(AddEntityContentAsync),trackerRec.MSGID, message);
                        _tracker.UpdateActionStatus(trackerRec.MSGID, (int)Enums.Action.UnProcessed, message);

                        hasSaved=false;
                    }


                //Step 3 get the entity content
                _logger.LogInformation("REQUESTING {0} content...", entityType);

               
                // add header to http client
                //remove authorization before adding because of error
                this.clientFactory.DefaultRequestHeaders.Remove("Authorization");
                this.clientFactory.DefaultRequestHeaders.Add("Authorization", contentHeaderKey);


                var clientEntityContent = await EntityTrackerProcessor.GetEntityContent(this.clientFactory, url);
                
                if ((string.IsNullOrEmpty(clientEntityContent))
                        || (clientEntityContent.Contains("Error")) 
                            || (clientEntityContent.Contains("{\"ClientDemographic\":[]}"))
                                || (clientEntityContent.Contains("{\"ClientEnrollment\":[]}"))
                                    || (clientEntityContent.Contains("{\"ClientTrackUsage\":[]}"))
                                        || (clientEntityContent.Contains("{\"UserDemographic\":[]}"))

                    )
                {
                   
                    //error message
                    message = String.Format("Fail to retrieve content for {0} . {1} \n Url {2}", msgType, clientEntityContent, url);

                    _logger.LogError("{0} - Tracker status was updated with errors. MsgID: {1} Message: {2}", nameof(AddEntityContentAsync),trackerRec.MSGID, message);
                    _tracker.UpdateActionStatus(trackerRec.MSGID, (int)Enums.Action.UnProcessed, message);

                    hasSaved = false;
                }
                else
                {


                  //Add to content table
                    var jsonMessage = clientEntityContent.Replace(@"\", "").Replace(@"""[", "[").Replace(@"]""", "]");


                    var contentModel = new MSG_ContentModel
                    {
                        MSGID = trackerRec.MSGID,
                        Content = jsonMessage
                    };

                    //map to table
                    var contentToDb = _mapper.Map<MSG_Content>(contentModel);
                    _content.AddContent(contentToDb);



                    //Message
                     message = String.Format("{0} successfully added.",entityType);
                    _logger.LogInformation("{0} - Tracker status was updated successfully for {1}. MsgID: {2}", nameof(AddEntityContentAsync),trackerRec.EntityType, trackerRec.MSGID);

                    //update tracker action status
                    _tracker.UpdateActionStatus(trackerRec.MSGID, (int)Enums.Action.Processed, null);

                    hasSaved = true;
                }

                return hasSaved;
            }
            catch (Exception ex)
            {
                string message = String.Format(CultureInfo.CurrentCulture, "{0} \n\n Exception:\n {1}.\n\n Inner Exception: {2}. \n\nStackTrace:\n{3}", nameof(AddEntityContentAsync), ex.Message, ex.InnerException, ex.StackTrace);

                _logger.LogError("{0} \n\n Exception:\n {1}.\n StackTrace:\n{2}", nameof(EntityContent), ex.InnerException, ex.StackTrace);
                
                return hasSaved = false;

            }
        }

        public  bool  AddFortuneEntityContent(MSG_Tracker trackerRec)
        {

            try
            {
            
                // assign values
                var msgType = (Enums.MSGType)Enum.Parse(typeof(Enums.MSGType), trackerRec.EntityType);
                string OrgAcctEntityContent =string.Empty;

                //Step 1 get the entity content
                _logger.LogInformation("REQUESTING {0} content msg Type {1} msg ID {2}", nameof(AddFortuneEntityContent), msgType, trackerRec.MSGID);

                switch (msgType)
                {
                    case Enums.MSGType.Account:
                        //Account
                        if (trackerRec.EntityID.HasValue)
                        {
                            OrgAcctEntityContent = _fortuneContent.GetAccount((int)trackerRec.EntityID);
                        }
                        break;
                    case Enums.MSGType.Organization:
                        //Organization
                        if ((trackerRec.EntityID.HasValue))
                        {
                            OrgAcctEntityContent = _fortuneContent.GetOrganization((int)trackerRec.EntityID);
                        }
                        break;
                }

             
                if ((string.IsNullOrEmpty(OrgAcctEntityContent))
                        || (OrgAcctEntityContent.Contains("Error"))
                            || (OrgAcctEntityContent.Contains("{\"Account\":[]}"))
                               || (OrgAcctEntityContent.Contains("{\"Organization\":[]}"))
                    )
                      {

                          //error message
                          message = String.Format("Fail to retrieve content for {0} . {1}}", msgType, OrgAcctEntityContent);

                          _logger.LogError("{0} - Tracker status was updated with errors. MsgID: {1} Message: {2}", nameof(AddFortuneEntityContent), trackerRec.MSGID, message);
                          _tracker.UpdateActionStatus(trackerRec.MSGID, (int)Enums.Action.UnProcessed, message);

                          hasSaved = false;
                      }
                      else
                      {


                    //Add to content table
                    var jsonMessage = OrgAcctEntityContent.Replace(@"\", "").Replace(@"""[", "[").Replace(@"]""", "]");


                    var contentModel = new MSG_ContentModel
                    {
                        MSGID = trackerRec.MSGID,
                        Content = jsonMessage
                    };

                    //map to table
                    var contentToDb = _mapper.Map<MSG_Content>(contentModel);
                    _content.AddContent(contentToDb);



                    //Message
                    message = String.Format("{0} successfully added.", entityType);
                    _logger.LogInformation("{0} - Tracker status was updated successfully for {1}. MsgID: {2}", nameof(AddFortuneEntityContent), trackerRec.EntityType, trackerRec.MSGID);

                    //update tracker action status
                    _tracker.UpdateActionStatus(trackerRec.MSGID, (int)Enums.Action.Processed, null);

                    hasSaved = true;
                }

                return hasSaved;
            }
            catch (Exception ex)
            {
                string message = String.Format(CultureInfo.CurrentCulture, "{0} \n\n Exception:\n {1}.\n\n Inner Exception: {2}. \n\nStackTrace:\n{3}", nameof(AddEntityContentAsync), ex.Message, ex.InnerException, ex.StackTrace);

                _logger.LogError("{0} \n\n Exception:\n {1}.\n StackTrace:\n{2}", nameof(EntityContent), ex.InnerException, ex.StackTrace);

                return hasSaved = false;

            }
        }
    }
}
