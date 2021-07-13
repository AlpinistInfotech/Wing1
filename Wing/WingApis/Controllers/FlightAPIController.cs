
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WingApis.ResponseAPI;

namespace WingApis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightAPIController : ControllerBase
    {
        IConfiguration _config;
        public FlightAPIController(IConfiguration configuration)
        {
            _config = configuration;
        }

        [Route("Authenticate")]
        [HttpPost]
        public async Task<mdlResponsedata> Authenticate()
        {
            return await CallAPIStandard(_config["FlightAPI:Authenticate"].ToString());
        }

        [Route("SearchAirPort")]
        [HttpPost]
        public async Task<mdlResponsedata> SearchAirPorts()
        {   
          return await CallAPIStandard(_config["FlightAPI:SearchAirPort"].ToString());
        }

        [Route("SearchFlight")]
        [HttpPost]
        public async Task<mdlResponsedata> SearchFlight()
        {
            return await CallAPIStandard(_config["FlightAPI:SearchFlight"].ToString());
        }


        [Route("FareQuote")]
        [HttpPost]
        public async Task<mdlResponsedata> FareQuote()
        {
            return await CallAPIStandard(_config["FlightAPI:FareQuote"].ToString());
        }


        [Route("FareRule")]
        [HttpPost]
        public async Task<mdlResponsedata> FareRule()
        {
            return await CallAPIStandard(_config["FlightAPI:FareRule"].ToString());
        }

        [Route("Book")]
        [HttpPost]
        public async Task<mdlResponsedata> Book()
        {
            return await CallAPIStandard(_config["FlightAPI:Book"].ToString());
        }

        public async Task<mdlResponsedata> CallAPIStandard(string url)
        {
            mdlResponsedata data = new mdlResponsedata();
            ResponseAPI aPI = new ResponseAPI();
            if (ModelState.IsValid)
            {
                try
                {
                    string tokendata = Request.Headers["Authorization"];
                    string bodycontent = "";

                    using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                    {
                        bodycontent = await reader.ReadToEndAsync();
                    }

                    data = await aPI.GetResponse(url, bodycontent, tokendata);
                }
                catch (Exception ex)
                {
                    data.Code = 1;
                    data.Message = ex.Message.ToString();
                }
            }
            return data;
        }
    }
}
