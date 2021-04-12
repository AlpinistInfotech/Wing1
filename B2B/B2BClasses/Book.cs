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

        Task<mdlFareQuotResponse> FareQuote(mdlFareQuotRequest mdlRq);
        Task<mdlFareRuleResponse> FareRule(mdlFareRuleRequest mdlRq);
        Task<List<enmServiceProvider>> GetActiveProviderAsync();
        Task<List<tblAirline>> GetAirlinesAsync();
        Task<List<tblAirport>> GetAirportAsync();
        Task<IEnumerable<mdlSearchResponse>> SearchFlight(mdlSearchRequest mdlRq);
        Task<mdlSearchResponse> SearchFlightMinPrices(mdlSearchRequest mdlRq);
    }

    public class Booking :  IBooking
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

            mdlSearchResponse searchResponse = new mdlSearchResponse() { ResponseStatus=0, Error= new mdlError() {Code=0, Message="" } };
            var res = (await SearchFlight(mdlRq)).ToList();
            if (res == null)
            {
                searchResponse.Error.Message = "No data found";
                return searchResponse;
            }
            if (res.Count() == 1)
            {
                return res[0];
            }

            return searchResponse;

        }




        public async Task<mdlFareQuotResponse> FareQuote(
             mdlFareQuotRequest mdlRq)
        {
            mdlFareQuotResponse mdlRs = new mdlFareQuotResponse();
            switch (mdlRq.Provider)
            {
                case enmServiceProvider.TBO:
                    //mdlRs =await tekTravel.FareQuoteAsync(mdlRq);
                    break;
                case enmServiceProvider.TripJack:
                    mdlRs = await _tripJack.FareQuoteAsync(mdlRq);
                    break;
            }
            return mdlRs;
        }
        public async Task<mdlFareRuleResponse> FareRule(//[FromServices] ITekTravel tekTravel,
             mdlFareRuleRequest mdlRq)
        {
            mdlFareRuleResponse mdlRs = new mdlFareRuleResponse();
            switch (mdlRq.Provider)
            {
                case enmServiceProvider.TBO:
                    //mdlRs =await tekTravel.FareQuoteAsync(mdlRq);
                    break;
                case enmServiceProvider.TripJack:
                    mdlRs = await _tripJack.FareRuleAsync(mdlRq);
                    break;
            }
            return mdlRs;
        }
        #endregion

    }
}
