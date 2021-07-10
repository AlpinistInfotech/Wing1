﻿
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
    [Route("api/[controller]")]
    [ApiController]

    public class FlightAPI : ControllerBase
    {
        IConfiguration _config;
        int _customerId;
        int _userId;
        int isvalid = 0;

        public FlightAPI(IConfiguration configuration)
        {
            _config = configuration;
        }


        [Route("SearchAirPort")]
        [HttpPost]
        public mdlResponsedata SearchAirPorts()
        {
            mdlResponsedata data = new mdlResponsedata();

            if (ModelState.IsValid)
            {

                try
                {
                    string url = _config["FlightAPI:SearchAirPort"];
                    string tokendata = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJfX2N1c3RvbWVySWQiOiIxIiwiX19Vc2VySWQiOiIxMSIsImV4cCI6MTYyNDU1NTc0NywiaXNzIjoid2luZy5jb20iLCJhdWQiOiJiYWJhcmFtLmNvbSJ9.rx1v1ycsLk0X1L_HUJ-j_ynPROeKdf3plWYHN7J17Ns";
                    ResponseAPI aPI = new ResponseAPI();
                    data =  aPI.GetResponse("", url, tokendata);
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
