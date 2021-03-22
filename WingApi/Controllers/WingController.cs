using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WingApi.Classes;
using WingApi.Classes.Database;
using WingApi.Classes.TekTravel;

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
        public async Task<ActionResult<IEnumerable<mdlSearchResponse>>> GettblCountryMaster(mdlSearchRequest mdlRq)
        {
            List<mdlSearchResponse> mdlRs = new List<mdlSearchResponse>();
            IWing wing1 = new TekTravel(_context, _config);
            mdlRs.Add(await wing1.SearchAsync(mdlRq));
            IWing wing2 = new TripJack(_context, _config);
            mdlRs.Add(await wing2.SearchAsync(mdlRq));
            return mdlRs;
        }

    }
}
