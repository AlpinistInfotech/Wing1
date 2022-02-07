using B2C.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2C.Classes
{
    public interface IFlightSearch
    {
        mdlAirportWraper GetAirport(bool IsDomestic, bool IsActive, string Token);
        mdlSearchResponse Search(mdlFlightSearchRequest mdl, string Token);
    }

    public class FlightSearch : IFlightSearch
    {
        private readonly IServerApi _IServerApi;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _memoryCache;
        
        public FlightSearch(IServerApi IServerApi, IConfiguration config, IMemoryCache memoryCache)
        {
            _IServerApi = IServerApi;
            _config = config;
            _memoryCache = memoryCache;
        }
        public mdlAirportWraper GetAirport(bool IsDomestic, bool IsActive, string Token)
        {
            //mdlAirportWraper returnData = new mdlAirportWraper() { messageType = enmMessageType.None };
            string cacheKey =_config["Caching:GetAirport:Name"];
            int AbsoluteExpiration = 3600, SlidingExpiration=3600;
            int.TryParse(_config["Caching:GetAirport:AbsoluteExpiration"], out AbsoluteExpiration);
            int.TryParse(_config["Caching:GetAirport:SlidingExpiration"], out SlidingExpiration);

            if (!_memoryCache.TryGetValue(cacheKey, out mdlAirportWraper returnData))
            {
                var responseData = _IServerApi.GetResponsePostMethod("", string.Format("Air/GetAirport/{0}/{1}", IsActive, IsDomestic), Token, "GET");
                if (responseData.MessageType == enmMessageType.Success)
                {
                    returnData = JsonConvert.DeserializeObject<mdlAirportWraper>(responseData.Message);
                    if (returnData.messageType == enmMessageType.Success)
                    {
                        var cacheExpiryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddSeconds(AbsoluteExpiration),
                            Priority = CacheItemPriority.High,
                            SlidingExpiration = TimeSpan.FromSeconds(SlidingExpiration)
                        };
                        _memoryCache.Set(cacheKey, returnData, cacheExpiryOptions);
                    }
                    
                }
                else
                {
                    throw new Exception(responseData.Message);
                }

            }
            
            return returnData;

        }

        public mdlSearchResponse Search(mdlFlightSearchRequest mdl,string Token)
        {
            
            string cacheKey =string.Concat( _config["Caching:FlightSearch:Name"],mdl.Orign,mdl.Destination,mdl.TravelDt,mdl.ReturnDt,mdl.CabinClass,mdl.JourneyType,mdl.AdultCount,mdl.ChildCount,mdl.InfantCount);
            int AbsoluteExpiration = 3600, SlidingExpiration = 3600;
            int.TryParse(_config["Caching:FlightSearch:AbsoluteExpiration"], out AbsoluteExpiration);
            int.TryParse(_config["Caching:FlightSearch:SlidingExpiration"], out SlidingExpiration);
            if (!_memoryCache.TryGetValue(cacheKey, out mdlSearchResponse returnData))
            {
                string jsonString = JsonConvert.SerializeObject(mdl);
                var responseData = _IServerApi.GetResponsePostMethod(jsonString, string.Format("Air/SearchFlight/{0}", _config["OrgCode"]), Token, "POST");
                if (responseData.MessageType == enmMessageType.Success)
                { 
                    var tempData=  JsonConvert.DeserializeObject<mdlSearchResponseWraper>(responseData.Message);
                    if (tempData.messageType == enmMessageType.Success)
                    {
                        returnData = tempData.returnId;
                        if (returnData.ResponseStatus == enmMessageType.Success)
                        {
                            var cacheExpiryOptions = new MemoryCacheEntryOptions
                            {
                                AbsoluteExpiration = DateTime.Now.AddSeconds(AbsoluteExpiration),
                                Priority = CacheItemPriority.Normal,
                                SlidingExpiration = TimeSpan.FromSeconds(SlidingExpiration)
                            };
                            _memoryCache.Set(cacheKey, returnData, cacheExpiryOptions);
                        }
                    }
                    else
                    {
                        returnData = new mdlSearchResponse()
                        {
                            ResponseStatus = tempData.messageType,
                            Error = new mdlError()
                            {
                                Code = 1,
                                Message = tempData.message
                            }
                        };
                    }
                    
                    
                    
                }
                else
                {
                    throw new Exception(responseData.Message);
                }
            }
            return returnData;
        }
    }
}
