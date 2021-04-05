using B2BClasses.Database;
using B2BClasses.Services.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace B2BClasses.Services.Air
{
   
    public interface IWing
    {
        Task<mdlSearchResponse> SearchAsync(mdlSearchRequest request);
        Task<mdlFareQuotResponse> FareQuoteAsync(mdlFareQuotRequest request);
        Task<mdlFareRuleResponse> FareRuleAsync(mdlFareRuleRequest request);
        Task<mdlBookingResponse> BookingAsync(mdlBookingRequest request);
    }

    public class Wing
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        private readonly ITripJack _tripJack;
        public Wing(DBContext context, IConfiguration config, ITripJack tripJack)
        {
            _context = context;
            _config = config;
            _tripJack = tripJack;
        }

        public async Task<IEnumerable<mdlSearchResponse>> SearchFlight(
          mdlSearchRequest mdlRq)
        {
            List<mdlSearchResponse> mdlRs = new List<mdlSearchResponse>();
            //var mdlTekTravel = tekTravel.SearchAsync(mdlRq) ;
            var mdlTripJack = _tripJack.SearchAsync(mdlRq);
            //mdlRs.Add(await mdlTekTravel);            
            mdlRs.Add(await mdlTripJack);
            return mdlRs;
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



    }



}
