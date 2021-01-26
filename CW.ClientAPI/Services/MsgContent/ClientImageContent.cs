using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using CW.ClientLibrary.Data;
using CW.ClientLibrary.Models;
using CW.ClientAPI.Services.General;
using CW.ClientLibrary.Services.MessageTracker;
using CW.ClientLibrary.Utility;
using CW.Library.Controllers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CW.ClientAPI.Models;

namespace CW.ClientAPI.Services.MsgContent
{
    public class ClientImageContent
    {
        private readonly IClientImage _imageContent;
        private readonly ICaseWorthyService _caseWorthyService;
        private readonly IMapper _mapper;
        private readonly ILogger<ClientImageContent> _logger;
        private readonly ITracker _tracker;

        private bool hasSaved = false;
        private string msgType = string.Empty;
        private String message = string.Empty;
        private CaseWorthySettingsModel caseWorthyModel { get; }
        private HttpClient clientFactory { get; }

        public ClientImageContent(IClientImage imageContent,
                      ICaseWorthyService caseWorthyService,
                      ITracker tracker,
                      IMapper mapper,
                      ILogger<ClientImageContent> logger)
        {
            _imageContent = imageContent;
            _tracker = tracker;
            _caseWorthyService = caseWorthyService;
            _mapper = mapper;
            _logger = logger;

            
            caseWorthyModel = _caseWorthyService.GetCaseWorthySettings();
            clientFactory = _caseWorthyService.GetHttpClient();

        }

        public async Task<bool> AddClientImageContentAsync(MSG_Tracker trackerPhotoRec)
        {


            // assign Entity Type
            msgType = trackerPhotoRec.EntityType;
            String method = "GET"; //needed for authorization header


            _logger.LogInformation("{0} - PROCESSING data for Msg ID: {1} ....", nameof(AddClientImageContentAsync), trackerPhotoRec.MSGID);


            //Step 1 - Get entity client image url with parameters          
            string url = EntityTrackerProcessor.GetContentUrl(caseWorthyModel.BaseAddress, msgType,  (int)trackerPhotoRec.SubEntityID);
            if (string.IsNullOrEmpty(url))
            {

                //error message
                 message = String.Format("Fail to retrieve {0} content. Url not found.",msgType);

                _logger.LogWarning("{0} - Tracker status was updated with errors. MsgID: {1} Message: {2}", nameof(AddClientImageContentAsync), message);
                _tracker.UpdateActionStatus(trackerPhotoRec.MSGID, (int)Enums.Action.UnProcessed, message);

                 hasSaved = false;
            }



            //Step 2 get the header key for url;
            var contentHeaderKey = EntityTrackerProcessor.AuthorizationHeaderKey(caseWorthyModel.AccessKey,  caseWorthyModel.SecretKey, url, method);
            if ((string.IsNullOrEmpty(contentHeaderKey)) || (contentHeaderKey.Contains("Invalid")))
            {
                //error message
                 message = String.Format("Fail to retrieve authorization key for {0} header.  {1}. Url: {2}", msgType, contentHeaderKey, url);

                _logger.LogWarning("{0} - Tracker status was updated with errors. MsgID: {1} Message: {2}", nameof(AddClientImageContentAsync), message);
                _tracker.UpdateActionStatus(trackerPhotoRec.MSGID, (int)Enums.Action.UnProcessed, message);

                hasSaved = false;


            }


            //Step 3 request the photo content
            _logger.LogInformation("REQUESTING photo content...");

            // add header to http client
            //remove authorization before adding because of error
            this.clientFactory.DefaultRequestHeaders.Remove("Authorization");
            this.clientFactory.DefaultRequestHeaders.Add("Authorization", contentHeaderKey);

            //restful call to get the client photo
            var photoContent = await EntityTrackerProcessor.GetEntityContent(clientFactory,url);


            //Null or empty
            if  ((String.IsNullOrWhiteSpace(photoContent)) || (photoContent.Contains("Error"))) 
            {
                //error message
                 message = String.Format("Fail to retrieve {0} content.  {1} ,Url: {2} ", msgType, photoContent, url);
                _logger.LogError("{0} - Tracker status was updated with errors. MsgID: {1} Message: {2}", nameof(AddClientImageContentAsync), trackerPhotoRec.MSGID, message);

                _tracker.UpdateActionStatus(trackerPhotoRec.MSGID, (int)Enums.Action.UnProcessed, message);

                hasSaved = false;

            }
            else
            {
                //Add to content table
                photoContent = photoContent.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });

                //Deserialize json string
                var jsonMessage = JsonConvert.DeserializeObject<RootObjectPhoto>(photoContent);

                if (jsonMessage.ClientPhoto.Count() == 0)
                {

                    //error message
                    message = String.Format("{0} content not found. \n Url: {1} ", msgType,url);
                   
                    _logger.LogWarning("{0} - Tracker status was updated. MsgID: {1} Message: {2}",nameof(AddClientImageContentAsync), trackerPhotoRec.MSGID, message);

                    _tracker.UpdateActionStatus(trackerPhotoRec.MSGID, (int)Enums.Action.UnProcessed, message);

                    hasSaved = true;

                }
                else
                {

                    var photoContentModel = jsonMessage.ClientPhoto[0];

                    //map to table
                    var clientPhotoToDb = _mapper.Map<ClientPhoto>(photoContentModel);


                    bool doesPhotoExist = _imageContent.ClientImageExists(clientPhotoToDb.PrintPhotoFileID);

                    if (doesPhotoExist == true)
                    {
                        _imageContent.UpdateClientImage(clientPhotoToDb);
                        //Message
                        message = String.Format("Successfully update {0} content.", msgType);
                    }
                    else
                    { 
                        _imageContent.AddClientImage(clientPhotoToDb);
                        //Message
                        message = String.Format("Successfully add {0} content.", msgType);
                    }

                    _logger.LogInformation("{0} - Tracker status was updated. MsgID: {1} Message: {2}", nameof(AddClientImageContentAsync),trackerPhotoRec.MSGID,message);

                    //update tracker action status
                    _tracker.UpdateActionStatus(trackerPhotoRec.MSGID, (int)Enums.Action.Processed, null);

                    hasSaved = true;

                };
            }

            return hasSaved;

        }
    }
}
