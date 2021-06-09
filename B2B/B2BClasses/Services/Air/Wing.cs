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
    public interface IWingFlight
    {
        Task<mdlSearchResponse> SearchAsync(mdlSearchRequest request, int CustomerId);
        Task<mdlFareQuotResponse> FareQuoteAsync(mdlFareQuotRequest request);
        Task<mdlFareRuleResponse> FareRuleAsync(mdlFareRuleRequest request);
        Task<mdlBookingResponse> BookingAsync(mdlBookingRequest request);
    }

    //public class WingFlight
    //{
    //    private readonly DBContext _context;
    //    private readonly IConfiguration _config;
    //    private readonly ITripJack _tripJack;
    //    public WingFlight(DBContext context, IConfiguration config, ITripJack tripJack)
    //    {
    //        _context = context;
    //        _config = config;
    //        _tripJack = tripJack;
    //    }
    //}
}
