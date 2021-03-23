using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WingApi.Classes;
using WingApi.Classes.Database;
using WingApi.Classes.TekTravel;
using WingApi.Classes.TripJack;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WingController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        public WingController(DBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [HttpPost]
        [Route("Search")]
        public async Task<ActionResult<IEnumerable<mdlSearchResponse>>> SearchFlight([FromServices] ITekTravel tekTravel,
            [FromServices] ITripJack tripJack, mdlSearchRequest mdlRq)
        {
            List<mdlSearchResponse> mdlRs = new List<mdlSearchResponse>();
            var mdlTekTravel = tekTravel.SearchAsync(mdlRq) ;
            var mdlTripJack = tripJack.SearchAsync(mdlRq);
            mdlRs.Add(await mdlTekTravel);            
            mdlRs.Add(await mdlTripJack);
            return mdlRs;
        }

        [HttpPost]
        [Route("FareQuote")]
        public async Task<ActionResult<mdlFareQuotResponse>> FareQuote([FromServices] ITekTravel tekTravel,
            [FromServices] ITripJack tripJack, mdlFareQuotRequest mdlRq)
        {
            mdlFareQuotResponse mdlRs = new mdlFareQuotResponse();
            switch (mdlRq.Provider) 
            {
                case enmServiceProvider.TBO:
                    mdlRs =await tekTravel.FareQuoteAsync(mdlRq);
                    break;
                case enmServiceProvider.TripJack:
                    mdlRs = await tripJack.FareQuoteAsync(mdlRq);
                    break;
            }
            return mdlRs;
        }

    }
}
