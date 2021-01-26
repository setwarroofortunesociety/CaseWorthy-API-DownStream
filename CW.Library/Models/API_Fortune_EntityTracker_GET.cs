using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CW.Library.Models
{
    public partial class API_Fortune_EntityTracker_GET
    {

        public string EntityType { get; set; }
        public int ClientID { get; set; }
        public int EntityID { get; set; }
        public int? SubEntityID { get; set; }
        public int? SubEntityID2 { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string SubTopic { get; set; }

    }
}
