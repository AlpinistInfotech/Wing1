using B2BClasses.Database;
using B2BClasses.Services.Air;
using B2BClasses.Services.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2BClasses
{
    


    public interface IBooking
    {
        int CustomerId { get; set; }
        int UserId { get; set; }

        Task<mdlBookingResponse> BookingAsync(mdlBookingRequest mdlRq);
        bool CompleteBooking(string traceIds, enmBookingStatus bookingStatus);
        Task<string> CustomerDataSave(mdlSearchRequest mdlRq, List<tblFlightBookingProviderTraceId> traceIds,enmJourneyType journeyType);
        Task<bool> CustomerFlightDetailSave(string traceId, List<mdlFareQuotResponse> mdls);
        bool CustomerPassengerDetailSave(mdlBookingRequest mdlRq, enmBookingStatus bookingStatus, enmServiceProvider sp);
        Task<List<mdlFareQuotResponse>> FareQuoteAsync(mdlFareQuotRequest mdlRq);
        Task<List<mdlFareRuleResponse>> FareRule(mdlFareRuleRequest mdlRq);
        List<tblFlightBookingMaster> FlighBookReport(DateTime FromDt, DateTime ToDate, bool OnBookingDt = true, bool AllCustomer = false, bool IncludePurchaseFare = false, enmBookingStatus bookingStatus= enmBookingStatus.All);
        Task<List<enmServiceProvider>> GetActiveProviderAsync();
        Task<List<tblAirline>> GetAirlinesAsync();
        Task<List<tblAirport>> GetAirportAsync();
        Task<IEnumerable<mdlSearchResponse>> SearchFlight(mdlSearchRequest mdlRq);
        Task<mdlSearchResponse> SearchFlightMinPrices(mdlSearchRequest mdlRq);
    }

    public class Booking : IBooking
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        private readonly IWingFlight _tripJack;

        private int _CustomerId;
        private int _userId;

        public int CustomerId { get { return _CustomerId; } set { _CustomerId = value; } }
        public int UserId { get { return _userId; } set { _userId = value; } }


        public Booking(DBContext context, IConfiguration config, ITripJack tripJack)
        {
            _context = context;
            _config = config;
            _tripJack = tripJack;
        }

        public async Task<List<tblAirline>> GetAirlinesAsync()
        {
            return await _context.tblAirline.Where(p => !p.IsDeleted).ToListAsync();
        }

        public async Task<List<tblAirport>> GetAirportAsync()
        {
            return await _context.tblAirport.Where(p => !p.IsDeleted).ToListAsync();
        }


        #region ***********************Flight********************************


        public List<tblFlightBookingMaster> FlighBookReport(DateTime FromDt, DateTime ToDate, bool OnBookingDt = true, bool AllCustomer = false, bool IncludePurchaseFare = false, enmBookingStatus bookingStatus= enmBookingStatus.All)
        {
            ToDate = Convert.ToDateTime(ToDate.AddDays(1).ToString("yyyy-MM-dd"));
            FromDt = Convert.ToDateTime(FromDt.ToString("yyyy-MM-dd"));

            IQueryable<tblFlightBookingMaster> Query = null;
            if (OnBookingDt)
            {
                Query = _context.tblFlightBookingMaster.Where(p => p.CreatedDt >= FromDt && p.CreatedDt < ToDate);
            }
            else
            {
                Query = _context.tblFlightBookingSegment.Where(p => p.TravelDt >= FromDt && p.TravelDt < ToDate).Select(p => p.tblFlightBookingMaster).Distinct();
            }
            if (bookingStatus != enmBookingStatus.All)
            {
                Query = Query.Where(p => p.BookingStatus == bookingStatus);
            }

            if (!AllCustomer)
            {
                Query = Query.Where(p => p.CustomerId == CustomerId);
            }
            if (IncludePurchaseFare)
            {
                Query = Query.Include(p => p.tblFlightBookingFarePurchaseDetails);
            }

            Query = Query.Include(p => p.tblFlightBookingSegments).Include(p => p.tblFlightBookingPassengerDetails).Include(p => p.tblFlightBookingFareDetails).Include(p => p.tblFlightBookingGSTDetails);
            return Query.OrderByDescending(p=>p.CreatedDt).ToList();
        }

        public async Task<string> CustomerDataSave(mdlSearchRequest mdlRq, List<tblFlightBookingProviderTraceId> traceIds, enmJourneyType journeyType)
        {
            tblFlightBookingMaster tbl = new tblFlightBookingMaster()
            {
                CustomerId = _CustomerId,
                AdultCount = mdlRq.AdultCount,
                ChildCount = mdlRq.ChildCount,
                InfantCount = mdlRq.InfantCount,
                DirectFlight = mdlRq.DirectFlight,
                JourneyType = journeyType,
                CreatedBy = _userId,
                CreatedDt = DateTime.Now,
                tblFlightBookingProviderTraceIds = traceIds,
                tblFlightBookingSegments = mdlRq.Segments.Select(p => new tblFlightBookingSegment
                {
                    Airline = string.Empty,
                    AirlineCode = string.Empty,
                    ArrivalTime = p.TravelDt,
                    TravelDt = p.TravelDt,
                    CabinClass = p.FlightCabinClass,
                    ClassOfBooking = string.Empty,
                    Origin = p.Origin,
                    Destination = p.Destination,
                    FlightNumber = string.Empty,
                    ProviderResultIndex = string.Empty,
                    ServiceProvider = enmServiceProvider.None,
                    DepartureTime = p.TravelDt
                }
                 ).ToList()
            };
            _context.tblFlightBookingMaster.Add(tbl);
            await _context.SaveChangesAsync();
            return tbl.Id;
        }

        public async Task<bool> CustomerFlightDetailSave(string traceId, List<mdlFareQuotResponse> mdls)
        {
            _context.tblFlightBookingSegment.RemoveRange(_context.tblFlightBookingSegment.Where(p => p.TraceId == traceId));
            _context.tblFlightBookingFareDetails.RemoveRange(_context.tblFlightBookingFareDetails.Where(p => p.TraceId == traceId));
            _context.tblFlightBookingFarePurchaseDetails.RemoveRange(_context.tblFlightBookingFarePurchaseDetails.Where(p => p.TraceId == traceId));

            int index = 1;
            foreach (var mdl in mdls)
            {
                var TPL = mdl.Results.FirstOrDefault().FirstOrDefault().TotalPriceList.FirstOrDefault();

                _context.tblFlightBookingSegment.AddRange(
                    mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.Segment?.Select(p => new tblFlightBookingSegment
                    {
                        Airline = p.Airline.Name,
                        AirlineCode = p.Airline.Code,
                        SegmentDisplayOrder = index,
                        ArrivalTime = p.ArrivalTime,
                        DepartureTime = p.DepartureTime,
                        Origin = p.Origin.CityCode,
                        Destination = p.Destination.CityCode,
                        CabinClass = TPL.ADULT.CabinClass,
                        ClassOfBooking = TPL.ADULT.ClassOfBooking,
                        FlightNumber = p.Airline.FlightNumber,
                        ProviderResultIndex = TPL.ResultIndex,
                        ServiceProvider = mdl.ServiceProvider,
                        TraceId = traceId,
                        TripIndicator = p.TripIndicator,
                        TravelDt = p.DepartureTime.Date,


                    })
                 );

                _context.tblFlightBookingFareDetails.Add(new tblFlightBookingFareDetails()
                {
                    TraceId = traceId,
                    convenience = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.TotalConvenience ?? 0,
                    NetFare = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.NetPrice ?? 0,
                    TotalFare = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.TotalPrice ?? 0,
                    WingAdultMarkup = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.ADULT?.FareComponent?.WingMarkup ?? 0,
                    WingChildMarkup = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.CHILD?.FareComponent?.WingMarkup ?? 0,
                    WingInfantMarkup = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.INFANT?.FareComponent?.WingMarkup ?? 0,
                    WingTotalMarkup = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.WingMarkup ?? 0,
                    CustomerMarkup = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.CustomerMarkup ?? 0,
                    SegmentDisplayOrder = index
                });

                double AdultBaseFare_ = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.ADULT?.FareComponent?.BaseFare ?? 0;
                double ChildBaseFare_ = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.CHILD?.FareComponent?.BaseFare ?? 0;
                double InfantBaseFare_ = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault().INFANT?.FareComponent?.BaseFare ?? 0;
                double AdultTotalFare_ = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.ADULT?.FareComponent?.TotalFare ?? 0;
                double ChildTotalFare_ = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.CHILD?.FareComponent?.TotalFare ?? 0;
                double InfantTotalFare_ = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.INFANT?.FareComponent?.TotalFare ?? 0; ;
                double AdultNetFare_ = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.ADULT?.FareComponent?.NetFare ?? 0;
                double ChildNetFare_ = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.CHILD?.FareComponent?.NetFare ?? 0;
                double InfantNetFare_ = mdl.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.INFANT?.FareComponent?.NetFare ?? 0;

                _context.tblFlightBookingFarePurchaseDetails.Add(new tblFlightBookingFarePurchaseDetails()
                {
                    TraceId = traceId,
                    Provider = mdl.ServiceProvider,
                    AdultBaseFare = AdultBaseFare_,
                    ChildBaseFare = ChildBaseFare_,
                    InfantBaseFare = InfantBaseFare_,
                    AdultTotalFare = AdultTotalFare_,
                    ChildTotalFare = ChildTotalFare_,
                    InfantTotalFare = InfantTotalFare_,
                    AdultNetFare = AdultNetFare_,
                    ChildNetFare = ChildNetFare_,
                    InfantNetFare = InfantNetFare_,
                    NetFare = ((mdl?.SearchQuery?.AdultCount ?? 0) * AdultNetFare_) + ((mdl?.SearchQuery?.ChildCount ?? 0) * ChildNetFare_) + ((mdl?.SearchQuery?.InfantCount ?? 0) * InfantNetFare_),
                    TotalFare = ((mdl?.SearchQuery?.AdultCount ?? 0) * AdultTotalFare_) + ((mdl?.SearchQuery?.ChildCount ?? 0) * ChildTotalFare_) + ((mdl?.SearchQuery?.InfantCount ?? 0) * InfantTotalFare_),
                    SegmentDisplayOrder = index
                });
                index = index + 1;

            }




            await _context.SaveChangesAsync();

            return true;
        }


        public bool CustomerPassengerDetailSave(mdlBookingRequest mdlRq, enmBookingStatus bookingStatus, enmServiceProvider sp)
        {

            var tm = _context.tblFlightBookingMaster.Where(p => p.Id == mdlRq.TraceId).FirstOrDefault();
            tm.ContactNo = (mdlRq.deliveryInfo?.contacts.FirstOrDefault() ?? string.Empty);
            tm.Email = (mdlRq.deliveryInfo?.emails.FirstOrDefault() ?? string.Empty);
            tm.BookingStatus = bookingStatus;
            _context.tblFlightBookingMaster.Update(tm);

            _context.tblFlightBookingPassengerDetails.RemoveRange(_context.tblFlightBookingPassengerDetails.Where(p => p.TraceId == mdlRq.TraceId && p.BookingId == mdlRq.BookingId));
            _context.tblFlightBookingPassengerDetails.AddRange(
            mdlRq.travellerInfo.Select(p => new tblFlightBookingPassengerDetails
            {
                Title = p.Title,
                passengerType = p.passengerType,
                FirstName = p.FirstName,
                LastName = p.LastName,
                TraceId = mdlRq.TraceId,
                dob = p.dob,
                PassportExpiryDate = p.PassportExpiryDate,
                PassportIssueDate = p.PassportIssueDate,
                pNum = p.pNum,
                BookingId = mdlRq.BookingId,
                ServiceProvider = sp
            }));

            _context.tblFlightBookingGSTDetails.RemoveRange(_context.tblFlightBookingGSTDetails.Where(p => p.TraceId == mdlRq.TraceId));
            if (mdlRq.gstInfo != null)
            {
                _context.tblFlightBookingGSTDetails.Add(new tblFlightBookingGSTDetails()
                {
                    TraceId = mdlRq.TraceId,
                    address = mdlRq.gstInfo.address,
                    email = mdlRq.gstInfo.email,
                    mobile = mdlRq.gstInfo.mobile,
                    gstNumber = mdlRq.gstInfo.gstNumber,
                    registeredName = mdlRq.gstInfo.registeredName,

                }
                 );
            }

            _context.SaveChanges();
            return true;
        }


        public bool CompleteBooking(string traceIds, enmBookingStatus bookingStatus)
        {
            var data = _context.tblFlightBookingMaster.FirstOrDefault(p => p.Id == traceIds);
            if (data != null)
            {
                data.BookingStatus = bookingStatus;
            }
            _context.tblFlightBookingMaster.Update(data);
            _context.SaveChanges();
            return true;
        }


        public async Task<List<enmServiceProvider>> GetActiveProviderAsync()
        {
            return await _context.tblActiveSerivceProvider.Where(p => p.IsEnabled).Select(p => p.ServiceProvider).ToListAsync();
        }



        private IWingFlight GetFlightObject(enmServiceProvider serviceProvider)
        {
            switch (serviceProvider)
            {
                case enmServiceProvider.TBO:
                    return null;
                case enmServiceProvider.TripJack:
                    return _tripJack;
            }
            return null;
        }

        public async Task<IEnumerable<mdlSearchResponse>> SearchFlight(
          mdlSearchRequest mdlRq)
        {

            List<mdlSearchResponse> mdlRs = new List<mdlSearchResponse>();
            List<enmServiceProvider> serviceProviders = await GetActiveProviderAsync();
            List<tblFlightBookingProviderTraceId> traceIds = new List<tblFlightBookingProviderTraceId>();

            enmJourneyType actualJourneyType = mdlRq.JourneyType;

            foreach (var sp in serviceProviders)
            {
                mdlSearchResponse mdlR = null;
                IWingFlight wingflight = GetFlightObject(sp);
                if (mdlRq.JourneyType == enmJourneyType.SpecialReturn)
                {
                    throw new NotImplementedException();
                }
                else if (mdlRq.JourneyType == enmJourneyType.OneWay)
                {
                    mdlR = await wingflight.SearchAsync(mdlRq, _CustomerId);
                    traceIds.Add(new tblFlightBookingProviderTraceId()
                    {
                        ProviderTraceId = mdlR.TraceId,
                        ServiceProvider = sp,
                        SegmentDisplayOrder = 1
                    });
                    mdlRs.Add(mdlR);
                }
                else if (mdlRq.JourneyType == enmJourneyType.Return || mdlRq.JourneyType == enmJourneyType.MultiStop)
                {

                    mdlRq.JourneyType = enmJourneyType.OneWay;
                    var lst = mdlRq.Segments.ToList();
                    for (int i = 0; i < lst.Count; i++)
                    {
                        mdlRq.Segments = new List<mdlSegmentRequest>();
                        mdlRq.Segments.Add(lst[i]);
                        var md = await wingflight.SearchAsync(mdlRq, _CustomerId);
                        traceIds.Add(new tblFlightBookingProviderTraceId()
                        {
                            ProviderTraceId = md.TraceId,
                            ServiceProvider = sp,
                            SegmentDisplayOrder = i + 1
                        });
                        if (mdlR == null)
                        {
                            mdlR = md;
                        }
                        else
                        {
                            mdlR.Results.Add(md.Results.FirstOrDefault());
                        }
                    }
                    mdlRs.Add(mdlR);
                }
            }
            string NewTraceID = await CustomerDataSave(mdlRq, traceIds, actualJourneyType);
            ChangeNewTraceIds(mdlRs, NewTraceID);
            return mdlRs;
        }

        private void ChangeNewTraceIds(List<mdlSearchResponse> mdls, string newTraceId)
        {
            foreach (var md in mdls)
            {
                md.TraceId = newTraceId;
            }
        }

        public async Task<mdlSearchResponse> SearchFlightMinPrices(mdlSearchRequest mdlRq)
        {
            mdlSearchResponse searchResponse = new mdlSearchResponse() { ResponseStatus = 0, Error = new mdlError() { Code = 0, Message = "" } };
            var res = (await SearchFlight(mdlRq)).ToList();
            if (res == null || res.Count == 0)
            {
                searchResponse.Error.Message = "No data found";
                return searchResponse;
            }
            if (res.Count() == 1)
            {
                res[0].ResponseStatus = 1;
                return res[0];
            }

            //Convert into Single
            int SegmentId = 0;
            bool IsAllSegmentAreEqual = false;
            List<List<mdlSearchResult>> Results = res.SelectMany(p => p.Results).ToList();
            for (int i = 0; i < Results.Count - 1; i++)
            {
                for (int j = Results[i].Count - 1; j > 0; j--)
                {

                    for (int k = j - 1; k >= 0; k--)
                    {
                        if (Results[i][j].Segment.Count == Results[i][k].Segment.Count)
                        {
                            IsAllSegmentAreEqual = true;
                            SegmentId = Results[i][j].Segment.Count - 1;
                            while (SegmentId >= 0)//check Wheather the Flight and segment are same or not
                            {
                                if (!(Results[i][j].Segment[SegmentId].Airline.Code == Results[i][k].Segment[SegmentId].Airline.Code
                                    && Results[i][j].Segment[SegmentId].Airline.FlightNumber == Results[i][k].Segment[SegmentId].Airline.FlightNumber))
                                {
                                    IsAllSegmentAreEqual = false;
                                    break;
                                }
                            }
                            if (IsAllSegmentAreEqual)
                            {
                                for (int PriceListCount1 = Results[i][j].TotalPriceList.Count - 1; PriceListCount1 >= 0; PriceListCount1--)
                                {

                                    for (int PriceListCount2 = Results[i][k].TotalPriceList.Count - 1; PriceListCount2 >= 0; PriceListCount2--)
                                    {
                                        if (Results[i][j].TotalPriceList[PriceListCount1].ADULT.CabinClass == Results[i][k].TotalPriceList[PriceListCount2].ADULT.CabinClass
                                            && Results[i][j].TotalPriceList[PriceListCount1].ADULT.ClassOfBooking == Results[i][k].TotalPriceList[PriceListCount2].ADULT.ClassOfBooking)
                                        {
                                            if (Results[i][j].TotalPriceList[PriceListCount1].ADULT.FareComponent.TotalFare > Results[i][k].TotalPriceList[PriceListCount2].ADULT.FareComponent.TotalFare)
                                            {
                                                Results[i][j].TotalPriceList.RemoveAt(PriceListCount1);
                                                goto PriceListCount1_;
                                            }
                                        }

                                    }
                                PriceListCount1_:;
                                }
                                Results[i][k].TotalPriceList.AddRange(Results[i][j].TotalPriceList);
                                Results[i].RemoveAt(j);
                                goto outerLoop;
                            }

                        }

                    }
                outerLoop:;

                }
            }
            searchResponse.Results = Results;
            searchResponse.ResponseStatus = 1;
            return searchResponse;

        }

        public async Task<List<mdlFareQuotResponse>> FareQuoteAsync(
             mdlFareQuotRequest mdlRq)
        {
            List<mdlFareQuotResponse> mdlRs = new List<mdlFareQuotResponse>();
            for (int i = 0; i < mdlRq.ResultIndex.Count(); i++)
            {
                if (mdlRq.ResultIndex[i] == null)
                {
                    continue;
                }
                var sp = (enmServiceProvider)Convert.ToInt32(mdlRq.ResultIndex?[i].Split("_").FirstOrDefault());

                int index = mdlRq.ResultIndex?[i].IndexOf('_') ?? -1;
                if (index >= 0)
                {
                    var ClientTraceID = _context.tblFlightBookingProviderTraceId.Where(p => p.TraceId == mdlRq.TraceId && p.SegmentDisplayOrder == (i + 1) && p.ServiceProvider == sp).FirstOrDefault()?.ProviderTraceId ?? "";
                    List<string> resIndex = new List<string>();
                    resIndex.Add(mdlRq.ResultIndex?[i].Substring(index + 1));
                    IWingFlight wingflight = GetFlightObject(sp);
                    mdlRs.Add(await wingflight.FareQuoteAsync(new mdlFareQuotRequest() { TraceId = ClientTraceID, ResultIndex = resIndex.ToArray() }));

                }
            }

            return mdlRs;
        }





        public async Task<List<mdlFareRuleResponse>> FareRule(
             mdlFareRuleRequest mdlRq)
        {
            List<mdlFareRuleResponse> mdlRs = new List<mdlFareRuleResponse>();
            for (int i = 0; i < mdlRq.ResultIndex.Count(); i++)
            {
                var sp = (enmServiceProvider)Convert.ToInt32(mdlRq.ResultIndex?[i].Split("_").FirstOrDefault());

                int index = mdlRq.ResultIndex[i].IndexOf('_');
                mdlRq.ResultIndex[i] = mdlRq.ResultIndex[i].Substring(index + 1);
                IWingFlight wingflight = GetFlightObject(sp);
                mdlRs.Add(await wingflight.FareRuleAsync(mdlRq));
            }
            return mdlRs;
        }

        public async Task<mdlBookingResponse> BookingAsync(mdlBookingRequest mdlRq)
        {
            mdlBookingResponse mdlRs = new mdlBookingResponse();
            var sp = (enmServiceProvider)Convert.ToInt32(mdlRq.BookingId?.Split("_").FirstOrDefault());
            int index = mdlRq.BookingId?.IndexOf('_') ?? -1;
            if (index >= 0)
            {
                mdlRq.BookingId = mdlRq.BookingId.Substring(index + 1);
                IWingFlight wingflight = GetFlightObject(sp);
                mdlRs = await wingflight.BookingAsync(mdlRq);

                if (mdlRs.ResponseStatus == 1)
                {
                    CustomerPassengerDetailSave(mdlRq, enmBookingStatus.Booked, sp);
                }
                else
                {
                    CustomerPassengerDetailSave(mdlRq, enmBookingStatus.Failed, sp);
                }
            }
            return mdlRs;
        }
        #endregion

    }
}
