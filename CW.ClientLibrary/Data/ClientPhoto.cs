using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace CW.ClientLibrary.Data
{
    public partial class ClientPhoto
    {
        [Key]
        public Int64 ClientPhotoID { get; set; }
         public int? ClientID { get; set; }
        public int PrintPhotoFileID { get; set; }
        public int ContextTypeID { get; set; }
        public int FileClassification { get; set; }
        public string FileLabel { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public string FileDataLink { get; set; }
        public byte? Restriction { get; set; }
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
        public int? LegacyID { get; set; }
        public string UserStamp  { get; set; }
        public DateTime? DateTimeStamp { get; set; }
     

    }
}