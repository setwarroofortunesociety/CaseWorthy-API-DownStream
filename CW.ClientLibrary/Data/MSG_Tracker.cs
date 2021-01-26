using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CW.ClientLibrary.Data
{
    public class MSG_Tracker
    {
        [Key]
        public Int64 MSGID   { get; set;}

        [Key, ForeignKey("IntervalID")]
        public Int64 IntervalID    { get; set;}

        [Required]
        public string EntityType   { get; set;}
        public Int64? ClientID   { get; set;}
        public int? EntityID    { get; set;}
        public int? SubEntityID { get; set;}
        public int? SubEntityID2  { get; set;}
        public DateTime? LastModifiedDate { get; set; }
        public string SubTopic { get; set; }
        public int CW_ActionID  { get; set; }
        public string CW_ErrorMessage { get; set; }
        public string UserStamp     { get; set; }
        public DateTime DateTimeStamp { get; set;}
        public int? FIX_ActionID     { get; set; }
        public DateTime? FIX_ActionDateTimestamp { get; set; }
        public string FIX_Message { get; set; }

        public MSG_Interval MsgInterval { get; set; }

    }
}
