using System;
using System.Net.Http;
using System.Threading.Tasks;
using CW.ClientAPI.Models;

namespace CW.ClientAPI.Services.General
{
    public interface ICaseWorthyService
    {
        Task<string> GetDataAsync();
        string GetAccessKey();
        string GetSecretKey();
        string GetBaseUrl();
        CaseWorthySettingsModel GetCaseWorthySettings();

        HttpClient GetHttpClient();
    }
}
   
