using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WingApi.Models;

namespace WingApi.Classes
{


    public class mdlAuthenticateRequest
    {
        public string ClientId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EndUserIp { get; set; }
    }


    public class mdlAuthenticateResponse
    {
        public int Status { get; set; }
        public string TokenId { get; set; }
        public Error Error { get; set; }
        public Member Member { get; set; }
    }

    public class Error
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class Member
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int MemberId { get; set; }
        public int AgencyId { get; set; }
        public string LoginName { get; set; }
        public string LoginDetails { get; set; }
        public bool isPrimaryAgent { get; set; }
    }




    public interface IWing
    {
        Task<mdlSearchResponse> SearchAsync(mdlSearchRequest request);
        Task<mdlFareQuotResponse> FareQuoteAsync(mdlFareQuotRequest request);
        Task<mdlFareRuleResponse> FareRuleAsync(mdlFareRuleRequest request);
        Task<mdlBookingResponse> BookingAsync(mdlBookingRequest request);
    }
}
