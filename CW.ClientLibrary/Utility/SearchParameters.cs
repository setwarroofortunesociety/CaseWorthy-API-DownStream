using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CW.ClientLibrary.Utility
{
     public class SearchParameters
    { 
        public string Query { get; set; }
        public int Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string Type { get; set; }
        public int? ActionID { get; set; }
    }
}
