using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CW.ClientLibrary.Utility
{
    internal class TrackerParameters
    { 
        public string SrchQry { get; set; }
        public int? SrchQry_ActionID { get; set; }
        public DateTime? QryStartDate { get; set; }
        public DateTime? QryEndDate { get; set; }
        public string  QryType { get; set; }
    }
}
