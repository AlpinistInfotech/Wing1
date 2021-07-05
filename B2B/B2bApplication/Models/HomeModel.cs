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
        public List<mdlTravellerinfo> travellerInfo { get; set; }
        
        public mdlGstInfo gstInfo { get; set; }        
        public mdlFareQuoteCondition FareQuoteCondition{ get; set; }

        
        [DataType( DataType.EmailAddress) ]
        public string emails { get; set; }
        [DataType(DataType.PhoneNumber)]
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
        public double Convenience { get; set; }
        public double TotalOtherCharge { get; set; }



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
                InfantTotalBaseFare = InfantBaseFare * InfantCount;
            }
            TotalBaseFare = AdultTotalBaseFare+ ChildTotalBaseFare+ InfantTotalBaseFare;
            TotalFare = FareQuotResponse.Select(p => p.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.TotalPrice?? 0).Sum();
            FeeSurcharge = TotalFare - TotalBaseFare;
            OtherCharge = 0;
            Convenience= FareQuotResponse.Select(p => p.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.Convenience ?? 0).Sum();
            TotalOtherCharge = OtherCharge + Convenience;
            NetFare = TotalOtherCharge + TotalFare;
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

        public async Task LoadFareQuotationAsync(int CustomerId, IBooking _booking, IMarkup _markup)
        {
            _booking.CustomerId = CustomerId;
            this.FareQuotResponse = new List<mdlFareQuotResponse>();
            this.FareRule = new List<mdlFareRuleResponse>();
            this.FareQuotResponse.AddRange(await _booking.FareQuoteAsync(FareQuoteRequest));
            foreach (var md in FareQuotResponse)
            {
                _markup.CustomerId = CustomerId;
                _markup.CustomerMarkup(md.Results);
                _markup.WingMarkupAmount(md.Results, md.SearchQuery.AdultCount, md.SearchQuery.ChildCount, md.SearchQuery.InfantCount);
                if (this.travellerInfo == null)
                {
                    this.travellerInfo = new List<mdlTravellerinfo>();
                    for (int i = 0; i < md.SearchQuery.AdultCount; i++)
                    {
                        this.travellerInfo.Add(new mdlTravellerinfo()
                        {
                            Title = "MR",
                            passengerType = enmPassengerType.Adult,
                            FirstName = string.Empty,
                            LastName = string.Empty,
                        });
                    }
                    for (int i = 0; i < md.SearchQuery.ChildCount; i++)
                    {
                        this.travellerInfo.Add(new mdlTravellerinfo()
                        {
                            Title = "MASTER",
                            passengerType = enmPassengerType.Child,
                            FirstName = string.Empty,
                            LastName = string.Empty,
                        });
                    }
                    for (int i = 0; i < md.SearchQuery.InfantCount; i++)
                    {
                        this.travellerInfo.Add(new mdlTravellerinfo()
                        {
                            Title = "MASTER",
                            passengerType = enmPassengerType.Infant,
                            FirstName = string.Empty,
                            LastName = string.Empty,
                        });
                    }

                }
                _markup.WingConvenienceAmount(md, this.travellerInfo);
                _markup.CalculateTotalPriceAfterMarkup(md.Results, md.SearchQuery.AdultCount, md.SearchQuery.ChildCount, md.SearchQuery.InfantCount);
            }
            
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
        public MultiSelectList _CustomerMaster { get; set; }
        public MultiSelectList _Airline { get; set; }
        public MultiSelectList _CustomerType { get; set; }
        public MultiSelectList _ServiceProvider { get; set; }
        public MultiSelectList _PassengerType { get; set; }
        public MultiSelectList _CabinClass { get; set; }

        public class BasicData
        {
            public int Id { get; set; }
            public string  Name { get; set; }
        }

        public void SetDefaultDropDown(DBContext _context)
        {
            GetAirline(_context);
            GetCustomer(_context);
            GetCustomerType();
            GetServiceProvider();
            GetPassengerType();
            GetCabinClass();
        }

        protected void GetAirline(DBContext _context)
        {
          _Airline= new MultiSelectList( _context.tblAirline.Where(p => !p.IsDeleted).Select(p => new { AirlineId = p.Id, AirlineName = p.Name + " - " + p.Code }), "AirlineId", "AirlineName", WingMarkup?.MarkupAirline);
        }
        protected void GetCustomer(DBContext _context)
        {
            _CustomerMaster = new MultiSelectList(_context.tblCustomerMaster.Where(p => p.IsActive).Select(p => new { CustomerId = p.Id, CustomerName = p.CustomerName + " - " + p.Code }), "CustomerId", "CustomerName", WingMarkup?.MarkupCustomerDetail);
        }
        protected void GetCustomerType()
        {
            List<BasicData> bd = new List<BasicData>();
            var temps=Enum.GetValues(typeof(enmCustomerType));
            foreach (var temp in temps)
            {
                bd.Add(new BasicData()
                {
                    Id = (int)temp,
                    Name = ((Enum)temp).GetDescription()?? temp.ToString()
                });
            }
            _CustomerType = new MultiSelectList(bd, "Id", "Name", WingMarkup?.MarkupCustomerType);
        }
        protected void GetServiceProvider()
        {
            List<BasicData> bd = new List<BasicData>();
            var temps = Enum.GetValues(typeof(enmServiceProvider));
            foreach (var temp in temps)
            {
                if ((enmServiceProvider)temp == enmServiceProvider.None)
                {
                    continue;
                }

                bd.Add(new BasicData()
                {
                    Id = (int)temp,
                    Name = ((Enum)temp).GetDescription() ?? temp.ToString()
                });
            }
            _ServiceProvider = new MultiSelectList(bd, "Id", "Name", WingMarkup?.MarkupServiceProvider);
        }
        protected void GetPassengerType()
        {
            List<BasicData> bd = new List<BasicData>();
            var temps = Enum.GetValues(typeof(enmPassengerType));
            foreach (var temp in temps)
            {
                bd.Add(new BasicData()
                {
                    Id = (int)temp,
                    Name = ((Enum)temp).GetDescription() ?? temp.ToString()
                });
            }
            _PassengerType = new MultiSelectList(bd, "Id", "Name", WingMarkup?.MarkupPassengerType);
        }
        protected void GetCabinClass()
        {
            List<BasicData> bd = new List<BasicData>();
            var temps = Enum.GetValues(typeof(enmCabinClass));
            foreach (var temp in temps)
            {
                bd.Add(new BasicData()
                {
                    Id = (int)temp,
                    Name = ((Enum)temp).GetDescription() ?? temp.ToString()
                });
            }
            _CabinClass = new MultiSelectList(bd, "Id", "Name", WingMarkup?.MarkupCabinClass);
        }


    }


    public class mdlWingMarkupReport
    {
        public int Id{ get; set; }
        public string Applicability { get; set; }
        public string FlightType { get; set; }
        public string ServiceProvider{ get; set; }
        public string CustomerType{ get; set; }
        public string Customer { get; set; }
        public string PassengerType{ get; set; }
        public string FlightClass{ get; set; }
        public string Airline { get; set; }
        public double Amount { get; set; }
        public DateTime EffectiveFromDt { get; set; }
        public DateTime EffectiveToDt { get; set; }
        public DateTime ModifiedDt { get; set; }
        public string ModifiedBy { get; set; }
        public string remarks { get; set; }

    }

    public class mdlConvenienceReport
    {
        public int Id { get; set; }
        public string Applicability { get; set; }
        public string FlightType { get; set; }
        public string ServiceProvider { get; set; }
        public string CustomerType { get; set; }
        public string Customer { get; set; }
        public string PassengerType { get; set; }
        public string FlightClass { get; set; }
        public string Airline { get; set; }
        public enmGender gender { get; set; }
        public double Amount { get; set; }
        public int DayCount { get; set; }
        public DateTime EffectiveFromDt { get; set; }
        public DateTime EffectiveToDt { get; set; }
        public DateTime ModifiedDt { get; set; }
        public string ModifiedBy { get; set; }
        public string remarks { get; set; }

    }

    public class mdlDiscountReport
    {
        public int Id { get; set; }
        public string Applicability { get; set; }
        public string FlightType { get; set; }
        public string ServiceProvider { get; set; }
        public string CustomerType { get; set; }
        public string Customer { get; set; }
        public string PassengerType { get; set; }
        public string FlightClass { get; set; }
        public string Airline { get; set; }
        public enmGender gender { get; set; }
        public double Amount { get; set; }
        public int DayCount { get; set; }
        public DateTime EffectiveFromDt { get; set; }
        public DateTime EffectiveToDt { get; set; }
        public DateTime ModifiedDt { get; set; }
        public string ModifiedBy { get; set; }
        public string remarks { get; set; }

    }

    public class mdlCustomerFlightAPIReport
    {
        public int Id { get; set; }
       
        public string FlightType { get; set; }
        public string ServiceProvider { get; set; }
        public string CustomerType { get; set; }
        public string Customer { get; set; }
 
        public string FlightClass { get; set; }
        public string Airline { get; set; }
     
        public DateTime EffectiveFromDt { get; set; }
        public DateTime EffectiveToDt { get; set; }
        public DateTime ModifiedDt { get; set; }
        public string ModifiedBy { get; set; }
        public string remarks { get; set; }

    }


    public class mdlFlightCancel
    {
        public string traceId { get; set; }
        public int segementDisplayOrder { get; set; }
        public string remarks { get; set; }
        public List< mdlPassengers> passengers { get; set; }
    }
    public class mdlPassengers
    {
        public int pid { get; set; }
        public bool check { get; set; }
    }
}


