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
    public partial class ClientPhotoModel
    {
        [Key]
        public Int64 ClientPhotoID { get; set; }

        [JsonProperty("clientID")]
        public int? ClientID { get; set; }

        [JsonProperty("printPhotoFileID")]
        public int PrintPhotoFileID { get; set; }

        [JsonProperty("contextTypeID")]
        public int ContextTypeID { get; set; }

        [JsonProperty("fileClassification")]
        public int FileClassification { get; set; }

        [JsonProperty("fileLabel")]
        public string FileLabel { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("fileDataLink")]
        public string FileDataLink { get; set; }

        [JsonProperty("restriction")]
        public byte? Restriction { get; set; }
        [JsonProperty("ownedByOrgID")]
        public int? OwnedByOrgID { get; set; }
        [JsonProperty("isEncrypted")]
        public bool? IsEncrypted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public int? LastModifiedBy { get; set; }
        [JsonProperty("lastModifiedDate")]
        public DateTime? LastModifiedDate { get; set; }

        [JsonProperty("createdBy")]
        public int? CreatedBy { get; set; }

        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty("orgGroupID")]
        public int? OrgGroupID { get; set; }

        [JsonProperty("writeOrgGroupID")]
        public int? WriteOrgGroupID { get; set; }

        [JsonProperty("createdFormID")]
        public int? CreatedFormID { get; set; }

        [JsonProperty("lastModifiedFormID")]
        public int? LastModifiedFormID { get; set; }

        [JsonProperty("imageFileBinary")]
        public string ImageFileBinary { get; set; }
        public int? LegacyID { get; set; }
        public string UserStamp { get; set; }
        public DateTime? DateTimeStamp { get; set; }
    


    }


    public partial class RootObjectPhoto
    {
        public List<ClientPhotoModel> ClientPhoto { get; set; }

    }
}
