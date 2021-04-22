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

        Task<mdlBookingResponse> BookingAsync(mdlBookingRequest mdlRq);
        Task<List<mdlFareQuotResponse>> FareQuoteAsync(mdlFareQuotRequest mdlRq);
        Task<List<mdlFareRuleResponse>> FareRule(mdlFareRuleRequest mdlRq);
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

        public int CustomerId { get { return _CustomerId; } set { _CustomerId = value; } }


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
            //Get the All Active Service Provider
            List<mdlSearchResponse> mdlRs = new List<mdlSearchResponse>();
            List<enmServiceProvider> serviceProviders = await GetActiveProviderAsync();
            foreach (var sp in serviceProviders)
            {
                IWingFlight wingflight = GetFlightObject(sp);
                mdlRs.Add(await wingflight.SearchAsync(mdlRq, _CustomerId));
            }
            return mdlRs;
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
                var sp = (enmServiceProvider)Convert.ToInt32(mdlRq.ResultIndex?[i].Split("_").FirstOrDefault());

                
                int index = mdlRq.ResultIndex?[i].IndexOf('_') ?? -1;
                if (index >= 0)
                {
                    List<string> resIndex = new List<string>();
                    resIndex.Add(mdlRq.ResultIndex?[i].Substring(index + 1));
                    IWingFlight wingflight = GetFlightObject(sp);
                    mdlRs.Add(await wingflight.FareQuoteAsync(new mdlFareQuotRequest() { TraceId = mdlRq.TraceId, ResultIndex = resIndex.ToArray() }));                    

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
            }
            return mdlRs;
        }
        #endregion

    }
}
