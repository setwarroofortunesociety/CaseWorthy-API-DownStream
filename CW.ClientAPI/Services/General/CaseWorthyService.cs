using System.Net.Http;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using CW.ClientAPI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace CW.ClientAPI.Services.General
{
    public class CaseWorthyService : ICaseWorthyService  
    {

        public static HttpClient Clientx { get; set; }
        public static string accessKey { get; }
        public static string secretKey { get; }
        public static string baseUrl { get; }

        private string urlString = string.Empty;
        

        private  readonly CaseWorthySettingsModel _caseworthySettings;
        private readonly ILogger<CaseWorthyService> _logger;

        public CaseWorthyService(IOptions<CaseWorthySettingsModel> caseworthySettings,
                                 ILogger<CaseWorthyService> logger)

        {
            _caseworthySettings = caseworthySettings.Value;
            _logger = logger;
        }

      
        public async Task<string> GetDataAsync()
        {
            try
            {
                 Clientx = new HttpClient();
                 Clientx.BaseAddress = new Uri(_caseworthySettings.BaseAddress);
                 Clientx.DefaultRequestHeaders.Accept.Clear();
                 Clientx.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                
                 var response = await Clientx.GetAsync("/").ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return response.ToString();
            }
            catch (HttpRequestException e)
            {
                return ($"Internal server error.  Error message {e.Message}");
            }



        }

        public HttpClient GetHttpClient()
        {

                Clientx = new HttpClient();
                Clientx.BaseAddress = new Uri(_caseworthySettings.BaseAddress);
                Clientx.DefaultRequestHeaders.Accept.Clear();
                Clientx.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                return Clientx;
         



        }

        public string GetAccessKey()
        {
            string accessKey = _caseworthySettings.AccessKey;

            if (string.IsNullOrWhiteSpace(accessKey))
            {
                return ($"Configuration error.  Access Key not found.");
            }

            return accessKey;

        }


        public CaseWorthySettingsModel GetCaseWorthySettings()
        {
         
               if(_caseworthySettings == null)
            {
                return null;
            }
                return _caseworthySettings;

        }


        public string GetSecretKey()
        {
             
            string secretKey = _caseworthySettings.SecretKey;

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                return ($"Configuration error.  Secret Key not found.");
            }

            return secretKey;

        }

        public string GetBaseUrl()
        {

            string baseUrl = _caseworthySettings.BaseAddress;

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return ($"Configuration error.  Base Address not found.");
            }

            return baseUrl;

        }

    }
}
