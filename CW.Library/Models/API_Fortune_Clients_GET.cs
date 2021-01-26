using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CW.Library.Models
{
    public partial class API_Fortune_Clients_GET
    {
       
        public int ClientID { get; set; }
        public int PrintPhotoFileID { get; set; }
        public int ContextTypeID { get; set; }
        public int? FileClassification { get; set; }
        public string FileLabel { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public string FileDataLink { get; set; }
        public int? Restriction { get; set; }
        public int? OwnedByOrgID { get; set; }
        public bool? IsEncrypted { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? OrgGroupID { get; set; }
        public int? WriteOrgGroupID { get; set; }
        public int? CreatedFormID { get; set; }
        public int? LastModifiedFormID { get; set; }
        public string ImageFileBinary { get; set; }
        public string Stg_UserName { get; set; }
        public DateTime? Stg_DateTimeStamp { get; set; }
        public int? Stg_ActionID { get; set; }
        public DateTime? Stg_ActionDateTimestamp { get; set; }

    }
}
