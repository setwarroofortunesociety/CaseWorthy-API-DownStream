using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CW.ClientLibrary.Data;
using Newtonsoft.Json;

namespace CW.ClientLibrary.Models
{
    public partial class FSP_CW_FHIR_Account_Fetch_Model
    {
        [JsonProperty("JSONMsg")]
        public string JSONMsg { get; set; }


    }
}