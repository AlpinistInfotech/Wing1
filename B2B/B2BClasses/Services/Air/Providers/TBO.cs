using B2BClasses.Database;
using B2BClasses.Services.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace B2BClasses.Services.Air
{
    public interface ITBO  :IWingFlight
    {
    }

    public class TBOAuthenticateResponse
    {   
        public string TokenId { get; set; }
        public int MemberId { get; set; }
        public int AgencyId { get; set; }
        public DateTime TokenLoginTime { get; set; }
        public DateTime TokenExpiryTime { get; set; }

    }
    
    public class TBO : ITBO
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;

        public static TBOAuthenticateResponse _loginDetails;

        public TBO(DBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }


        private mdlError GetResponse(string requestData, string url)
        {
            mdlError mdl = new mdlError();
            mdl.Code = 1;
            mdl.Message = string.Empty;
            try
            {
                
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Accept-Encoding", "gzip");
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
                WebResponse webResponse = request.GetResponse();
                var rsp = webResponse.GetResponseStream();
                if (rsp == null)
                {
                    mdl.Message = "No Response Found";
                    throw new Exception("No Response Found");
                }
                using (StreamReader readStream = new StreamReader(new GZipStream(rsp, CompressionMode.Decompress)))
                {
                    mdl.Code = 0;
                    mdl.Message = readStream.ReadToEnd();//JsonConvert.DeserializeXmlNode(readStream.ReadToEnd(), "root").InnerXml;
                }
                return mdl;
            }
            catch (WebException webEx)
            {
                mdl.Code = 1;
                //get the response stream
                WebResponse response = webEx.Response;
                Stream stream = response.GetResponseStream();
                String responseMessage = new StreamReader(stream).ReadToEnd();
                mdl.Message = responseMessage;
            }
            catch (Exception ex)
            {
                mdl.Code = 1;
                mdl.Message = ex.Message;
            }
            return mdl;
        }

        


        public async Task<mdlSearchResponse> SearchAsync(mdlSearchRequest request, int CustomerId)
        {


            mdlSearchResponse response = null;
            #if (false)

            if (request.JourneyType == enmJourneyType.OneWay)
            {
                response = SearchFromDb(request);
                if (response == null)
                {
                    response = await SearchFromTboAsync(request);
                }
            }
            else if (request.JourneyType == enmJourneyType.Return ||
                request.JourneyType == enmJourneyType.MultiStop)
            {
                request.JourneyType = enmJourneyType.OneWay;
                var lst = request.Segments.ToList();
                for (int i = 0; i < lst.Count; i++)
                {
                    request.Segments = new List<mdlSegmentRequest>();
                    request.Segments.Add(lst[i]);
                    mdlSearchResponse tempRe = SearchFromDb(request);
                    if (tempRe == null)
                    {
                        tempRe = await SearchFromTboAsync(request);
                    }

                    if (response == null)
                    {
                        response = tempRe;
                    }
                    else
                    {
                        if (tempRe.Results != null)
                        {
                            response.Results.Add(tempRe.Results.FirstOrDefault());
                        }
                    }
                }
            }
            //Add Provider Previx in Result index
            int tboId = (int)enmServiceProvider.TBO;
            response.Results?.ForEach(p =>
            {
                p.ForEach(q => q.TotalPriceList.ForEach(r => r.ResultIndex = "" + tboId + "_" + r.ResultIndex));
            });


        #endif
            return response;
        }


       
#region ********************* Login Classes ***********************


        private mdlAuthenticateResponse LoginAsync()
        {
            mdlAuthenticateResponse mdl = new mdlAuthenticateResponse();
            bool LoadData = false;
            if (_loginDetails == null)
            {
                _loginDetails = new TBOAuthenticateResponse();
                LoadData = true;
            }
            if ((DateTime.Compare(TBO._loginDetails.TokenExpiryTime, DateTime.Now) <= 0) || (DateTime.Now.Day - _loginDetails.TokenLoginTime.Day > 0))
            {
                LoadData = true;
            }
            if (LoadData)
            {
                int TokenExpiryMinute = 240;
                int.TryParse(_config["TBO:Credential:TokenExpiryMinute"], out TokenExpiryMinute);
                mdlAuthenticateRequest request = new mdlAuthenticateRequest();
                request.ClientId = _config["TBO:Credential:ClientId"];
                request.UserName = _config["TBO:Credential:UserName"];
                request.Password = _config["TBO:Credential:Password"];
                request.EndUserIp = "::1";
                string tboUrl = _config["TBO:API:Login"];
                string jsonString = JsonConvert.SerializeObject(request);
                var HaveResponse = GetResponse(jsonString, tboUrl);
                if (HaveResponse.Code == 0)
                {
                    mdl = System.Text.Json.JsonSerializer.Deserialize<mdlAuthenticateResponse>(HaveResponse.Message);
                    if (mdl.Status == 1)
                    {
                        _loginDetails.TokenId = mdl.TokenId;
                        _loginDetails.AgencyId = mdl.Member.AgencyId;
                        _loginDetails.MemberId = mdl.Member.MemberId;
                        _loginDetails.TokenId = mdl.TokenId;
                        _loginDetails.TokenExpiryTime = DateTime.Now.AddMinutes(TokenExpiryMinute);
                        _loginDetails.TokenLoginTime = DateTime.Now;
                    }
                }
                else
                {
                    mdl.Error = new Error()
                    {
                        //errCode = "1",
                        //message = "Invalid Login",
                    };
                }

            }
            else
            {
                mdl.Status = 1;
                mdl.TokenId = _loginDetails.TokenId;
                mdl.Member = new Member
                {
                    AgencyId = _loginDetails.AgencyId,
                    MemberId = _loginDetails.MemberId,
                };

            }


            return mdl;
        }

        public Task<mdlFareQuotResponse> FareQuoteAsync(mdlFareQuotRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<mdlFareRuleResponse> FareRuleAsync(mdlFareRuleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<mdlBookingResponse> BookingAsync(mdlBookingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<mdlFlightCancellationChargeResponse> CancelationChargeAsync(mdlCancellationRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<mdlFlightCancellationResponse> CancellationAsync(mdlCancellationRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<mdlCancelationDetails> CancelationDetailsAsync(string request)
        {
            throw new NotImplementedException();
        }

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
#endregion


#if (false)

#region *******************Search Function***************************

        private mdlSearchResponse SearchFromDb(mdlSearchRequest request)
        {
            throw new NotImplementedException();
            //mdlSearchResponse mdlSearchResponse = null;
            //DateTime CurrentTime = DateTime.Now;
            //tblTboTravelDetail Data = null;
            //if (request.JourneyType == enmJourneyType.OneWay ||
            //    request.JourneyType == enmJourneyType.Return ||
            //    request.JourneyType == enmJourneyType.SpecialReturn
            //    )
            //{
            //    Data = _context.tblTboTravelDetail.Where(p => p.Origin == request.Segments[0].Origin && p.Destination == request.Segments[0].Destination
            //      && request.AdultCount == p.AdultCount && p.ChildCount == request.ChildCount && p.InfantCount == request.InfantCount && p.CabinClass == request.Segments[0].FlightCabinClass
            //      && p.TravelDate == request.Segments[0].TravelDt
            //      && p.ExpireDt > CurrentTime
            //    ).Include(p => p.tblTboTravelDetailResult).OrderByDescending(p => p.ExpireDt).FirstOrDefault();
            //    if (Data != null)
            //    {
            //        List<List<mdlSearchResult>> AllResults = new List<List<mdlSearchResult>>();
            //        var disSegIds = Data.tblTboTravelDetailResult.Select(p => p.segmentId).Distinct().OrderBy(p => p);
            //        foreach (var d in disSegIds)
            //        {
            //            AllResults.Add(
            //               SearchResultMap(Data.tblTboTravelDetailResult.Where(p => p.segmentId == d).Select(p => JsonConvert.DeserializeObject<ONWARD_RETURN_COMBO>(p.JsonData)).ToArray()));
            //        }

            //        mdlSearchResponse = new mdlSearchResponse()
            //        {
            //            ServiceProvider = enmServiceProvider.TBO,
            //            TraceId = Data.TraceId.ToString(),
            //            ResponseStatus = 1,
            //            Error = new mdlError()
            //            {
            //                Code = 0,
            //                Message = "-"
            //            },
            //            Origin = Data.Origin,
            //            Destination = Data.Destination,
            //            Results = AllResults

            //        };

            //    }
            //}


            //return mdlSearchResponse;




        }

        private SearchqueryWraper SearchRequestMap(mdlSearchRequest request)
        {
            enmCabinClass enmCabin = request.Segments[0].FlightCabinClass;
            List<Routeinfo> routeinfos = new List<Routeinfo>();
            for (int i = 0; i < request.Segments.Count(); i++)
            {
                if (i > 0 && (request.JourneyType == enmJourneyType.OneWay || request.JourneyType == enmJourneyType.AdvanceSearch))
                {
                    break;
                }
                if (i > 1 && (request.JourneyType == enmJourneyType.Return || request.JourneyType == enmJourneyType.SpecialReturn))
                {
                    break;
                }
                Routeinfo routeinfo = new Routeinfo()
                {
                    fromCityOrAirport = new cityorairport()
                    {
                        code = request.Segments[i].Origin
                    },
                    toCityOrAirport = new cityorairport()
                    {
                        code = request.Segments[i].Destination
                    },
                    travelDate = request.Segments[i].TravelDt.ToString("yyyy-MM-dd"),
                };
                routeinfos.Add(routeinfo);
            }



            Searchquery mdl = new Searchquery()
            {
                cabinClass = enmCabin.ToString(),
                paxInfo = new Paxinfo()
                {
                    ADULT = request.AdultCount,
                    CHILD = request.ChildCount,
                    INFANT = request.InfantCount
                },
                routeInfos = routeinfos.ToArray(),
                searchModifiers = new Searchmodifiers()
                {
                    isDirectFlight = true,
                    isConnectingFlight = true
                },

            };

            if (request.PreferredAirlines != null)
            {
                mdl.preferredAirline = request.PreferredAirlines.Select(p => new cityorairport { code = p }).ToArray();
            }

            SearchqueryWraper mdlW = new SearchqueryWraper()
            {
                searchQuery = mdl
            };
            return mdlW;
        }

        private async Task<mdlSearchResponse> SearchFromTboAsync(mdlSearchRequest request)
        {

            int MaxLoginAttempt = 1, LoginAttempt = 0;
            int.TryParse(_config["TBO:MaxLoginAttempt"], out MaxLoginAttempt);
            mdlSearchResponse mdlS = null;
            SearchResponse mdl = null;
            SearchResponseWraper mdlTemp = null;
            string tboUrl = _config["TBO:API:Search"];
            string jsonString = System.Text.Json.JsonSerializer.Serialize(SearchRequestMap(request, LoginAsync()));
            var HaveResponse = GetResponse(jsonString, tboUrl);
            if (HaveResponse.Code == 0)
            {
                mdlTemp = (System.Text.Json.JsonSerializer.Deserialize<SearchResponseWraper>(HaveResponse.Message));
                if (mdlTemp != null)
                {
                    mdl = mdlTemp.Response;
                }
            }

            if (mdl != null)
            {
                if (mdl.ResponseStatus == 3 && LoginAttempt < MaxLoginAttempt)//failure
                {
                    LoginAttempt++;
                    var AuthenticateResponse = await LoginAsync();
                    if (AuthenticateResponse.Status == 1)
                    {
                        goto StartSendRequest;
                    }
                    mdlS = new mdlSearchResponse()
                    {
                        ResponseStatus = 3,
                        Error = new mdlError()
                        {
                            Code = mdl.Error.ErrorCode,
                            Message = mdl.Error.ErrorMessage,
                        }
                    };
                }
                else if (mdl.ResponseStatus == 1)//success
                {


                    //var result = Search_SaveAsync(request, TokenDetails.TokenId, mdl.TraceId, mdl.Results);

                    //List<mdlSearchResult[]> AllResults = new List<mdlSearchResult[]>();
                    //List<mdlSearchResult> ResultOB = new List<mdlSearchResult>();
                    //List<mdlSearchResult> ResultIB = new List<mdlSearchResult>();
                    //foreach (var dt in tempdata)
                    //{
                    //    if (dt.ResultIndex.Contains("OB"))
                    //    {
                    //        ResultOB.Add(SearchResultMap(dt, "OB"));
                    //    }
                    //    else if (dt.ResultIndex.Contains("IB"))
                    //    {
                    //        ResultIB.Add(SearchResultMap(dt, "IB"));
                    //    }
                    //    else
                    //    {
                    //        ResultOB.Add(SearchResultMap(dt, "OB"));
                    //    }
                    //}
                    //if (ResultOB.Count() > 0)
                    //{
                    //    AllResults.Add(ResultOB.ToArray());
                    //}
                    //if (ResultIB.Count() > 0)
                    //{
                    //    AllResults.Add(ResultIB.ToArray());
                    //}



                    mdlS = new mdlSearchResponse()
                    {
                        ServiceProvider = enmServiceProvider.TBO,
                        TraceId = mdl.TraceId,
                        ResponseStatus = 1,
                        Error = new mdlError()
                        {
                            Code = 0,
                            Message = "-"
                        },
                        Origin = mdl.Origin,
                        Destination = mdl.Destination,

                    };
                    //await result;
                }
                else
                {
                    mdlS = new mdlSearchResponse()
                    {
                        ResponseStatus = 3,
                        Error = new mdlError()
                        {
                            Code = mdl.Error.ErrorCode,
                            Message = mdl.Error.ErrorMessage,
                        }
                    };
                }

            }
            else
            {
                mdlS = new mdlSearchResponse()
                {
                    ResponseStatus = 100,
                    Error = new mdlError()
                    {
                        Code = 100,
                        Message = "Unable to Process",
                    }
                };
            }

            return mdlS;
        }


#region  ********************** Search Classes **************************
        public class SearchRequest
        {
            public string EndUserIp { get; set; }
            public string TokenId { get; set; }
            public int AdultCount { get; set; }
            public int ChildCount { get; set; }
            public int InfantCount { get; set; }
            public bool DirectFlight { get; set; }
            //public string OneStopFlight { get; set; }
            public enmJourneyType JourneyType { get; set; }
            public string[] PreferredAirlines { get; set; }
            public SegmentRequest[] Segments { get; set; }
            public string[] Sources { get; set; }
        }

        public class SegmentRequest
        {
            public string Origin { get; set; }
            public string Destination { get; set; }
            public enmCabinClass FlightCabinClass { get; set; }
            public DateTime PreferredDepartureTime { get; set; }
            public DateTime PreferredArrivalTime { get; set; }
        }


        public class SearchResponseWraper
        {
            public SearchResponse Response { get; set; }
        }

        public class SearchResponse
        {
            public int ResponseStatus { get; set; }
            public Error Error { get; set; }
            public string TraceId { get; set; }
            public string Origin { get; set; }
            public string Destination { get; set; }
            public SearchResult[][] Results { get; set; }
        }

        public class Error
        {
            public int ErrorCode { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class SearchResult
        {
            public bool IsHoldAllowedWithSSR { get; set; }
            public string ResultIndex { get; set; }
            public int Source { get; set; }
            public bool IsLCC { get; set; }
            public bool IsRefundable { get; set; }
            public bool IsPanRequiredAtBook { get; set; }
            public bool IsPanRequiredAtTicket { get; set; }
            public bool IsPassportRequiredAtBook { get; set; }
            public bool IsPassportRequiredAtTicket { get; set; }
            public bool GSTAllowed { get; set; }
            public bool IsCouponAppilcable { get; set; }
            public bool IsGSTMandatory { get; set; }
            public string AirlineRemark { get; set; }
            public string ResultFareType { get; set; }
            public Fare Fare { get; set; }
            public Farebreakdown[] FareBreakdown { get; set; }
            public SegmentResponse[][] Segments { get; set; }
            //public DateTime LastTicketDate { get; set; }
            public string TicketAdvisory { get; set; }
            public Farerule[] FareRules { get; set; }
            public string AirlineCode { get; set; }
            public string ValidatingAirline { get; set; }
            public bool IsUpsellAllowed { get; set; }
            public Penaltycharges PenaltyCharges { get; set; }
        }

        public class Fare
        {
            public string Currency { get; set; }
            public double BaseFare { get; set; }
            public double Tax { get; set; }
            public Taxbreakup[] TaxBreakup { get; set; }
            public double YQTax { get; set; }
            public double AdditionalTxnFeeOfrd { get; set; }
            public double AdditionalTxnFeePub { get; set; }
            public double PGCharge { get; set; }
            public double OtherCharges { get; set; }
            public Chargebu[] ChargeBU { get; set; }
            public double Discount { get; set; }
            public double PublishedFare { get; set; }
            public double CommissionEarned { get; set; }
            public double PLBEarned { get; set; }
            public double IncentiveEarned { get; set; }
            public double OfferedFare { get; set; }
            public double TdsOnCommission { get; set; }
            public double TdsOnPLB { get; set; }
            public double TdsOnIncentive { get; set; }
            public double ServiceFee { get; set; }
            public double TotalBaggageCharges { get; set; }
            public double TotalMealCharges { get; set; }
            public double TotalSeatCharges { get; set; }
            public double TotalSpecialServiceCharges { get; set; }
        }

        public class Taxbreakup
        {
            public string key { get; set; }
            public double value { get; set; }
        }

        public class Chargebu
        {
            public string key { get; set; }
            public double value { get; set; }
        }

        public class Penaltycharges
        {
            public dynamic ReissueCharge { get; set; }
            public dynamic CancellationCharge { get; set; }
        }

        public class Farebreakdown
        {
            public string Currency { get; set; }
            public enmPassengerType PassengerType { get; set; }
            public int PassengerCount { get; set; }
            public double BaseFare { get; set; }
            public double Tax { get; set; }
            public Taxbreakup[] TaxBreakUp { get; set; }
            public double YQTax { get; set; }
            public double AdditionalTxnFeeOfrd { get; set; }
            public double AdditionalTxnFeePub { get; set; }
            public double PGCharge { get; set; }
            public double SupplierReissueCharges { get; set; }
        }


        public class SegmentResponse
        {
            public string Baggage { get; set; }
            public string CabinBaggage { get; set; }
            public enmCabinClass CabinClass { get; set; }
            public int TripIndicator { get; set; }
            public int SegmentIndicator { get; set; }
            public Airline Airline { get; set; }
            public int NoOfSeatAvailable { get; set; }
            public Origin Origin { get; set; }
            public Destination Destination { get; set; }
            public int Duration { get; set; }
            public int GroundTime { get; set; }
            public int Mile { get; set; }
            public bool StopOver { get; set; }
            public string FlightInfoIndex { get; set; }
            public string StopPoint { get; set; }
            public DateTime? StopPointArrivalTime { get; set; }
            public DateTime? StopPointDepartureTime { get; set; }
            public string Craft { get; set; }
            public string Remark { get; set; }
            public bool IsETicketEligible { get; set; }
            public string FlightStatus { get; set; }
            public string Status { get; set; }
            public int AccumulatedDuration { get; set; }
        }

        public class Airline
        {
            public string AirlineCode { get; set; }
            public string AirlineName { get; set; }
            public string FlightNumber { get; set; }
            public string FareClass { get; set; }
            public string OperatingCarrier { get; set; }
        }

        public class Origin
        {
            public Airport Airport { get; set; }
            public DateTime DepTime { get; set; }
        }

        public class Airport
        {
            public string AirportCode { get; set; }
            public string AirportName { get; set; }
            public string Terminal { get; set; }
            public string CityCode { get; set; }
            public string CityName { get; set; }
            public string CountryCode { get; set; }
            public string CountryName { get; set; }
        }

        public class Destination
        {
            public Airport Airport { get; set; }
            public DateTime ArrTime { get; set; }
        }

        public class Farerule
        {

            public int FlightId { get; set; }
            public string Origin { get; set; }
            public string Destination { get; set; }
            public string Airline { get; set; }
            public string FareBasisCode { get; set; }
            public string FareRuleDetail { get; set; }
            public string FareRestriction { get; set; }
            public string FareFamilyCode { get; set; }
            public string FareRuleIndex { get; set; }
            public DateTime DepartureTime { get; set; }
            public DateTime ReturnDate { get; set; }
        }




#endregion

#endregion

#endif






    }
}
