using B2BClasses.Services.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace B2BClasses.Models
{



    public class mdlWingMarkup
    {
        public int Id { get; set; }
        public bool IsAllProvider { get; set; }
        public bool IsAllCustomer { get; set; }
        public bool IsApplicableOnEachPasenger { get; set; }// Is Markup is applicable on Each Passnger or on Ticket
        public bool IsAllPessenger { get; set; }//Applicable For All Pasenger
        public bool IsAllFlightClass { get; set; }
        public enmGender Gender { get; set; }
        public double MarkupAmt { get; set; }
        public DateTime EffectiveFromDt { get; set; }
        public DateTime EffectiveToDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }        
    }
}
