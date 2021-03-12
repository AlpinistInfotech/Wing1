using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WingApi.Classes
{


    public enum enmCabinClass
    {
        //ALL=1,
        Economy=2,
        PremiumEconomy=3,
        Business=4,
        //PremiumBusiness=5,
        First=6
    }
    public enum enmPreferredDepartureTime
    {
        AnyTime=1,
        Morning=2,
        AfterNoon=3,
        Evening=4,
        Night=5
    }

    public class AuthenticateRequestModel
    {
        public string ClinetId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EndUserIp { get; set; }
    }

    public class ApiError
    { 
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class AuthenticateResponse
    {
        public string  UserName { get; set; }
        public ApiError Error { get; set; }
        public string TokenId { get; set; }
        public int Status { get; set; }
    }

    public class mdlRouteInfo
    { 
        public string FromClity { get; set; }
        public string ToClity { get; set; }
        public DateTime TravelDate { get; set; }        
    }

    public class mdlSearchRequest
    {   
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public int InfantCount { get; set; }
        public bool DirectFlight { get; set; }
        public enmCabinClass CabinClass { get; set; }
        public List<mdlRouteInfo> RouteInfo { get; set; }
        public enmPreferredDepartureTime PreferredDepartureTime { get; set; }
        public List<string> PreferredAirlines { get; set; }
    }

    public class mdlSearchResponse
    {
        public string TraceId { get; set; }
    }

    public interface IWing
    {
        public AuthenticateResponse Login();
        
    }
}
