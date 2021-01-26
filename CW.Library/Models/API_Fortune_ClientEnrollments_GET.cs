using System;
using System.Collections.Generic;
using System.Text;

namespace CW.Library.Models
{
    public partial class API_Fortune_ClientEnrollments_GET
    {
        public int EnrollmentID { get; set; }
        public int ClientID { get; set; }
        public int? FamilyID { get; set; }
        public int? ProgramID { get; set; }
        public string ProgramName { get; set; }
        public int? AccountID { get; set; }
        public string AccountName { get; set; }
        public int? OrganizationID { get; set; }
        public byte? Status { get; set; }
        public string StatusText { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? LegacyID { get; set; }
        public string DeniedReason { get; set; }
        public int? FamilyOrIndividual { get; set; }
        public string SubStatus { get; set; }
        public DateTime? ExitTimeStamp { get; set; }
        public string X_EnrollmentStatus { get; set; }
        public string X_EnrollmentComment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedBy { get; set; }
        public int? PrimWkrUserID { get; set; }
        public string PrimWkrUserName { get; set; }
        public string PrimWkrFirstName { get; set; }
        public string PrimWkrLastName { get; set; }
    }
}
