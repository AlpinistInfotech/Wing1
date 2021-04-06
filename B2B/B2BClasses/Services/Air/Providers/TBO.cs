using B2BClasses.Database;
using B2BClasses.Services.Enums;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
    public interface ITBO //: IWing
    {
    }
    public class TBO: ITBO
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
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
            var HaveResponse = GetResponse(jsonString, tboUrl);
            if (HaveResponse.Code == 0)
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
                mdl.Error = new Error()
                {
                    ErrorCode= 1,
                    ErrorMessage = "Invalid Login",
                };
            }

            return mdl;
        }

        public Task<mdlSearchResponse> SearchAsync(mdlSearchRequest request)
        {
            mdlSearchResponse response = null;
            //if (request.JourneyType == enmJourneyType.OneWay)//only Journey TYpe is one way then Fetch from DB else Fetch from tbo
            //{
            //    response = SearchFromDb(request);
            //    if (response == null)//no data found in Db
            //    {
            //        response = await SearchFromTboAsync(request);
            //    }
            //}
            //else
            //{
            //    response = await SearchFromTboAsync(request);
            //}
            return Task.FromResult( response);
        }

        #region ********************* Login Classes ***********************

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


        #region ************************* Search Classes ***************************

        private async Task<bool> Search_SaveAsync(mdlSearchRequest request, string _TokenId, string _TraceId, SearchResult[][] response)
        {

            int ExpirationMinute = 14;
            int.TryParse(_config["TBO:TraceIdExpiryTime"], out ExpirationMinute);
            double minFare = 0,minFareReturn = 0;
            if (response != null && response.Length > 0)
            {
                minFare = response[0].Min(p => p.Fare.PublishedFare);
                if (response.Length > 1)
                {
                    minFareReturn = response[1].Min(p => p.Fare.PublishedFare); 
                }
            }

            List<tblTboTravelDetailResult> tbldetails = new List<tblTboTravelDetailResult>();
            for (int i = 0; i < response.Length; i++)
            {
                tbldetails.AddRange( response[i].Select(p => new tblTboTravelDetailResult { segmentId = i, ResultIndex = p.ResultIndex, ResultType = (p.ResultIndex.Contains("OB") ? "OB" : "IB"), OfferedFare = p.Fare.OfferedFare, PublishedFare = p.Fare.PublishedFare, JsonData = System.Text.Json.JsonSerializer.Serialize(p) }));
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
                MinPublishFareReturn= minFareReturn,
                JourneyType = request.JourneyType,
                AdultCount = request.AdultCount,
                ChildCount = request.ChildCount,
                InfantCount = request.InfantCount,
                GenrationDt = TickGeration,
                ExpireDt = TickGeration.AddMinutes(ExpirationMinute),                
                tblTboTravelDetailResult = tbldetails,
            };
            _context.tblTboTravelDetail.Add(td);
            await _context.SaveChangesAsync();
            return true;
        }


        private DateTime PreferredTimeConversion(enmPreferredDepartureTime enm, DateTime travelDate)
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
        //for One way and Return
        private List<mdlSearchResult> SearchResultMap(List<SearchResult> sr)
        {
            List<mdlSearchResult> mdlSearchResults_ = new List<mdlSearchResult>();
            bool IsRecordExists = false;

            // Add Distinct Segment Coresponding to each Search Result
            foreach (var tempData in sr)
            {
                foreach (var temp in tempData.Segments)
                {
                    IsRecordExists = false;
                    foreach (var tempmdl in mdlSearchResults_.Select(p => p.Segment))
                    {
                        if (tempmdl.Count== temp.Length)
                        {
                            for (int i = 0; i < tempmdl.Count; i++)
                            {
                                if (tempmdl[i].Origin.AirportCode == temp[i].Origin.Airport.AirportCode
                                     && tempmdl[i].Destination.AirportCode == temp[i].Destination.Airport.AirportCode
                                     && tempmdl[i].Airline.Code == temp[i].Airline.AirlineCode
                                     && tempmdl[i].Airline.FlightNumber == temp[i].Airline.FlightNumber
                                     )
                                {
                                    IsRecordExists = true;
                                }
                            }
                        }
                    }
                    if (!IsRecordExists)
                    {
                        mdlSearchResult mdl = new mdlSearchResult();
                        List<mdlTotalpricelist> mdlTotalpricelists = new List<mdlTotalpricelist>();
                        mdl.TotalPriceList = mdlTotalpricelists;

                        mdl.Segment = temp.Select(p => new mdlSegment
                        {
                            Airline = new mdlAirline() { Code = p.Airline.AirlineCode, FlightNumber = p.Airline.FlightNumber, isLcc = tempData.IsLCC, Name = p.Airline.AirlineName, OperatingCarrier = p.Airline.OperatingCarrier },
                            ArrivalTime = p.Destination.ArrTime,
                            DepartureTime = p.Origin.DepTime,
                            Destination = new mdlAirport()
                            {
                                AirportCode = p.Destination.Airport.AirportCode,
                                AirportName = p.Destination.Airport.AirportName,
                                CityCode = p.Destination.Airport.CityCode,
                                CityName = p.Destination.Airport.CityName,
                                CountryCode = p.Destination.Airport.CountryCode,
                                CountryName = p.Destination.Airport.CountryName,
                                Terminal = p.Destination.Airport.Terminal
                            },
                            Origin = new mdlAirport()
                            {
                                AirportCode = p.Origin.Airport.AirportCode,
                                AirportName = p.Origin.Airport.AirportName,
                                CityCode = p.Origin.Airport.CityCode,
                                CityName = p.Origin.Airport.CityName,
                                CountryCode = p.Origin.Airport.CountryCode,
                                CountryName = p.Origin.Airport.CountryName,
                                Terminal = p.Origin.Airport.Terminal
                            },
                            Duration = p.Duration,
                            Mile = p.Mile,
                            TripIndicator = p.TripIndicator
                        }).ToList();
                        mdlSearchResults_.Add(mdl);
                    }
                }
            }
            //Add Fare Coresponding to Fare Result
            for(int i=0;i< mdlSearchResults_.Count;i++)
            {
                foreach (var tempData in sr)
                {
                    foreach (var temp in tempData.Segments)
                    {
                        if (mdlSearchResults_[i].Segment.Count == temp.Length)
                        {
                            
                            for (int j = 0; j < mdlSearchResults_[i].Segment.Count; j++)
                            {
                                if (mdlSearchResults_[i].Segment[j].Origin.AirportCode == temp[i].Origin.Airport.AirportCode
                                     && mdlSearchResults_[i].Segment[j].Destination.AirportCode == temp[i].Destination.Airport.AirportCode
                                     && mdlSearchResults_[i].Segment[j].Airline.Code == temp[i].Airline.AirlineCode
                                     && mdlSearchResults_[i].Segment[j].Airline.FlightNumber == temp[i].Airline.FlightNumber
                                     )
                                {

                                    var adt = tempData.FareBreakdown.FirstOrDefault(p => p.PassengerType == enmPassengerType.Adult);
                                    var chd = tempData.FareBreakdown.FirstOrDefault(p => p.PassengerType == enmPassengerType.Child);
                                    var inft = tempData.FareBreakdown.FirstOrDefault(p => p.PassengerType == enmPassengerType.Infant);
                                    mdlPassenger Adult = null;
                                    mdlPassenger Child = new mdlPassenger();
                                    mdlPassenger Infant = new mdlPassenger();
                                    if (adt != null)
                                    {
                                        Adult = new mdlPassenger();
                                        Adult.CabinClass = temp[i].CabinClass;
                                        Adult.ClassOfBooking = temp[i].Airline.FareClass;
                                        Adult.BaggageInformation=new mdlBaggageInformation() { 
                                            CabinBaggage= temp[i].CabinBaggage ,
                                            CheckingBaggage= temp[i].Baggage
                                        } ;
                                        Adult.FareBasis= tempData.FareRules.FirstOrDefault()?.FareBasisCode;
                                        Adult.RefundableType = (!tempData.IsRefundable) ? 0: 2;
                                        Adult.SeatRemaing = temp[i].NoOfSeatAvailable;
                                        Adult.IsFreeMeel = false;
                                        Adult.FareComponent = new mdlFareComponent()
                                        {
                                            BaseFare = adt.BaseFare,
                                            IGST = 0,
                                            TaxAndFees = adt.Tax + adt.PGCharge + adt.AdditionalTxnFeeOfrd + adt.AdditionalTxnFeePub,
                                            NetCommission = 0,
                                            
                                        };
                                        Adult.FareComponent.TotalFare = Adult.FareComponent.BaseFare + Adult.FareComponent.TaxAndFees;
                                        Adult.FareComponent.NetFare = Adult.FareComponent.TotalFare;
                                    }
                                    if (chd != null)
                                    {
                                        Child = new mdlPassenger();
                                        Child.CabinClass = temp[i].CabinClass;
                                        Child.ClassOfBooking = temp[i].Airline.FareClass;
                                        Child.BaggageInformation = new mdlBaggageInformation()
                                        {
                                            CabinBaggage = temp[i].CabinBaggage,
                                            CheckingBaggage = temp[i].Baggage
                                        };
                                        Child.FareBasis = tempData.FareRules.FirstOrDefault()?.FareBasisCode;
                                        Child.RefundableType = (!tempData.IsRefundable) ? 0 : 2;
                                        Child.SeatRemaing = temp[i].NoOfSeatAvailable;
                                        Child.IsFreeMeel = false;
                                        Child.FareComponent = new mdlFareComponent()
                                        {
                                            BaseFare = chd.BaseFare,
                                            IGST = 0,
                                            TaxAndFees = chd.Tax + chd.PGCharge + chd.AdditionalTxnFeeOfrd + chd.AdditionalTxnFeePub,
                                            NetCommission = 0,

                                        };
                                        Child.FareComponent.TotalFare = Child.FareComponent.BaseFare + Child.FareComponent.TaxAndFees;
                                        Child.FareComponent.NetFare = Child.FareComponent.TotalFare;
                                    }
                                    if (chd != null)
                                    {
                                        Infant = new mdlPassenger();
                                        Infant.CabinClass = temp[i].CabinClass;
                                        Infant.ClassOfBooking = temp[i].Airline.FareClass;
                                        Infant.BaggageInformation = new mdlBaggageInformation()
                                        {
                                            CabinBaggage = string.Empty,
                                            CheckingBaggage = string.Empty
                                        };
                                        Infant.FareBasis = tempData.FareRules.FirstOrDefault()?.FareBasisCode;
                                        Infant.RefundableType = (!tempData.IsRefundable) ? 0 : 2;
                                        Infant.SeatRemaing = temp[i].NoOfSeatAvailable;
                                        Infant.IsFreeMeel = false;
                                        Infant.FareComponent = new mdlFareComponent()
                                        {
                                            BaseFare = inft.BaseFare,
                                            IGST = 0,
                                            TaxAndFees = inft.Tax + inft.PGCharge + inft.AdditionalTxnFeeOfrd + inft.AdditionalTxnFeePub,
                                            NetCommission = 0,

                                        };
                                        Infant.FareComponent.TotalFare = Infant.FareComponent.BaseFare + Infant.FareComponent.TaxAndFees;
                                        Infant.FareComponent.NetFare = Infant.FareComponent.TotalFare;
                                    }

                                    mdlSearchResults_[i].TotalPriceList.Add(new mdlTotalpricelist()
                                    {  
                                        ADULT= Adult,
                                        CHILD=Child,
                                        INFANT=Infant,
                                        fareIdentifier=tempData.ResultFareType,
                                        ResultIndex= tempData.ResultIndex,                                        

                                    });


                                }
                            }
                        }
                    }
                }
            }
            return mdlSearchResults_;
        }

        private SearchRequest SearchRequestMap(mdlSearchRequest request, string TokenId)
        {

            SegmentRequest[] sr = request.Segments.Select(p => new SegmentRequest
            {
                Origin = p.Origin,
                Destination = p.Destination,
                FlightCabinClass = p.FlightCabinClass,
                PreferredDepartureTime = PreferredTimeConversion(p.PreferredDeparture, p.TravelDt),
                PreferredArrivalTime = PreferredTimeConversion(p.PreferredDeparture > p.PreferredArrival ? p.PreferredDeparture : p.PreferredArrival, p.TravelDt)
            }).ToArray();
            SearchRequest mdl = new SearchRequest()
            {
                EndUserIp = "::1",
                TokenId = TokenId,
                AdultCount = request.AdultCount,
                ChildCount = request.ChildCount,
                InfantCount = request.InfantCount,
                DirectFlight = request.DirectFlight,
                JourneyType = request.JourneyType,
                PreferredAirlines = request.PreferredAirlines,
                Segments = sr,                
            };
            return mdl;
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
            string jsonString = System.Text.Json.JsonSerializer.Serialize(SearchRequestMap(request, TokenDetails.TokenId));
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




    }
}
