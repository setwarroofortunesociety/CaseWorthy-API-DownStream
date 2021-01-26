using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CW.Library.Helper
{
    public class CWHelper
    {
        public static HttpClient CWRestClient { get; set; }

        public static void InitializeClient()
        {
            CWRestClient = new HttpClient();
            CWRestClient.BaseAddress = new Uri("http://localhost:3001/");
            CWRestClient.DefaultRequestHeaders.Accept.Clear();
            CWRestClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        }

    }


}
