using B2BApis.Model;
using B2BClasses;
using B2BClasses.Database;
using B2BClasses.Models;
using B2BClasses.Services.Air;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace B2BApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class HomeController : ControllerBase
    {
        IConfiguration _config;
        int _customerId;
        int _userId;
        int isvalid = 0;

        public HomeController(IConfiguration configuration)
        {
            _config = configuration;
        }

        [Route("SearchAirPort")]
        public async Task<mdlAirportApi> SearchAirPorts([FromServices] IBooking booking)
        {
            mdlAirportApi airportApi = new mdlAirportApi();
            if (ModelState.IsValid)
            {
                try
                {
                    isvalid = ValidateToken();
                    if (isvalid == 0 || _customerId <= 0 || _userId <= 0)
                    {
                        airportApi.StatusCode = 0;
                        airportApi.StatusMessage = "Invalid Token Data";
                    }
                    else
                    {
                        booking.CustomerId = _customerId;
                        booking.UserId = _userId;
                        airportApi.tblAirports = await booking.GetAirportAsync();
                        airportApi.StatusCode = 1;
                        airportApi.StatusMessage = "Success";
                        airportApi.DefaultFromAirport = "DEL";
                        airportApi.DefaultToAirport = "BOM";
                    }
                }
                catch (Exception ex)
                {
                    airportApi.tblAirports = null;
                    airportApi.StatusCode = 0;
                    airportApi.StatusMessage = ex.Message;
                }
            }
            return airportApi;

        }

        

        [Route("SearchFlight")]
        [HttpPost]
        public async Task<mdlBookingSearchApi> SearchFlight([FromServices] IBooking booking, mdlSearchRequest mdl)
        {
            mdlBookingSearchApi bookingsearch = new mdlBookingSearchApi();

            if (ModelState.IsValid)
            {
                try
                { 
                    isvalid = ValidateToken();
                    if (isvalid == 0 || _customerId<=0 || _userId<=0)
                    {
                        bookingsearch.StatusCode = 0;
                        bookingsearch.StatusMessage = "Invalid Token Data";
                    }
                    else
                    {
                        booking.CustomerId = _customerId;
                        booking.UserId = _userId;
                        bookingsearch.mdlSearches = await booking.SearchFlight(mdl);
                        bookingsearch.StatusCode = 1;
                        bookingsearch.StatusMessage = "Success";
                    }
                }
                catch (Exception ex)
                {
                    bookingsearch.mdlSearches = null;
                    bookingsearch.StatusCode = 0;
                    bookingsearch.StatusMessage = ex.Message;
                }
            }
            return bookingsearch;

        }

        [Route("FareQuote")]
        [HttpPost]
        public async Task<mdlfarequoteApi> FareQuote([FromServices] IBooking booking, mdlFareQuotRequest mdl)
        {
            mdlfarequoteApi mdlfarequote = new mdlfarequoteApi();

            if (ModelState.IsValid)
            {
                try
                {
                    isvalid = ValidateToken();
                    if (isvalid == 0 || _customerId <= 0 || _userId <= 0)
                    {
                        mdlfarequote.StatusCode = 0;
                        mdlfarequote.StatusMessage = "Invalid Token Data";
                    }
                    else
                    {
                        booking.CustomerId = _customerId;
                        booking.UserId = _userId;
                        mdlfarequote.mdlQuote = await booking.FareQuoteAsync(mdl);
                        mdlfarequote.StatusCode = 1;
                        mdlfarequote.StatusMessage = "Success";
                    }
                }
                catch (Exception ex)
                {
                    mdlfarequote.StatusCode = 0;
                    mdlfarequote.StatusMessage = ex.Message;
                }
            }
            return mdlfarequote;

        }

        [Route("FareRule")]
        [HttpPost]
        public async Task<mdlfareruleApi> FareRule([FromServices] IBooking booking, mdlFareRuleRequest mdl)
        {
            mdlfareruleApi mdlfarerule = new mdlfareruleApi();

            if (ModelState.IsValid)
            {
                try
                {
                    isvalid = ValidateToken();
                    if (isvalid == 0 || _customerId <= 0 || _userId <= 0)
                    {
                        mdlfarerule.StatusCode = 0;
                        mdlfarerule.StatusMessage = "Invalid Token Data";
                    }
                    else
                    {
                        booking.CustomerId = _customerId;
                        booking.UserId = _userId;
                        mdlfarerule.mdlFare = await booking.FareRule(mdl);
                        mdlfarerule.StatusCode = 1;
                        mdlfarerule.StatusMessage = "Success";
                    }
                }
                catch (Exception ex)
                {
                    mdlfarerule.StatusCode = 0;
                    mdlfarerule.StatusMessage = ex.Message;
                }
            }
            return mdlfarerule;

        }


        [Route("Booking")]
        [HttpPost]
        public async Task<mdlbooking> Booking([FromServices] IBooking booking, mdlBookingRequest mdl)
        {
            mdlbooking mdlbook = new mdlbooking();

            if (ModelState.IsValid)
            {
                try
                {
                    isvalid = ValidateToken();
                    if (isvalid == 0 || _customerId <= 0 || _userId <= 0)
                    {
                        mdlbook.StatusCode = 0;
                        mdlbook.StatusMessage = "Invalid Token Data";
                    }
                    else
                    {
                        booking.CustomerId = _customerId;
                        booking.UserId = _userId;
                        mdlbook.mdlBooking = await booking.BookingAsync(mdl);
                        mdlbook.StatusCode = 1;
                        mdlbook.StatusMessage = "Success";
                    }
                }
                catch (Exception ex)
                {
                    mdlbook.StatusCode = 0;
                    mdlbook.StatusMessage = ex.Message;
                }
            }
            return mdlbook;

        }


        private int ValidateToken()
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                string authHeader = Request.Headers["Authorization"];
                authHeader = authHeader.Replace("Bearer ", "");
                var jsonToken = handler.ReadToken(authHeader);
                var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
                _customerId = Convert.ToInt32( tokenS.Claims.First(claim => claim.Type == "__customerId").Value);
                _userId = Convert.ToInt32(tokenS.Claims.First(claim => claim.Type == "__UserId").Value);
                isvalid = 1;
            }
            catch {
                isvalid = 0;
            }
            return isvalid;
        }
    }
}
