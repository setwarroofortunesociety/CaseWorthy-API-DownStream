using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CW.ClientLibrary.Models
{
    public class MSG_IntervalModel
    {
        [Key]
       public Int64 IntervalID { get; set; }

       public DateTime StartDateTime { get; set; }
       public DateTime EndDateTime { get; set; }
       public string EntityType { get; set; }
       public int Status { get; set; }
       public string Comments { get; set; }
       public string UserStamp { get; set; }
       public DateTime DateTimeStamp { get; set; }
      
    }
}
