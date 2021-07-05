using System;

namespace Database
{
    public class ProcHolidayPackageSearch
    {
        public int DetailId { get; set; }
        public string PackageName { get; set; }

        public enmPackageCustomerType PackageType { get; set; }

        public string PackageFromDate { get; set; }

        public string PackageToDate { get; set; }

        public int PriceFrom { get; set; }

        public int PriceTo { get; set; }
        public int MemberCount { get; set; }

        public int DaysCount { get; set; }


        public int country_id { get; set; }
        public int state_id { get; set; }

        public enmStatus is_active { get; set; }

        
        public string  country_name { get; set; }
        public string state_name { get; set; }

        public string PackageDescription { get; set; }
        public string SpecialNote { get; set; }
        public string UploadPackageImage { get; set; }
        public string UploadOtherImage { get; set; }

        public string CreatedDt { get; set; }
        public int CreatedByid { get; set; }
        public string CreatedByName { get; set; }

        public bool Isdeleted { get; set; }

        public string LastModifieddate { get; set; }
        public int lastModifiedByid { get; set; }
        public string lastModifiedByName { get; set; }

    }
}