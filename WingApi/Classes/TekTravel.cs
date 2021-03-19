using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WingApi.Classes.Database;

namespace WingApi.Classes.TekTravel
{
   

    public class TekTravel : IWing
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        public TekTravel(DBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private mdlError GetResponse(string requestData, string url)
        {
            mdlError mdl = new mdlError();
            mdl.ErrorCode = 1;            
            mdl.Message = string.Empty;
            try
            {
                //var client = new RestClient(url);
                //client.Timeout = -1;
                //var request = new RestRequest(Method.POST);
                //request.AddHeader("Content-Type", "application/json");
                ////request.AddHeader("Accept-Encoding", "gzip");
                //request.AddParameter("application/json", requestData);
                //IRestResponse response = client.Execute(request);
                //if (response.IsSuccessful)
                //{
                //    responseXML = response.Content;
                //}

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
                    mdl.ErrorCode = 0;
                    mdl.Message = readStream.ReadToEnd();//JsonConvert.DeserializeXmlNode(readStream.ReadToEnd(), "root").InnerXml;
                }
                return mdl;
            }
            catch (WebException webEx)
            {
                mdl.ErrorCode = 1;
                //get the response stream
                WebResponse response = webEx.Response;
                Stream stream = response.GetResponseStream();
                String responseMessage = new StreamReader(stream).ReadToEnd();
                mdl.Message = responseMessage;                 
            }
            catch (Exception ex)
            {
                mdl.ErrorCode = 1;
                mdl.Message = ex.Message;
            }
            return mdl;
        }

        private async Task<mdlAuthenticateResponse> LoginAsync()
        {
            mdlAuthenticateResponse mdl = new mdlAuthenticateResponse();
            mdlAuthenticateRequest request = new mdlAuthenticateRequest();
            request.ClientId = _config["TBO:Credential:ClientId"];
            request.UserName = _config["TBO:Credential:UserName"];
            request.Password = _config["TBO:Credential:Password"];
            request.EndUserIp = "::1";
            string tboUrl = _config["TBO:API:Login"];            
            string jsonString = JsonConvert.SerializeObject(request);
            var HaveResponse= GetResponse(jsonString, tboUrl);
            if (HaveResponse.ErrorCode == 0)
            {
                mdl = System.Text.Json.JsonSerializer.Deserialize<mdlAuthenticateResponse>(HaveResponse.Message);
                if (mdl.Status == 1)
                {
                    _context.tblTboTokenDetails.Add(new tblTboTokenDetails() { TokenId = mdl.TokenId, AgencyId = mdl.Member.AgencyId.ToString(), MemberId = mdl.Member.MemberId.ToString(), GenrationDt = DateTime.Now });
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                mdl.Error = new Classes.Error()
                {
                    ErrorCode = 1,
                    ErrorMessage = "Invalid Login",
                };
            }
            
            return mdl;
        }

        public async Task<mdlSearchResponse> SearchAsync(mdlSearchRequest request)
        {
            mdlSearchResponse response = null;
            if (request.JourneyType == enmJourneyType.OneWay)//only Journey TYpe is one way then Fetch from DB else Fetch from tbo
            {
                response = SearchFromDb(request);
                if (response == null)//no data found in Db
                {
                    response = await SearchFromTboAsync(request);
                }
            }
            else
            {
                response = await SearchFromTboAsync(request);
            }
            return response;
        }

        private async Task<bool> Search_SaveAsync(mdlSearchRequest request, string _TokenId, string _TraceId, SearchResult[] response)
        {

            int ExpirationMinute = 14;
            int.TryParse(_config["TBO:TraceIdExpiryTime"], out ExpirationMinute);
            double minFare = 0;
            if (response != null && response.Length > 0)
            {
                minFare = response.Min(p => p.Fare.PublishedFare);
            }
            DateTime TickGeration = DateTime.Now;

            tblTboTravelDetail td = new tblTboTravelDetail()
            {
                TravelDate = request.Segments[0].TravelDt,
                CabinClass = request.Segments[0].FlightCabinClass,
                Origin = request.Segments[0].Origin,
                Destination = request.Segments[0].Destination,
                TokenId = _TokenId,
                TraceId = _TraceId,
                MinPublishFare = minFare,
                JourneyType = request.JourneyType,
                AdultCount = request.AdultCount,
                ChildCount = request.ChildCount,
                InfantCount = request.InfantCount,
                GenrationDt = TickGeration,
                ExpireDt = TickGeration.AddMinutes(ExpirationMinute),
                tblTboTravelDetailResult = response.Select(p => new tblTboTravelDetailResult { ResultIndex = p.ResultIndex, ResultType = (p.ResultIndex.Contains("OB") ? "OB" : "IB"), OfferedFare = p.Fare.OfferedFare, PublishedFare = p.Fare.PublishedFare, JsonData = System.Text.Json.JsonSerializer.Serialize(p) }).ToList(),
            };
            _context.tblTboTravelDetail.Add(td);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<mdlSearchResponse> SearchFromTboAsync(mdlSearchRequest request)
        {

            int MaxLoginAttempt = 1, LoginAttempt = 0;
            int.TryParse(_config["TBO:MaxLoginAttempt"], out MaxLoginAttempt);
            mdlSearchResponse mdlS = null;            
            SearchResponse mdl = null;
            SearchResponseWraper mdlTemp = null;
            string tboUrl = _config["TBO:API:Search"];

            StartSendRequest:
            //Load tokken ID 
            var TokenDetails = _context.tblTboTokenDetails.OrderByDescending(p => p.GenrationDt).FirstOrDefault();
            if (TokenDetails == null)
            {
                var AuthenticateResponse = await LoginAsync();
                if (AuthenticateResponse.Status == 1 && LoginAttempt < MaxLoginAttempt)
                {
                    LoginAttempt++;
                    goto StartSendRequest;
                }
            }
            string jsonString = System.Text.Json.JsonSerializer.Serialize(SearchRequestMap( request, TokenDetails.TokenId));
            var HaveResponse = GetResponse(jsonString, tboUrl);
            if (HaveResponse.ErrorCode == 0)
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
                            ErrorCode = mdl.Error.ErrorCode,
                            ErrorMessage = mdl.Error.ErrorMessage,
                        }
                    };
                }
                else if (mdl.ResponseStatus == 1)//success
                {
                    var tempdata = mdl.Results.SelectMany(p => p).ToArray();
                    var result = Search_SaveAsync(request, request.TokenId, mdl.TraceId, tempdata);
                    List<mdlSearchResult[]> AllResults = new List<mdlSearchResult[]>();
                    List<mdlSearchResult> ResultOB = new List<mdlSearchResult>();
                    List<mdlSearchResult> ResultIB = new List<mdlSearchResult>();
                    foreach (var dt in tempdata)
                    {
                        if (dt.ResultIndex.Contains("OB"))
                        {
                            ResultOB.Add(SearchResultMap(dt));
                        }
                        else
                        {
                            ResultIB.Add(SearchResultMap(dt));
                        }
                    }
                    AllResults.Add(ResultOB.ToArray());
                    AllResults.Add(ResultIB.ToArray());

                    mdlS = new mdlSearchResponse()
                    {
                        ServiceProvider = enmServiceProvider.TBO,
                        TraceId = mdl.TraceId,
                        ResponseStatus = 1,
                        Error = new mdlError()
                        {
                            ErrorCode = 0,
                            ErrorMessage = "-"
                        },
                        Origin = mdl.Origin,
                        Destination = mdl.Destination,
                        Results = AllResults.ToArray()
                    };
                    await result;
                }
                else
                {
                    mdlS = new mdlSearchResponse()
                    {
                        ResponseStatus = 3,
                        Error = new mdlError()
                        {
                            ErrorCode = mdl.Error.ErrorCode,
                            ErrorMessage = mdl.Error.ErrorMessage,
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
                        ErrorCode = 100,
                        ErrorMessage = "Unable to Process",
                    }
                };
            }

            return mdlS;
        }

        private mdlSearchResult SearchResultMap(SearchResult sr)
        {
            mdlSearchResult mdl = new mdlSearchResult();
            mdl.IsHoldAllowedWithSSR = sr.IsHoldAllowedWithSSR;
            mdl.ResultIndex = sr.ResultIndex;
            mdl.Source = sr.Source;
            mdl.IsLCC = sr.IsLCC;
            mdl.IsRefundable = sr.IsRefundable;
            mdl.IsPanRequiredAtBook = sr.IsPanRequiredAtBook;
            mdl.IsPanRequiredAtTicket = sr.IsPanRequiredAtTicket;
            mdl.IsPassportRequiredAtBook = sr.IsPassportRequiredAtBook;
            mdl.IsPassportRequiredAtTicket = sr.IsPassportRequiredAtTicket;
            mdl.GSTAllowed = sr.GSTAllowed;
            mdl.IsCouponAppilcable = sr.IsCouponAppilcable;
            mdl.IsGSTMandatory = sr.IsGSTMandatory;
            mdl.AirlineRemark = sr.AirlineRemark;
            mdl.ResultFareType = sr.ResultFareType;
            mdl.TicketAdvisory = sr.TicketAdvisory;
            mdl.AirlineCode = sr.AirlineCode;
            mdl.ValidatingAirline = sr.ValidatingAirline;
            mdl.IsUpsellAllowed = sr.IsUpsellAllowed;
            //mdl.LastTicketDate = sr.LastTicketDate;
            mdl.Fare = new mdlFare();
            if (sr.Fare != null)
            {
                mdl.Fare.Currency = sr.Fare.Currency;
                mdl.Fare.BaseFare = sr.Fare.BaseFare;
                mdl.Fare.Tax = sr.Fare.Tax;
                mdl.Fare.TaxBreakup = sr.Fare.TaxBreakup.Select(p => new mdlTaxbreakup { key = p.key, value = p.value }).ToArray();
                mdl.Fare.YQTax = sr.Fare.YQTax;
                mdl.Fare.AdditionalTxnFeeOfrd = sr.Fare.AdditionalTxnFeeOfrd;
                mdl.Fare.AdditionalTxnFeePub = sr.Fare.AdditionalTxnFeePub;
                mdl.Fare.PGCharge = sr.Fare.PGCharge;
                mdl.Fare.OtherCharges = sr.Fare.OtherCharges;
                mdl.Fare.ChargeBU = sr.Fare.ChargeBU.Select(p => new mdlChargebu { key = p.key, value = p.value }).ToArray();
                mdl.Fare.Discount = sr.Fare.Discount;
                mdl.Fare.PublishedFare = sr.Fare.PublishedFare;
                mdl.Fare.CommissionEarned = sr.Fare.CommissionEarned;
                mdl.Fare.PLBEarned = sr.Fare.PLBEarned;
                mdl.Fare.IncentiveEarned = sr.Fare.IncentiveEarned;
                mdl.Fare.OfferedFare = sr.Fare.OfferedFare;
                mdl.Fare.TdsOnCommission = sr.Fare.TdsOnCommission;
                mdl.Fare.TdsOnPLB = sr.Fare.TdsOnPLB;
                mdl.Fare.TdsOnIncentive = sr.Fare.TdsOnIncentive;
                mdl.Fare.ServiceFee = sr.Fare.ServiceFee;
                mdl.Fare.TotalBaggageCharges = sr.Fare.TotalBaggageCharges;
                mdl.Fare.TotalMealCharges = sr.Fare.TotalMealCharges;
                mdl.Fare.TotalSeatCharges = sr.Fare.TotalSeatCharges;
                mdl.Fare.TotalSpecialServiceCharges = sr.Fare.TotalSpecialServiceCharges;
            }

            mdl.FareBreakdown = sr.FareBreakdown.Select(p => new mdlFarebreakdown
            {
                Currency = p.Currency,
                PassengerType = p.PassengerType,
                PassengerCount = p.PassengerCount,
                BaseFare = p.BaseFare,
                Tax = p.Tax,
                YQTax = p.YQTax,
                AdditionalTxnFeeOfrd = p.AdditionalTxnFeeOfrd,
                AdditionalTxnFeePub = p.AdditionalTxnFeePub,
                PGCharge = p.PGCharge,
                SupplierReissueCharges = p.SupplierReissueCharges,
                //TaxBreakUp = p.TaxBreakUp.Select(q => new mdlTaxbreakup { key = q.key, value = q.value }).ToArray()
            }).ToArray();

            List<mdlSegmentResponse[]> SegmentsResponse = new List<mdlSegmentResponse[]>();
            foreach (var sg in sr.Segments)
            {
                SegmentsResponse.Add(sg.Select(p => new mdlSegmentResponse
                {
                    Baggage = p.Baggage,
                    CabinBaggage = p.CabinBaggage,
                    CabinClass = p.CabinClass,
                    TripIndicator = p.TripIndicator,
                    SegmentIndicator = p.SegmentIndicator,
                    Airline = new mdlAirline()
                    {
                        AirlineCode = p.Airline.AirlineCode,
                        AirlineName = p.Airline.AirlineName,
                        FlightNumber = p.Airline.FlightNumber,
                        FareClass = p.Airline.FareClass,
                        OperatingCarrier = p.Airline.OperatingCarrier,
                    },
                    NoOfSeatAvailable = p.NoOfSeatAvailable,
                    Origin = new mdlOrigin()
                    {
                        Airport = new mdlAirport()
                        {
                            AirportCode = p.Origin.Airport.AirportCode,
                            AirportName = p.Origin.Airport.AirportName,
                            Terminal = p.Origin.Airport.Terminal,
                            CityCode = p.Origin.Airport.CityCode,
                            CityName = p.Origin.Airport.CityName,
                            CountryCode = p.Origin.Airport.CountryCode,
                            CountryName = p.Origin.Airport.CountryName,
                        },
                        DepTime = p.Origin.DepTime
                    },
                    Destination = new mdlDestination()
                    {
                        Airport = new mdlAirport()
                        {
                            AirportCode = p.Destination.Airport.AirportCode,
                            AirportName = p.Destination.Airport.AirportName,
                            Terminal = p.Destination.Airport.Terminal,
                            CityCode = p.Destination.Airport.CityCode,
                            CityName = p.Destination.Airport.CityName,
                            CountryCode = p.Destination.Airport.CountryCode,
                            CountryName = p.Destination.Airport.CountryName,
                        },
                        ArrTime = p.Destination.ArrTime
                    },
                    Duration = p.Duration,
                    GroundTime = p.GroundTime,
                    Mile = p.Mile,
                    StopOver = p.StopOver,
                    FlightInfoIndex = p.FlightInfoIndex,
                    StopPoint = p.StopPoint,
                    StopPointArrivalTime = p.StopPointArrivalTime,
                    StopPointDepartureTime = p.StopPointDepartureTime,
                    Craft = p.Craft,
                    Remark = p.Remark,
                    IsETicketEligible = p.IsETicketEligible,
                    FlightStatus = p.FlightStatus,
                    Status = p.Status,
                    AccumulatedDuration = p.AccumulatedDuration
                }).ToArray());

            }
            mdl.Segments = SegmentsResponse.ToArray();
            mdl.FareRules = sr.FareRules.Select(p => new mdlFarerule
            {
                Origin = p.Origin,
                Destination = p.Destination,
                Airline = p.Airline,
                FareBasisCode = p.FareBasisCode,
                FareRuleDetail = p.FareRuleDetail,
                FareRestriction = p.FareRestriction,
                FareFamilyCode = p.FareFamilyCode,
                FareRuleIndex = p.FareRuleIndex
            }).ToArray();
            if (sr.PenaltyCharges != null)
            {
                mdl.PenaltyCharges = new mdlPenaltycharges()
                {
                    ReissueCharge = sr.PenaltyCharges.ReissueCharge,
                    CancellationCharge = sr.PenaltyCharges.CancellationCharge
                };
            }

            return mdl;


        }

        private DateTime PreferredTimeConversion(enmPreferredDepartureTime enm,DateTime travelDate)
        {
            switch (enm)
            {
                case enmPreferredDepartureTime.Morning:
                    travelDate = travelDate.AddHours(8);
                    break;
                case enmPreferredDepartureTime.AfterNoon:
                    travelDate = travelDate.AddHours(14);
                    break;
                case enmPreferredDepartureTime.Evening:
                    travelDate = travelDate.AddHours(19);
                    break;
            }
            return travelDate;
        }

        private SearchRequest SearchRequestMap(mdlSearchRequest request, string TokenId) {

            SegmentRequest[] sr= request.Segments.Select(p => new SegmentRequest { Origin = p.Origin, Destination = p.Destination, FlightCabinClass = p.FlightCabinClass,
                PreferredDepartureTime = PreferredTimeConversion(p.PreferredDeparture, p.TravelDt),
                PreferredArrivalTime = PreferredTimeConversion(p.PreferredArrival, p.TravelDt)
            }).ToArray();
            SearchRequest mdl = new SearchRequest()
            {
                EndUserIp = request.EndUserIp,
                TokenId = TokenId,
                AdultCount = request.AdultCount,
                ChildCount = request.ChildCount,
                InfantCount = request.InfantCount,
                DirectFlight = request.DirectFlight,
                JourneyType = request.JourneyType,
                PreferredAirlines = request.PreferredAirlines,
                Segments = sr,
                Sources = request.Sources
            };
            return mdl;
        }

        private mdlSearchResponse SearchFromDb(mdlSearchRequest request)
        {
            mdlSearchResponse mdlSearchResponse = null;
            DateTime CurrentTime = DateTime.Now;
            tblTboTravelDetail Data = null;
            if (request.JourneyType == enmJourneyType.OneWay)
            {
                Data = _context.tblTboTravelDetail.Where(p => p.Origin == request.Segments[0].Origin && p.Destination == request.Segments[0].Destination
                  && request.AdultCount == p.AdultCount && p.ChildCount == request.ChildCount && p.InfantCount == request.InfantCount && p.CabinClass == request.Segments[0].FlightCabinClass
                  && p.TravelDate == request.Segments[0].TravelDt
                  && p.ExpireDt > CurrentTime
                ).Include(p => p.tblTboTravelDetailResult).OrderByDescending(p => p.ExpireDt).FirstOrDefault();
                if (Data != null)
                {
                    List<mdlSearchResult[]> AllResults = new List<mdlSearchResult[]>();

                    List<mdlSearchResult> ResultOB = new List<mdlSearchResult>();
                    List<mdlSearchResult> ResultIB = new List<mdlSearchResult>();
                    foreach (var dt in Data.tblTboTravelDetailResult)
                    {
                        if (dt.ResultType == "OB")
                        {
                            ResultOB.Add(SearchResultMap(System.Text.Json.JsonSerializer.Deserialize<SearchResult>(dt.JsonData)));
                        }
                        else
                        {
                            ResultIB.Add(SearchResultMap(System.Text.Json.JsonSerializer.Deserialize<SearchResult>(dt.JsonData)));
                        }

                    }
                    AllResults.Add(ResultOB.ToArray());
                    AllResults.Add(ResultIB.ToArray());
                    mdlSearchResponse = new mdlSearchResponse()
                    {
                        ServiceProvider = enmServiceProvider.TBO,
                        TraceId = Data.TraceId.ToString(),
                        ResponseStatus = 1,
                        Error = new mdlError()
                        {
                            ErrorCode = 0,
                            ErrorMessage = "-"
                        },
                        Origin = Data.Origin,
                        Destination = Data.Destination,
                        Results = AllResults.ToArray()

                    };

                }
            }


            return mdlSearchResponse;




        }


        #region ************************* Search Classes ***************************




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
            public string Origin { get; set; }
            public string Destination { get; set; }
            public string Airline { get; set; }
            public string FareBasisCode { get; set; }
            public string FareRuleDetail { get; set; }
            public string FareRestriction { get; set; }
            public string FareFamilyCode { get; set; }
            public string FareRuleIndex { get; set; }
        }

        #endregion




    }
}
