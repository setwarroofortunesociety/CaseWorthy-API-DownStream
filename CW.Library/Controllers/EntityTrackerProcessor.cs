using CW.Library.Models;
using CW.Library.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CW.Library.Controllers
{
    public static class EntityTrackerProcessor
    {
        private static string urlString;
        private static HttpResponseMessage response;
        private static string responseContent = null;
        private static string headerKey;
        private static IEnumerable<API_Fortune_EntityTracker_GET> Entitytracker { get; set; }


        // <summary>
        ///Get the authorization key
        /// </summary>
        /// <returns>Authorization key for url header</returns>
        public static string AuthorizationHeaderKey(string accessKey, string secretKey, string url, string method)
        {
            headerKey = AmxTokenGenerator.GenerateAuthorizationHeader(secretKey, accessKey, method, url, null);

            return headerKey;
        }

        // <summary>
        ///Get the entity tracker content from Caseworthy
        /// </summary>
        /// <returns>List of entity items that has been modified between start/end date time period (optional) entity type</returns>
        public static async Task<IEnumerable<API_Fortune_EntityTracker_GET>> GetEntityToProcess(HttpClient client, string url)
        {
            try
            {
                response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                if (response.IsSuccessStatusCode)
                {
                   var x = await response.Content.ReadAsStringAsync();

                
                    //Add to content table
                    x = x.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });

                    var jsonmsg = JsonConvert.DeserializeObject(x);


                    Entitytracker = Entitytracker; 

                }

                return Entitytracker;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error message {1} inner exception {2}", ex.Message, ex.InnerException);
            }

            finally
            {
                if (!(response == null))
                {
                    response.Dispose();
                }

            }
        }

            ///Get the individual entity  content from Caseworthy
        /// </summary>
        /// <returns> Individual entity items that has been modified </returns>
        public static async Task<string> GetEntityContent(HttpClient client, string url)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {


                //HttpResponseMessage response = await CWHelper.CWRestClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                if (response.IsSuccessStatusCode)
                {

                   
                    
                    responseContent = await response.Content.ReadAsStringAsync();

                }
                else
                {

                    // responseContent = String.Format("Fail to retrieve {0} content. StatusCode: {1} ReasonPhrase {2}: .", msgType,response.StatusCode,response.ReasonPhrase);
                    responseContent = null;

                }


                return responseContent;


            }
            finally
            {
                if (!(response == null))
                {
                    response.Dispose();
                }

            }
        }

        // <summary>
        ///Get the entity tracker url for Caseworthy
        /// </summary>
        /// <returns>Entity Tracker url with parameters</returns>
        public static string GetEntityTrackerUrl(string baseUrl, DateTime StartDate, DateTime EndDate, string entityType)
        {

            if ((string.IsNullOrWhiteSpace(baseUrl)) || (StartDate == DateTime.MinValue) || (EndDate == DateTime.MinValue))
            {
                urlString = String.Format("Invalid Entity Tracker Url");
            }

            if (string.IsNullOrWhiteSpace(entityType))
            {
                urlString = string.Format("{0}entitytracker?startDate={1}&endDate={2}", baseUrl, StartDate.ToString("yyyy-MM-dd'T'HH:mm:ss"), EndDate.ToString("yyyy-MM-dd'T'HH:mm:ss"));
            }
            else
            {
                urlString = string.Format("{0}entitytracker?startDate={1}&endDate={2}&entitytype={3}", baseUrl, StartDate.ToString("yyyy-MM-dd'T'HH:mm:ss"), EndDate.ToString("yyyy-MM-dd'T'HH:mm:ss"), entityType);

            }
             
             return urlString;

        }

        // <summary>
        ///Get the individual content for each entity from Caseworthy
        /// </summary>
        /// <returns>Individual entity content based on entity id and message type parameters</returns>
        public static string GetContentUrl(string baseUrl, string msgType, int entityId)
        {

            if ((string.IsNullOrWhiteSpace(baseUrl)) || (string.IsNullOrWhiteSpace(msgType)) || (entityId == 0))
            {
                urlString = String.Format("Invalid Content Url");
            }

            //Get the entity id for endpoint
            switch (msgType)
            {
                case "Client":
                    //client
                    urlString = string.Format("{0}clients/{1}", baseUrl, entityId);
                    break;

                case "ClientPhoto":
                    //clientphoto
                    urlString = string.Format("{0}clientphotos/{1}", baseUrl, entityId);
                    break;

                case "ClientEnrollment":
                    //client Enrollment
                    urlString = string.Format("{0}clientenrollments/{1}", baseUrl, entityId);
                    break;

                case "ClientTrackUsage":
                    //client TrackUsage
                    urlString = string.Format("{0}clienttrackusage/{1}", baseUrl, entityId);
                    break;
            }

            return urlString;

        }
    }
}
