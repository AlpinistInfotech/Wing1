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
        Task<mdlFareQuotResponse> FareQuote(mdlFareQuotRequest mdlRq);
        Task<mdlFareRuleResponse> FareRule(mdlFareRuleRequest mdlRq);
        Task<List<tblAirline>> GetAirlinesAsync();
        Task<List<tblAirport>> GetAirportAsync();
        Task<IEnumerable<mdlSearchResponse>> SearchFlight(mdlSearchRequest mdlRq);
    }

    public class Booking : IBooking
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        private readonly IWingFlight _tripJack;

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
        #endregion

    }
}
