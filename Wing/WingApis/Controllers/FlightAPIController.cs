
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using static WingApis.ResponseAPI;

namespace WingApis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightAPIController : ControllerBase
    {
        IConfiguration _config;
        int _customerId;
        int _userId;
        int isvalid = 0;

        public FlightAPIController(IConfiguration configuration)
        {
            _config = configuration;
        }

        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    string[] testdata = { "prabhakar", "ram", "Mohan" };
        //    return testdata;
        //}
        [Route("SearchAirPort")]
        //[HttpPost]
        public async Task<mdlResponsedata> SearchAirPorts()
        {
            mdlResponsedata data = new mdlResponsedata();

            if (ModelState.IsValid)
            {

                try
                {
                    string url = _config["FlightAPI:SearchAirPort"];
                    string tokendata = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJfX2N1c3RvbWVySWQiOiIxIiwiX19Vc2VySWQiOiIxMSIsImV4cCI6MTYyNDU1NTc0NywiaXNzIjoid2luZy5jb20iLCJhdWQiOiJiYWJhcmFtLmNvbSJ9.rx1v1ycsLk0X1L_HUJ-j_ynPROeKdf3plWYHN7J17Ns";
                    ResponseAPI aPI = new ResponseAPI();
                    data = await aPI.GetResponse("", url, tokendata);
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
