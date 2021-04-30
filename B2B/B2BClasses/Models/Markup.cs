﻿using B2BClasses.Database;
using B2BClasses.Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace B2BClasses.Models
{

    

    public class mdlWingMarkup
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Applicability")]        
        public enmMarkupApplicability Applicability { get; set; }
        [Display(Name = "Flight Type")]
        public enmDirectFlight DirectFlight { get; set; }
        [Display(Name = "Provider")]
        public bool IsAllProvider { get; set; }        
        [Display(Name = "Customer Type")]
        public bool IsAllCustomerType { get; set; }        
        [Display(Name = "Customer")]
        public bool IsAllCustomer { get; set; }        
        [Display(Name = "Passenger Type")]
        public bool IsAllPessengerType { get; set; }//Applicable For All Pasenger
        [Display(Name = "Flight Class")]
        public bool IsAllFlightClass { get; set; }
        [Display(Name = "Airline")]
        public bool IsAllAirline { get; set; }
        [Display(Name = "Gender")]
        public enmGender Gender { get; set; }        
        [Display(Name = "Amount")]
        public double Amount { get; set; }
        [Display(Name = "Effective FromDt")]
        public DateTime EffectiveFromDt { get; set; }        
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
        [Display(Name = "Modified By")]
        public string ModifiedByName { get; set; }
        [Display(Name = "Remarks")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.")]
        public string remarks { get; set; }

        public List<enmServiceProvider> MarkupServiceProvider { get; set; }
        public List<enmCustomerType> MarkupCustomerType { get; set; }        
        public List<int> MarkupCustomerDetail { get; set; }        
        public List<enmPassengerType> MarkupPassengerType { get; set; }
        public List<enmCabinClass> MarkupCabinClass { get; set; }        
        public List<int> MarkupAirline { get; set; }
        public List<string> MarkupAirlineCode { get; set; }


    }
}
