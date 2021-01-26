using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CW.Library.Models
{
    public partial class ClientDemographic
    {

        public int? Age { get; set; }
        public int EntityID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? Gender { get; set; }
        public int? PrimaryLanguage { get; set; }
        public int? Race { get; set; }
        public byte? Ethnicity { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string CellPhone { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int OwnedByOrgID { get; set; }
        public int Restriction { get; set; }
        public byte? CitizenshipStatusID { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int? AddressID { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public DateTime? AddressBeginDate { get; set; }
        public DateTime? AddressEndDate { get; set; }
        // public float? Latitude { get; set; }
        // public float? Longitude { get; set; }
        public DateTime? TodayDate { get; set; }
        public int? FamilyID { get; set; }
        public DateTime? ThisYearBD { get; set; }
        public string MaskSSN { get; set; }
        public string SSN4 { get; set; }
        public byte? DOBDataQuality { get; set; }


    }
}
