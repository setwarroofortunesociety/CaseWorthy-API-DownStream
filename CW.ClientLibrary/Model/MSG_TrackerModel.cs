using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CW.ClientLibrary.Models
{
    public class MSG_TrackerModel
    {
        [Key]
        public Int64 MSGID { get; set; }

        [Key, ForeignKey("IntervalID")]
        public Int64 IntervalID { get; set; }

        [Required]
        [JsonProperty("EntityType")]
        public string EntityType { get; set; }

        [JsonProperty("ClientID")]
        public Int64? ClientID { get; set; }

        [JsonProperty("EntityID")]
        public int? EntityID { get; set; }

        [JsonProperty("SubEntityID")]
        public int? SubEntityID { get; set; }

        [JsonProperty("SubEntityID2")]
        public int? SubEntityID2 { get; set; }

        [JsonProperty("lastModifiedDate")]
        public DateTime? LastModifiedDate { get; set; }

        [JsonProperty("SubTopic")]
        public string SubTopic { get; set; }
        public int CW_ActionID { get; set; }
        public string CW_ErrorMessage { get; set; }
        public string UserStamp { get; set; }
        public DateTime DateTimeStamp { get; set; }
        public int? FIX_ActionID { get; set; }
        public DateTime? FIX_ActionDateTimestamp { get; set; }
        public string FIX_Message { get; set; }

        public MSG_IntervalModel MsgInterval { get; set; }

    }


    public partial class RootObjectTracker
    { 
        public  List<MSG_TrackerModel>  entityTracker { get; set; }

    }
}
