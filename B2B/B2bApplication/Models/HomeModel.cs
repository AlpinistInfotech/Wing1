using B2BClasses;
using B2BClasses.Database;
using B2BClasses.Services.Air;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B2BClasses.Services.Enums;
using System.ComponentModel.DataAnnotations;
using B2BClasses.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace B2bApplication.Models
{
    public class mdlFlightReview
    {
        public mdlFareQuotRequest FareQuoteRequest { get; set; }
        public List<mdlFareQuotResponse> FareQuotResponse { get; set; }
        public List<mdlFareRuleResponse> FareRule{ get; set; }
        //public List<string> TraceId { get; set; }
        //public List<string> BookingId { get; set; }
        public List< mdlTravellerinfo> travellerInfo { get; set; }
        
        public mdlGstInfo gstInfo { get; set; }        
        public mdlFareQuoteCondition FareQuoteCondition{ get; set; }

        
        public string emails { get; set; }
        public string contacts { get; set; }

        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public int InfantCount { get; set; }

        public double AdultBaseFare { get; set; }
        public double ChildBaseFare { get; set; }
        public double InfantBaseFare { get; set; }

        public double AdultTotalBaseFare { get; set; }
        public double ChildTotalBaseFare { get; set; }
        public double InfantTotalBaseFare { get; set; }

        public double TotalBaseFare { get; set; }
        public double FeeSurcharge { get; set; }
        public double TotalFare { get; set; }
        public double OtherCharge { get; set; }        
        public double NetFare { get; set; }
        

        public void SetFareAmount()
        {
            AdultTotalBaseFare = FareQuotResponse.Select(p => p.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.ADULT?.FareComponent?.BaseFare ?? 0).Sum();
            ChildTotalBaseFare = FareQuotResponse.Select(p => p.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.CHILD?.FareComponent?.BaseFare ?? 0).Sum();
            InfantTotalBaseFare = FareQuotResponse.Select(p => p.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.INFANT?.FareComponent?.BaseFare ?? 0).Sum();
            if (AdultCount > 0)
            {
                AdultBaseFare = AdultTotalBaseFare ;
                AdultTotalBaseFare = AdultBaseFare * AdultCount;
            }
            if (ChildCount > 0)
            {
                ChildBaseFare = ChildTotalBaseFare ;
                ChildTotalBaseFare = ChildBaseFare * ChildCount;
            }
            if (InfantCount > 0)
            {
                InfantBaseFare = InfantTotalBaseFare ;
                InfantTotalBaseFare = InfantBaseFare * InfantBaseFare;
            }
            TotalBaseFare = AdultTotalBaseFare + ChildTotalBaseFare + InfantTotalBaseFare;

            double AdultTotalFare = FareQuotResponse.Select(p => p.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.ADULT?.FareComponent?.TotalFare ?? 0).Sum();
            double ChildTotalFare = FareQuotResponse.Select(p => p.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.CHILD?.FareComponent?.TotalFare ?? 0).Sum();
            double InfantTotalFare = FareQuotResponse.Select(p => p.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.INFANT?.FareComponent?.TotalFare ?? 0).Sum();
            TotalFare = (AdultTotalFare* AdultCount) + (ChildTotalFare * ChildCount)+ ( InfantTotalFare * InfantCount);
            FeeSurcharge = TotalFare - TotalBaseFare;
            OtherCharge = 0;
            NetFare = TotalFare + OtherCharge;
        }

        public void SetFareQuoteCondtion()
        {
            if (FareQuoteCondition == null)
            {
                FareQuoteCondition = new mdlFareQuoteCondition()
                {
                    dob = new mdlDobCondition()
                    {
                        adobr = FareQuotResponse.Any(p => p.FareQuoteCondition?.dob?.adobr ?? false),
                        cdobr = FareQuotResponse.Any(p => p.FareQuoteCondition?.dob?.cdobr ?? false),
                        idobr = FareQuotResponse.Any(p => p.FareQuoteCondition?.dob?.idobr ?? false),
                    },
                    GstCondition = new mdlGstCondition()
                    {
                        IsGstMandatory = FareQuotResponse.Any(p => p.FareQuoteCondition?.GstCondition?.IsGstMandatory ?? false),
                        IsGstApplicable = FareQuotResponse.Any(p => p.FareQuoteCondition?.GstCondition?.IsGstApplicable ?? false),
                    },
                    IsHoldApplicable = FareQuotResponse.All(p => p.FareQuoteCondition?.IsHoldApplicable ?? false),
                    PassportCondition = new mdlPassportCondition()
                    {
                        IsPassportExpiryDate = FareQuotResponse.Any(p => p.FareQuoteCondition?.PassportCondition?.IsPassportExpiryDate ?? false),
                        isPassportIssueDate = FareQuotResponse.Any(p => p.FareQuoteCondition?.PassportCondition?.isPassportIssueDate ?? false),
                        isPassportRequired = FareQuotResponse.Any(p => p.FareQuoteCondition?.PassportCondition?.isPassportRequired ?? false),
                    }
                };
            }

                
        }

        public void BookingRequestDefaultData()
        {
            if (FareQuotResponse == null || FareQuotResponse.Count == 0)
            {
                return;
            }
            //TraceId = new List<string>();
            //BookingId= new List<string>();
            //BookingId.AddRange(FareQuotResponse.Select(p => p.BookingId));
            //TraceId.AddRange(FareQuotResponse.Select(p => p.TraceId));


            if (travellerInfo == null)
            {
                travellerInfo = new List<mdlTravellerinfo>();

                int AdultCount = FareQuotResponse?.FirstOrDefault()?.SearchQuery?.AdultCount ?? 0;
                int ChildCount = FareQuotResponse?.FirstOrDefault()?.SearchQuery?.ChildCount ?? 0;
                int InfantCount = FareQuotResponse?.FirstOrDefault()?.SearchQuery?.InfantCount ?? 0;
                this.AdultCount = AdultCount;
                this.ChildCount = ChildCount;
                this.InfantCount = InfantCount;

                while (AdultCount > 0)
                {
                    travellerInfo.Add(new mdlTravellerinfo() { passengerType = enmPassengerType.Adult });
                    AdultCount--;
                }
                while (ChildCount > 0)
                {
                    travellerInfo.Add(new mdlTravellerinfo() { passengerType = enmPassengerType.Child });
                    ChildCount--;
                }
                while (InfantCount > 0)
                {
                    travellerInfo.Add(new mdlTravellerinfo() { passengerType = enmPassengerType.Infant });
                    InfantCount--;
                }
            }
            else
            {
                this.AdultCount= FareQuotResponse?.FirstOrDefault()?.SearchQuery?.AdultCount ?? 0;
                this.ChildCount = FareQuotResponse?.FirstOrDefault()?.SearchQuery?.ChildCount ?? 0;
                this.InfantCount = FareQuotResponse?.FirstOrDefault()?.SearchQuery?.InfantCount ?? 0;
            }

            
            SetFareQuoteCondtion();
            SetFareAmount();
        }
    }

    public class mdlFlighBook
    {
        public List<mdlFareQuotResponse> FareQuotResponse { get; set; }
        public List<bool> IsSucess { get; set; }
        public List<string> BookingId { get; set; }
    }


    public class mdlWingMarkupWraper
    { 
        public mdlWingMarkup WingMarkup { get; set; }
        public SelectList _CustomerMaster { get; set; }
        public SelectList _Airline { get; set; }

        public  List<tblCustomerMaster> CustomerMaster { get; set; }
        public List<tblAirline> Airline { get; set; }
        

    }
    
}


