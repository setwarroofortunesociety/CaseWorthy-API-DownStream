using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CW.ClientLibrary.Models
{
    public class MSG_ContentModel
    {
        [Key]
        public Int64 ContentID { get; set; }
        
        [Key,ForeignKey("MSGID")]
        public Int64 MSGID { get; set; }
        public string Content { get; set; }


        public MSG_TrackerModel MsgTracker { get; set; }
    }
}
