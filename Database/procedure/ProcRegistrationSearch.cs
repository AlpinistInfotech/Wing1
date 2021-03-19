namespace Database
{
    public class ProcRegistrationSearch
    {
        public string TCID { get; set; }
        public string JoiningDate { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string landmark { get; set; }
        public int countryid { get; set; }
        public string countryname { get; set; }
        public int stateid { get; set; }
        public string statename { get; set; }
        public int cityid { get; set; }
        public string cityname { get; set; }
        public string pincode { get; set; }
        public bool terminate { get; set; }
        public bool block { get; set; }
        public string terminatename { get; set; }
        public string blockname { get; set; }
        public enmApprovalType isactive { get; set; }
        public string activename { get; set; }
        public int tcnid { get; set; }
        public int tcspnid { get; set; }
        public string tcspid { get; set; }
        public string tcspname { get; set; }
        public string mobileno { get; set; }
        public string emailid { get; set; }

        public string gender { get; set; }
        public int gender_id { get; set; }

        public int approve_by { get; set; }
        public string approve_datetime { get; set; }
        public string approve_remarks { get; set; }
        public string husband_wifename { get; set; }

        public string nominee_name { get; set; }
        public string nominee_relation { get; set; }


    }
}