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
                    var cacheExpiryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddSeconds(AbsoluteExpiration),
                        Priority = CacheItemPriority.High,
                        SlidingExpiration = TimeSpan.FromSeconds(SlidingExpiration)
                    };
                    _memoryCache.Set(cacheKey, returnData, cacheExpiryOptions);
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
            mdlSearchResponse returnData = new mdlSearchResponse();
            string jsonString = JsonConvert.SerializeObject(mdl);
            var responseData = _IServerApi.GetResponsePostMethod(jsonString, string.Format("Air/SearchFlight/{0}", _config["OrgCode"]), Token, "POST");
            if (responseData.MessageType == enmMessageType.Success)
            {
                returnData = JsonConvert.DeserializeObject<mdlSearchResponse>(responseData.Message);
            }
            else
            {
                throw new Exception(responseData.Message);
            }
            return returnData;
        }
    }
}
