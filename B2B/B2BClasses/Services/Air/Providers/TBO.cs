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
                    errCode= "1",
                    message = "Invalid Login",
                };
            }

            return mdl;
        }

        public class Error
        {
            public string errCode { get; set; }
            public string message { get; set; }
            public string details { get; set; }
        }

        public async Task<mdlSearchResponse> SearchAsync(mdlSearchRequest request, int CustomerId)
        {
            mdlSearchResponse response = null;
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

            return response;
        }

        private mdlSearchResponse SearchFromDb(mdlSearchRequest request)
        {
            mdlSearchResponse mdlSearchResponse = null;
            DateTime CurrentTime = DateTime.Now;
            tblTboTravelDetail Data = null;
            if (request.JourneyType == enmJourneyType.OneWay ||
                request.JourneyType == enmJourneyType.Return ||
                request.JourneyType == enmJourneyType.SpecialReturn
                )
            {
                Data = _context.tblTboTravelDetail.Where(p => p.Origin == request.Segments[0].Origin && p.Destination == request.Segments[0].Destination
                  && request.AdultCount == p.AdultCount && p.ChildCount == request.ChildCount && p.InfantCount == request.InfantCount && p.CabinClass == request.Segments[0].FlightCabinClass
                  && p.TravelDate == request.Segments[0].TravelDt
                  && p.ExpireDt > CurrentTime
                ).Include(p => p.tblTboTravelDetailResult).OrderByDescending(p => p.ExpireDt).FirstOrDefault();
                if (Data != null)
                {
                    List<List<mdlSearchResult>> AllResults = new List<List<mdlSearchResult>>();
                    var disSegIds = Data.tblTboTravelDetailResult.Select(p => p.segmentId).Distinct().OrderBy(p => p);
                    foreach (var d in disSegIds)
                    {
                        AllResults.Add(
                           SearchResultMap(Data.tblTboTravelDetailResult.Where(p => p.segmentId == d).Select(p => JsonConvert.DeserializeObject<ONWARD_RETURN_COMBO>(p.JsonData)).ToArray()));
                    }

                    mdlSearchResponse = new mdlSearchResponse()
                    {
                        ServiceProvider = enmServiceProvider.TBO,
                        TraceId = Data.TraceId.ToString(),
                        ResponseStatus = 1,
                        Error = new mdlError()
                        {
                            Code = 0,
                            Message = "-"
                        },
                        Origin = Data.Origin,
                        Destination = Data.Destination,
                        Results = AllResults

                    };

                }
            }


            return mdlSearchResponse;




        }
        public class ONWARD_RETURN_COMBO
        {
            public Si[] sI { get; set; }
            public Totalpricelist[] totalPriceList { get; set; }
        }

        public class Si
        {
            public Fd fD { get; set; }
            public int stops { get; set; }
            public So[] so { get; set; }
            public int duration { get; set; }
            public int cT { get; set; }
            public Da da { get; set; }
            public Aa aa { get; set; }
            public DateTime dt { get; set; }
            public DateTime at { get; set; }
            public bool iand { get; set; }
            public int sN { get; set; }
            public Ob oB { get; set; }
            public SsrInfo ssrInfo { get; set; }
        }


        public class Fd
        {
            public Ai aI { get; set; }
            public string fN { get; set; }
            public string eT { get; set; }
        }

        public class Ai
        {
            public string code { get; set; }
            public string name { get; set; }
            public bool isLcc { get; set; }
        }

        public class Da
        {
            public string code { get; set; }
            public string name { get; set; }
            public string cityCode { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public string countryCode { get; set; }
            public string terminal { get; set; }
        }

        public class Aa
        {
            public string code { get; set; }
            public string name { get; set; }
            public string cityCode { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public string countryCode { get; set; }
            public string terminal { get; set; }
        }

        public class Ob
        {
            public string code { get; set; }
            public string name { get; set; }
            public bool isLcc { get; set; }
        }

        public class So
        {
            public string code { get; set; }
            public string name { get; set; }
            public string cityCode { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public string countryCode { get; set; }
        }

        public class Totalpricelist
        {
            public Fd1 fd { get; set; }
            public string fareIdentifier { get; set; }
            public string id { get; set; }
            public string messages { get; set; }
            public string[] msri { get; set; }
            public string sri { get; set; }
        }

        public class Fd1
        {
            public Passenger INFANT { get; set; }
            public Passenger CHILD { get; set; }
            public Passenger ADULT { get; set; }
        }

        public class Passenger
        {
            public Fc fC { get; set; }
            public Afc afC { get; set; }
            public int sR { get; set; }
            public Bi bI { get; set; }
            public int rT { get; set; }
            public string cc { get; set; }
            public string cB { get; set; }
            public string fB { get; set; }
            public bool mi { get; set; }

        }


        public class Fc
        {
            public double TAF { get; set; }
            public double NF { get; set; }
            public double BF { get; set; }
            public double TF { get; set; }
            public double IGST { get; set; }
            public double NCM { get; set; }
            public double SSRP { get; set; }

        }

        public class Afc
        {
            public TAF TAF { get; set; }
            public NCM NCM { get; set; }
        }
        public class NCM
        {
            public double TDS { get; set; }
            public double OC { get; set; }
        }
        public class TAF
        {
            public double MF { get; set; }
            public double OT { get; set; }
            public double MFT { get; set; }
            public double AGST { get; set; }
            public double YQ { get; set; }
            public double YR { get; set; }
            public double WO { get; set; }
        }

        public class Bi
        {
            public string iB { get; set; }
            public string cB { get; set; }
        }

        public class Status
        {
            public bool success { get; set; }
            public int httpStatus { get; set; }
        }

        public class SsrInfo
        {
            public SsrServices[] SEAT { get; set; }
            public SsrServices[] MEAL { get; set; }
            public SsrServices[] EXTRASERVICES { get; set; }
        }
        public class SsrServices
        {
            public string code { get; set; }
            public double amount { get; set; }
            public string desc { get; set; }

        }

        public class Alert
        {
            public double oldFare { get; set; }
            public double newFare { get; set; }
            public string type { get; set; }
        }
        public class Pc
        {
            public string code { get; set; }
            public string name { get; set; }
            public bool isLcc { get; set; }
        }
        public class pcs
        {
            public bool pped { get; set; }
            public bool pid { get; set; }
            public bool pm { get; set; }

        }

        public class Conditions
        {
            public pcs pcs { get; set; }
            public object[] ffas { get; set; }
            public bool isa { get; set; }
            public Dob dob { get; set; }
            public bool isBA { get; set; }
            public int st { get; set; }
            public DateTime sct { get; set; }
            public Gst gst { get; set; }
        }
        public class Dob
        {
            public bool adobr { get; set; }
            public bool cdobr { get; set; }
            public bool idobr { get; set; }
        }

        public class Gst
        {
            public bool gstappl { get; set; }
            public bool igm { get; set; }
        }

        public class Totalpriceinfo
        {
            public Totalfaredetail totalFareDetail { get; set; }
        }

        public class Totalfaredetail
        {
            public Fc fC { get; set; }
            public Afc afC { get; set; }
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

        private List<mdlSearchResult> SearchResultMap(ONWARD_RETURN_COMBO[] sr)
        {
            List<mdlSearchResult> mdls = new List<mdlSearchResult>();
            mdls.AddRange(sr.Select(p => new mdlSearchResult
            {
                Segment = p.sI.Select(q => new mdlSegment
                {
                    Airline = new mdlAirline()
                    {
                        Code = q.fD?.aI?.code,
                        Name = q.fD?.aI?.name,
                        isLcc = q.fD?.aI?.isLcc ?? false,
                        FlightNumber = q.fD?.fN ?? string.Empty,
                        OperatingCarrier = q.oB?.code ?? string.Empty,
                    },
                    ArrivalTime = q.at,
                    DepartureTime = q.dt,
                    Duration = q.duration,
                    Mile = 0,
                    TripIndicator = q.sN,
                    Origin = new mdlAirport()
                    {
                        AirportCode = q.da?.code ?? string.Empty,
                        AirportName = q.da?.name ?? string.Empty,
                        CityCode = q.da?.cityCode ?? string.Empty,
                        CityName = q.da?.city ?? string.Empty,
                        CountryCode = q.da?.countryCode ?? string.Empty,
                        CountryName = q.da?.country ?? string.Empty,
                        Terminal = q.da?.terminal ?? string.Empty,
                    },
                    Destination = new mdlAirport()
                    {
                        AirportCode = q.aa?.code ?? string.Empty,
                        AirportName = q.aa?.name ?? string.Empty,
                        CityCode = q.aa?.cityCode ?? string.Empty,
                        CityName = q.aa?.city ?? string.Empty,
                        CountryCode = q.aa?.countryCode ?? string.Empty,
                        CountryName = q.aa?.country ?? string.Empty,
                        Terminal = q.aa?.terminal ?? string.Empty,
                    },

                }).ToList(),
                TotalPriceList = p.totalPriceList.Select(q => new mdlTotalpricelist
                {
                    fareIdentifier = q.fareIdentifier,
                    ResultIndex = q.id,
                    sri = q.sri,
                    msri = q.msri == null ? new List<string>() : q.msri.ToList(),
                    ADULT = new mdlPassenger()
                    {
                        FareComponent = new mdlFareComponent()
                        {
                            BaseFare = q.fd?.ADULT?.fC?.BF ?? 0,
                            IGST = q.fd?.ADULT?.fC?.IGST ?? 0,
                            TaxAndFees = q.fd?.ADULT?.fC?.TAF ?? 0,
                            TotalFare = q.fd?.ADULT?.fC?.TF ?? 0,
                            NetFare = q.fd?.ADULT?.fC?.TF ?? 0,
                        },
                        BaggageInformation = new mdlBaggageInformation()
                        {
                            CabinBaggage = q.fd?.ADULT?.bI?.cB ?? string.Empty,
                            CheckingBaggage = q.fd?.ADULT?.bI?.iB ?? string.Empty
                        },
                        CabinClass = (enmCabinClass)Enum.Parse(typeof(enmCabinClass), q.fd?.ADULT?.cc ?? (nameof(enmCabinClass.ECONOMY)), true),
                        ClassOfBooking = q.fd?.ADULT?.cB ?? string.Empty,
                        FareBasis = q.fd?.ADULT?.fB ?? string.Empty,
                        IsFreeMeel = q.fd?.ADULT?.mi ?? false,
                        RefundableType = q.fd?.ADULT?.rT ?? 0,
                        SeatRemaing = q.fd?.ADULT?.sR ?? 0,
                    },
                    CHILD = new mdlPassenger()
                    {
                        FareComponent = new mdlFareComponent()
                        {
                            BaseFare = q.fd?.CHILD?.fC?.BF ?? 0,
                            IGST = q.fd?.CHILD?.fC?.IGST ?? 0,
                            TaxAndFees = q.fd?.CHILD?.fC?.TAF ?? 0,
                            TotalFare = q.fd?.CHILD?.fC?.TF ?? 0,
                            NetFare = q.fd?.CHILD?.fC?.TF ?? 0,
                        },
                        BaggageInformation = new mdlBaggageInformation()
                        {
                            CabinBaggage = q.fd?.CHILD?.bI?.cB ?? string.Empty,
                            CheckingBaggage = q.fd?.CHILD?.bI?.iB ?? string.Empty
                        },
                        CabinClass = (enmCabinClass)Enum.Parse(typeof(enmCabinClass), q.fd?.CHILD?.cc ?? (nameof(enmCabinClass.ECONOMY)), true),
                        ClassOfBooking = q.fd?.CHILD?.cB ?? string.Empty,
                        FareBasis = q.fd?.CHILD?.fB ?? string.Empty,
                        IsFreeMeel = q.fd?.CHILD?.mi ?? false,
                        RefundableType = q.fd?.CHILD?.rT ?? 0,
                        SeatRemaing = q.fd?.CHILD?.sR ?? 0,
                    },
                    INFANT = new mdlPassenger()
                    {
                        FareComponent = new mdlFareComponent()
                        {
                            BaseFare = q.fd?.INFANT?.fC?.BF ?? 0,
                            IGST = q.fd?.INFANT?.fC?.IGST ?? 0,
                            TaxAndFees = q.fd?.INFANT?.fC?.TAF ?? 0,
                            TotalFare = q.fd?.INFANT?.fC?.TF ?? 0,
                            NetFare = q.fd?.INFANT?.fC?.TF ?? 0,
                        },
                        BaggageInformation = new mdlBaggageInformation()
                        {
                            CabinBaggage = q.fd?.INFANT?.bI?.cB ?? string.Empty,
                            CheckingBaggage = q.fd?.INFANT?.bI?.iB ?? string.Empty
                        },
                        CabinClass = (enmCabinClass)Enum.Parse(typeof(enmCabinClass), q.fd?.INFANT?.cc ?? (nameof(enmCabinClass.ECONOMY)), true),
                        ClassOfBooking = q.fd?.INFANT?.cB ?? string.Empty,
                        FareBasis = q.fd?.INFANT?.fB ?? string.Empty,
                        IsFreeMeel = q.fd?.INFANT?.mi ?? false,
                        RefundableType = q.fd?.INFANT?.rT ?? 0,
                        SeatRemaing = q.fd?.INFANT?.sR ?? 0,
                    },
                }
                   ).ToList()
            }));
            return mdls;
        }


        //for One way and Return
        //private List<mdlSearchResult> SearchResultMap(List<SearchResult> sr)
        //{
        //    List<mdlSearchResult> mdlSearchResults_ = new List<mdlSearchResult>();
        //    bool IsRecordExists = false;

        //    // Add Distinct Segment Coresponding to each Search Result
        //    foreach (var tempData in sr)
        //    {
        //        foreach (var temp in tempData.Segments)
        //        {
        //            IsRecordExists = false;
        //            foreach (var tempmdl in mdlSearchResults_.Select(p => p.Segment))
        //            {
        //                if (tempmdl.Count== temp.Length)
        //                {
        //                    for (int i = 0; i < tempmdl.Count; i++)
        //                    {
        //                        if (tempmdl[i].Origin.AirportCode == temp[i].Origin.Airport.AirportCode
        //                             && tempmdl[i].Destination.AirportCode == temp[i].Destination.Airport.AirportCode
        //                             && tempmdl[i].Airline.Code == temp[i].Airline.AirlineCode
        //                             && tempmdl[i].Airline.FlightNumber == temp[i].Airline.FlightNumber
        //                             )
        //                        {
        //                            IsRecordExists = true;
        //                        }
        //                    }
        //                }
        //            }
        //            if (!IsRecordExists)
        //            {
        //                mdlSearchResult mdl = new mdlSearchResult();
        //                List<mdlTotalpricelist> mdlTotalpricelists = new List<mdlTotalpricelist>();
        //                mdl.TotalPriceList = mdlTotalpricelists;

        //                mdl.Segment = temp.Select(p => new mdlSegment
        //                {
        //                    Airline = new mdlAirline() { Code = p.Airline.AirlineCode, FlightNumber = p.Airline.FlightNumber, isLcc = tempData.IsLCC, Name = p.Airline.AirlineName, OperatingCarrier = p.Airline.OperatingCarrier },
        //                    ArrivalTime = p.Destination.ArrTime,
        //                    DepartureTime = p.Origin.DepTime,
        //                    Destination = new mdlAirport()
        //                    {
        //                        AirportCode = p.Destination.Airport.AirportCode,
        //                        AirportName = p.Destination.Airport.AirportName,
        //                        CityCode = p.Destination.Airport.CityCode,
        //                        CityName = p.Destination.Airport.CityName,
        //                        CountryCode = p.Destination.Airport.CountryCode,
        //                        CountryName = p.Destination.Airport.CountryName,
        //                        Terminal = p.Destination.Airport.Terminal
        //                    },
        //                    Origin = new mdlAirport()
        //                    {
        //                        AirportCode = p.Origin.Airport.AirportCode,
        //                        AirportName = p.Origin.Airport.AirportName,
        //                        CityCode = p.Origin.Airport.CityCode,
        //                        CityName = p.Origin.Airport.CityName,
        //                        CountryCode = p.Origin.Airport.CountryCode,
        //                        CountryName = p.Origin.Airport.CountryName,
        //                        Terminal = p.Origin.Airport.Terminal
        //                    },
        //                    Duration = p.Duration,
        //                    Mile = p.Mile,
        //                    TripIndicator = p.TripIndicator
        //                }).ToList();
        //                mdlSearchResults_.Add(mdl);
        //            }
        //        }
        //    }
        //    //Add Fare Coresponding to Fare Result
        //    for(int i=0;i< mdlSearchResults_.Count;i++)
        //    {
        //        foreach (var tempData in sr)
        //        {
        //            foreach (var temp in tempData.Segments)
        //            {
        //                if (mdlSearchResults_[i].Segment.Count == temp.Length)
        //                {

        //                    for (int j = 0; j < mdlSearchResults_[i].Segment.Count; j++)
        //                    {
        //                        if (mdlSearchResults_[i].Segment[j].Origin.AirportCode == temp[i].Origin.Airport.AirportCode
        //                             && mdlSearchResults_[i].Segment[j].Destination.AirportCode == temp[i].Destination.Airport.AirportCode
        //                             && mdlSearchResults_[i].Segment[j].Airline.Code == temp[i].Airline.AirlineCode
        //                             && mdlSearchResults_[i].Segment[j].Airline.FlightNumber == temp[i].Airline.FlightNumber
        //                             )
        //                        {

        //                            var adt = tempData.FareBreakdown.FirstOrDefault(p => p.PassengerType == enmPassengerType.Adult);
        //                            var chd = tempData.FareBreakdown.FirstOrDefault(p => p.PassengerType == enmPassengerType.Child);
        //                            var inft = tempData.FareBreakdown.FirstOrDefault(p => p.PassengerType == enmPassengerType.Infant);
        //                            mdlPassenger Adult = null;
        //                            mdlPassenger Child = new mdlPassenger();
        //                            mdlPassenger Infant = new mdlPassenger();
        //                            if (adt != null)
        //                            {
        //                                Adult = new mdlPassenger();
        //                                Adult.CabinClass = temp[i].CabinClass;
        //                                Adult.ClassOfBooking = temp[i].Airline.FareClass;
        //                                Adult.BaggageInformation=new mdlBaggageInformation() { 
        //                                    CabinBaggage= temp[i].CabinBaggage ,
        //                                    CheckingBaggage= temp[i].Baggage
        //                                } ;
        //                                Adult.FareBasis= tempData.FareRules.FirstOrDefault()?.FareBasisCode;
        //                                Adult.RefundableType = (!tempData.IsRefundable) ? 0: 2;
        //                                Adult.SeatRemaing = temp[i].NoOfSeatAvailable;
        //                                Adult.IsFreeMeel = false;
        //                                Adult.FareComponent = new mdlFareComponent()
        //                                {
        //                                    BaseFare = adt.BaseFare,
        //                                    IGST = 0,
        //                                    TaxAndFees = adt.Tax + adt.PGCharge + adt.AdditionalTxnFeeOfrd + adt.AdditionalTxnFeePub,
        //                                    NetCommission = 0,

        //                                };
        //                                Adult.FareComponent.TotalFare = Adult.FareComponent.BaseFare + Adult.FareComponent.TaxAndFees;
        //                                Adult.FareComponent.NetFare = Adult.FareComponent.TotalFare;
        //                            }
        //                            if (chd != null)
        //                            {
        //                                Child = new mdlPassenger();
        //                                Child.CabinClass = temp[i].CabinClass;
        //                                Child.ClassOfBooking = temp[i].Airline.FareClass;
        //                                Child.BaggageInformation = new mdlBaggageInformation()
        //                                {
        //                                    CabinBaggage = temp[i].CabinBaggage,
        //                                    CheckingBaggage = temp[i].Baggage
        //                                };
        //                                Child.FareBasis = tempData.FareRules.FirstOrDefault()?.FareBasisCode;
        //                                Child.RefundableType = (!tempData.IsRefundable) ? 0 : 2;
        //                                Child.SeatRemaing = temp[i].NoOfSeatAvailable;
        //                                Child.IsFreeMeel = false;
        //                                Child.FareComponent = new mdlFareComponent()
        //                                {
        //                                    BaseFare = chd.BaseFare,
        //                                    IGST = 0,
        //                                    TaxAndFees = chd.Tax + chd.PGCharge + chd.AdditionalTxnFeeOfrd + chd.AdditionalTxnFeePub,
        //                                    NetCommission = 0,

        //                                };
        //                                Child.FareComponent.TotalFare = Child.FareComponent.BaseFare + Child.FareComponent.TaxAndFees;
        //                                Child.FareComponent.NetFare = Child.FareComponent.TotalFare;
        //                            }
        //                            if (chd != null)
        //                            {
        //                                Infant = new mdlPassenger();
        //                                Infant.CabinClass = temp[i].CabinClass;
        //                                Infant.ClassOfBooking = temp[i].Airline.FareClass;
        //                                Infant.BaggageInformation = new mdlBaggageInformation()
        //                                {
        //                                    CabinBaggage = string.Empty,
        //                                    CheckingBaggage = string.Empty
        //                                };
        //                                Infant.FareBasis = tempData.FareRules.FirstOrDefault()?.FareBasisCode;
        //                                Infant.RefundableType = (!tempData.IsRefundable) ? 0 : 2;
        //                                Infant.SeatRemaing = temp[i].NoOfSeatAvailable;
        //                                Infant.IsFreeMeel = false;
        //                                Infant.FareComponent = new mdlFareComponent()
        //                                {
        //                                    BaseFare = inft.BaseFare,
        //                                    IGST = 0,
        //                                    TaxAndFees = inft.Tax + inft.PGCharge + inft.AdditionalTxnFeeOfrd + inft.AdditionalTxnFeePub,
        //                                    NetCommission = 0,

        //                                };
        //                                Infant.FareComponent.TotalFare = Infant.FareComponent.BaseFare + Infant.FareComponent.TaxAndFees;
        //                                Infant.FareComponent.NetFare = Infant.FareComponent.TotalFare;
        //                            }

        //                            mdlSearchResults_[i].TotalPriceList.Add(new mdlTotalpricelist()
        //                            {  
        //                                ADULT= Adult,
        //                                CHILD=Child,
        //                                INFANT=Infant,
        //                                fareIdentifier=tempData.ResultFareType,
        //                                ResultIndex= tempData.ResultIndex,                                        

        //                            });


        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return mdlSearchResults_;
        //}

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

        //private async Task<mdlSearchResponse> SearchFromTboAsync(mdlSearchRequest request)
        //{

        //    int MaxLoginAttempt = 1, LoginAttempt = 0;
        //    int.TryParse(_config["TBO:MaxLoginAttempt"], out MaxLoginAttempt);
        //    mdlSearchResponse mdlS = null;
        //    SearchResponse mdl = null;
        //    SearchResponseWraper mdlTemp = null;
        //    string tboUrl = _config["TBO:API:Search"];

        //    StartSendRequest:
        //    //Load tokken ID 
        //    var TokenDetails = _context.tblTboTokenDetails.OrderByDescending(p => p.GenrationDt).FirstOrDefault();
        //    if (TokenDetails == null)
        //    {
        //        var AuthenticateResponse = await LoginAsync();
        //        if (AuthenticateResponse.Status == 1 && LoginAttempt < MaxLoginAttempt)
        //        {
        //            LoginAttempt++;
        //            goto StartSendRequest;
        //        }
        //    }
        //    string jsonString = System.Text.Json.JsonSerializer.Serialize(SearchRequestMap(request, TokenDetails.TokenId));
        //    var HaveResponse = GetResponse(jsonString, tboUrl);
        //    if (HaveResponse.Code == 0)
        //    {
        //        mdlTemp = (System.Text.Json.JsonSerializer.Deserialize<SearchResponseWraper>(HaveResponse.Message));
        //        if (mdlTemp != null)
        //        {
        //            mdl = mdlTemp.Response;
        //        }
        //    }

        //    if (mdl != null)
        //    {
        //        if (mdl.ResponseStatus == 3 && LoginAttempt < MaxLoginAttempt)//failure
        //        {
        //            LoginAttempt++;
        //            var AuthenticateResponse = await LoginAsync();
        //            if (AuthenticateResponse.Status == 1)
        //            {
        //                goto StartSendRequest;
        //            }
        //            mdlS = new mdlSearchResponse()
        //            {
        //                ResponseStatus = 3,
        //                Error = new mdlError()
        //                {
        //                    Code = mdl.Error.ErrorCode,
        //                    Message = mdl.Error.ErrorMessage,
        //                }
        //            };
        //        }
        //        else if (mdl.ResponseStatus == 1)//success
        //        {


        //            //var result = Search_SaveAsync(request, TokenDetails.TokenId, mdl.TraceId, mdl.Results);

        //            //List<mdlSearchResult[]> AllResults = new List<mdlSearchResult[]>();
        //            //List<mdlSearchResult> ResultOB = new List<mdlSearchResult>();
        //            //List<mdlSearchResult> ResultIB = new List<mdlSearchResult>();
        //            //foreach (var dt in tempdata)
        //            //{
        //            //    if (dt.ResultIndex.Contains("OB"))
        //            //    {
        //            //        ResultOB.Add(SearchResultMap(dt, "OB"));
        //            //    }
        //            //    else if (dt.ResultIndex.Contains("IB"))
        //            //    {
        //            //        ResultIB.Add(SearchResultMap(dt, "IB"));
        //            //    }
        //            //    else
        //            //    {
        //            //        ResultOB.Add(SearchResultMap(dt, "OB"));
        //            //    }
        //            //}
        //            //if (ResultOB.Count() > 0)
        //            //{
        //            //    AllResults.Add(ResultOB.ToArray());
        //            //}
        //            //if (ResultIB.Count() > 0)
        //            //{
        //            //    AllResults.Add(ResultIB.ToArray());
        //            //}



        //            mdlS = new mdlSearchResponse()
        //            {
        //                ServiceProvider = enmServiceProvider.TBO,
        //                TraceId = mdl.TraceId,
        //                ResponseStatus = 1,
        //                Error = new mdlError()
        //                {
        //                    Code = 0,
        //                    Message = "-"
        //                },
        //                Origin = mdl.Origin,
        //                Destination = mdl.Destination,

        //            };
        //            //await result;
        //        }
        //        else
        //        {
        //            mdlS = new mdlSearchResponse()
        //            {
        //                ResponseStatus = 3,
        //                Error = new mdlError()
        //                {
        //                    Code = mdl.Error.ErrorCode,
        //                    Message = mdl.Error.ErrorMessage,
        //                }
        //            };
        //        }

        //    }
        //    else
        //    {
        //        mdlS = new mdlSearchResponse()
        //        {
        //            ResponseStatus = 100,
        //            Error = new mdlError()
        //            {
        //                Code = 100,
        //                Message = "Unable to Process",
        //            }
        //        };
        //    }

        //    return mdlS;
        //}

        #region ****************** Request ******************************

        public class SearchqueryWraper
        {
            public Searchquery searchQuery { get; set; }
        }

        public class Searchquery
        {
            public string cabinClass { get; set; }
            public Paxinfo paxInfo { get; set; }
            public Routeinfo[] routeInfos { get; set; }
            public Searchmodifiers searchModifiers { get; set; }
            public cityorairport[] preferredAirline { get; set; }
        }

        public class Paxinfo
        {
            public int ADULT { get; set; }
            public int CHILD { get; set; }
            public int INFANT { get; set; }
        }

        public class Searchmodifiers
        {
            public bool isDirectFlight { get; set; }
            public bool isConnectingFlight { get; set; }
        }

        public class Routeinfo
        {
            public cityorairport fromCityOrAirport { get; set; }
            public cityorairport toCityOrAirport { get; set; }
            public string travelDate { get; set; }
        }

        public class cityorairport
        {
            public string code { get; set; }
        }


        #endregion

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

        private async Task<bool> Search_SaveAsync(mdlSearchRequest request, string TraceId, TBOinfos tboInfos)
        {
            if (tboInfos == null)
            {
                return false;
            }
            int ExpirationMinute = 14;
            int.TryParse(_config["TBO:TraceIdExpiryTime"], out ExpirationMinute);
            double minFare = 0, minFareReturn = 0;

            List<tblTboTravelDetailResult> mdlDetail = new List<tblTboTravelDetailResult>();
            if (tboInfos.ONWARD != null)
            {
                if (tboInfos.ONWARD.Length > 0)
                {
                    minFare = tboInfos.ONWARD.Select(p => p.totalPriceList.Min(q => q.fd?.ADULT?.fC?.TF ?? 0)).Min();
                }


                mdlDetail.AddRange(tboInfos.ONWARD.Select(p => new tblTboTravelDetailResult
                {
                    segmentId = 0,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "OB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));



            }
            if (tboInfos.RETURN != null)
            {
                if (tboInfos.RETURN.Length > 0)
                {
                    minFareReturn = tboInfos.RETURN.Select(p => p.totalPriceList.Min(q => q.fd?.ADULT?.fC?.TF ?? 0)).Min();
                }
                mdlDetail.AddRange(tboInfos.RETURN.Select(p => new tblTboTravelDetailResult
                {
                    segmentId = 1,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "IB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));
            }
            if (tboInfos.COMBO != null)
            {
                if (tboInfos.COMBO.Length > 0)
                {
                    minFare = tboInfos.COMBO.Select(p => p.totalPriceList.Min(q => q.fd?.ADULT?.fC?.TF ?? 0)).Min();
                }
                mdlDetail.AddRange(tboInfos.COMBO.Select(p => new tblTboTravelDetailResult
                {
                    segmentId = 0,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "OB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));
            }
            if (tboInfos._0 != null)
            {
                if (tboInfos._0.Length > 0)
                {
                    minFare = tboInfos._0.Select(p => p.totalPriceList.Select(q => new { TotalPrice = q.fd?.ADULT.fC.TF ?? 0 }).Min()).Min().TotalPrice;
                }
                mdlDetail.AddRange(tboInfos._0.Select(p => new tblTboTravelDetailResult
                {
                    segmentId = 0,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "OB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));
            }
            if (tboInfos._1 != null)
            {
                if (tboInfos._1.Length > 0)
                {
                    minFare = tboInfos._1.Select(p => p.totalPriceList.Select(q => new { TotalPrice = q.fd?.ADULT.fC.TF ?? 0 }).Min()).Min().TotalPrice;
                }
                mdlDetail.AddRange(tboInfos._1.Select(p => new tblTboTravelDetailResult
                {
                    segmentId = 1,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "OB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));
            }
            if (tboInfos._2 != null)
            {
                if (tboInfos._2.Length > 0)
                {
                    minFare = tboInfos._2.Select(p => p.totalPriceList.Select(q => new { TotalPrice = q.fd?.ADULT.fC.TF ?? 0 }).Min()).Min().TotalPrice;
                }
                mdlDetail.AddRange(tboInfos._2.Select(p => new tblTboTravelDetailResult
                {
                    segmentId = 2,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "OB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));
            }
            if (tboInfos._3 != null)
            {
                if (tboInfos._3.Length > 0)
                {
                    minFare = tboInfos._3.Select(p => p.totalPriceList.Select(q => new { TotalPrice = q.fd?.ADULT.fC.TF ?? 0 }).Min()).Min().TotalPrice;
                }
                mdlDetail.AddRange(tboInfos._3.Select(p => new tblTboTravelDetailResult
                {
                    segmentId = 3,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "OB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));
            }
            if (tboInfos._4 != null)
            {
                if (tboInfos._4.Length > 0)
                {
                    minFare = tboInfos._4.Select(p => p.totalPriceList.Select(q => new { TotalPrice = q.fd?.ADULT.fC.TF ?? 0 }).Min()).Min().TotalPrice;
                }
                mdlDetail.AddRange(tboInfos._4.Select(p => new tblTboTravelDetailResult
                {
                    segmentId = 4,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "OB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));
            }
            if (tboInfos._5 != null)
            {
                if (tboInfos._5.Length > 0)
                {
                    minFare = tboInfos._5.Select(p => p.totalPriceList.Select(q => new { TotalPrice = q.fd?.ADULT.fC.TF ?? 0 }).Min()).Min().TotalPrice;
                }
                mdlDetail.AddRange(tboInfos._5.Select(p => new tblTboTravelDetailResult
                {
                    segmentId = 5,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "OB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));
            }



            DateTime TickGeration = DateTime.Now;
            tblTboTravelDetail td = new tblTboTravelDetail()
            {
                TravelDate = request.Segments[0].TravelDt,
                CabinClass = request.Segments[0].FlightCabinClass,
                Origin = request.Segments[0].Origin,
                Destination = request.Segments[0].Destination,
                MinPublishFare = minFare,
                MinPublishFareReturn = minFareReturn,
                JourneyType = request.JourneyType,
                AdultCount = request.AdultCount,
                ChildCount = request.ChildCount,
                InfantCount = request.InfantCount,
                GenrationDt = TickGeration,
                TraceId = TraceId,
                ExpireDt = TickGeration.AddMinutes(ExpirationMinute),
                tblTboTravelDetailResult = mdlDetail,
            };
            _context.tblTboTravelDetail.Add(td);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<mdlSearchResponse> SearchFromTboAsync(mdlSearchRequest request)
        {
            SearchresultWraper mdl = null;
            mdlSearchResponse mdlS = null;
            string tboUrl = _config["TBO:API:Search"];


            string jsonString = JsonConvert.SerializeObject(SearchRequestMap(request));
            var HaveResponse = GetResponse(jsonString, tboUrl);
            {
                if (HaveResponse.Code == 0)
                {
                    mdl = (JsonConvert.DeserializeObject<SearchresultWraper>(HaveResponse.Message));
                }
                if (mdl != null)
                {
                    if (mdl.status.success)//success
                    {
                        if (mdl.searchResult?.tboInfos == null)
                        {
                            mdlS = new mdlSearchResponse()
                            {
                                ResponseStatus = 3,
                                Error = new mdlError()
                                {
                                    Code = mdl.status.httpStatus,
                                    Message = "No data found",
                                }
                            };
                        }

                        string TraceId = Guid.NewGuid().ToString();
                        var result = Search_SaveAsync(request, TraceId, mdl.searchResult.tboInfos);
                        List<List<mdlSearchResult>> AllResults = new List<List<mdlSearchResult>>();
                        List<mdlSearchResult> Result1 = new List<mdlSearchResult>();
                        List<mdlSearchResult> Result2 = new List<mdlSearchResult>();
                        List<mdlSearchResult> Result3 = new List<mdlSearchResult>();
                        List<mdlSearchResult> Result4 = new List<mdlSearchResult>();
                        List<mdlSearchResult> Result5 = new List<mdlSearchResult>();
                        List<mdlSearchResult> Result6 = new List<mdlSearchResult>();

                        if (mdl.searchResult.tboInfos != null)
                        {
                            if (mdl.searchResult.tboInfos.ONWARD != null)
                            {
                                Result1.AddRange(SearchResultMap(mdl.searchResult.tboInfos.ONWARD));
                            }
                            if (mdl.searchResult.tboInfos.RETURN != null)
                            {
                                Result2.AddRange(SearchResultMap(mdl.searchResult.tboInfos.RETURN));
                            }
                            if (mdl.searchResult.tboInfos.COMBO != null)
                            {
                                Result1.AddRange(SearchResultMap(mdl.searchResult.tboInfos.COMBO));
                            }
                            if (mdl.searchResult.tboInfos._0 != null)
                            {
                                Result1.AddRange(SearchResultMap(mdl.searchResult.tboInfos._0));
                            }
                            if (mdl.searchResult.tboInfos._1 != null)
                            {
                                Result2.AddRange(SearchResultMap(mdl.searchResult.tboInfos._1));
                            }
                            if (mdl.searchResult.tboInfos._2 != null)
                            {
                                Result3.AddRange(SearchResultMap(mdl.searchResult.tboInfos._2));
                            }
                            if (mdl.searchResult.tboInfos._3 != null)
                            {
                                Result4.AddRange(SearchResultMap(mdl.searchResult.tboInfos._3));
                            }
                            if (mdl.searchResult.tboInfos._4 != null)
                            {
                                Result5.AddRange(SearchResultMap(mdl.searchResult.tboInfos._4));
                            }
                            if (mdl.searchResult.tboInfos._5 != null)
                            {
                                Result6.AddRange(SearchResultMap(mdl.searchResult.tboInfos._5));
                            }


                        }
                        if (Result1.Count() > 0)
                        {
                            AllResults.Add(Result1);
                        }
                        if (Result2.Count() > 0)
                        {
                            AllResults.Add(Result2);
                        }
                        if (Result3.Count() > 0)
                        {
                            AllResults.Add(Result3);
                        }
                        if (Result4.Count() > 0)
                        {
                            AllResults.Add(Result4);
                        }
                        if (Result5.Count() > 0)
                        {
                            AllResults.Add(Result5);
                        }
                        if (Result6.Count() > 0)
                        {
                            AllResults.Add(Result6);
                        }

                        mdlS = new mdlSearchResponse()
                        {
                            ServiceProvider = enmServiceProvider.TBO,
                            TraceId = TraceId,
                            ResponseStatus = 1,
                            Error = new mdlError()
                            {
                                Code = 0,
                                Message = "-"
                            },
                            Origin = request.Segments[0].Origin,
                            Destination = request.Segments[0].Destination,
                            Results = AllResults
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
                                Code = mdl.status.httpStatus,
                                Message = "Invalid Request",
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
            }

            return mdlS;
        }

        public class SearchresultWraper
        {
            public Searchresult searchResult { get; set; }
            public Status status { get; set; }
            public Metainfo metaInfo { get; set; }
        }

        public class Metainfo
        {
        }

        public class SearchresultMulticity
        {
            public ONWARD_RETURN_COMBO[] tboInfos { get; set; }
        }

        public class Searchresult
        {
            public TBOinfos tboInfos { get; set; }
            public Alert[] alerts { get; set; }
            public Searchquery searchQuery { get; set; }
            public string bookingId { get; set; }
            public Totalpriceinfo totalPriceInfo { get; set; }
            public Status status { get; set; }
            public Conditions conditions { get; set; }
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

        public class TBOinfos
        {
            public ONWARD_RETURN_COMBO[] ONWARD { get; set; }
            public ONWARD_RETURN_COMBO[] RETURN { get; set; }
            public ONWARD_RETURN_COMBO[] COMBO { get; set; }
            [JsonProperty(PropertyName = "0")]
            public ONWARD_RETURN_COMBO[] _0 { get; set; }
            [JsonProperty(PropertyName = "1")]
            public ONWARD_RETURN_COMBO[] _1 { get; set; }
            [JsonProperty(PropertyName = "2")]
            public ONWARD_RETURN_COMBO[] _2 { get; set; }
            [JsonProperty(PropertyName = "3")]
            public ONWARD_RETURN_COMBO[] _3 { get; set; }
            [JsonProperty(PropertyName = "4")]
            public ONWARD_RETURN_COMBO[] _4 { get; set; }
            [JsonProperty(PropertyName = "5")]
            public ONWARD_RETURN_COMBO[] _5 { get; set; }
            [JsonProperty(PropertyName = "6")]
            public ONWARD_RETURN_COMBO[] _6 { get; set; }

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
            public int TBOIndicator { get; set; }
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

        private async Task RemoveFromDb(mdlFareQuotRequest request)
        {
            DateTime currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            var Existing = _context.tblTboTravelDetail.Where(p => p.TraceId == request.TraceId && p.ExpireDt > currentDate).FirstOrDefault();
            if (Existing != null)
            {
                _context.Database.ExecuteSqlRaw("delete from tblTBoTravelDetailResult where TravelDetailId=@p0", Existing.TravelDetailId);
                _context.Database.ExecuteSqlRaw("delete from tblTBoTravelDetail where TravelDetailId=@p0", Existing.TravelDetailId);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<mdlFareQuotResponse> FareQuoteAsync(mdlFareQuotRequest request)
        {
            mdlFareQuotResponse mdl = await FareQuoteFromTBOAsync(request);
            if (mdl.IsPriceChanged)
            {
                await RemoveFromDb(request);
            }
            return mdl;
        }

        private FareQuotRequest FareQuoteRequestMap(mdlFareQuotRequest request)
        {

            FareQuotRequest mdl = new FareQuotRequest()
            {
                priceIds = request.ResultIndex
            };
            return mdl;
        }

        private async Task<mdlFareQuotResponse> FareQuoteFromTBOAsync(mdlFareQuotRequest request)
        {

            mdlFareQuotResponse mdlS = null;
            FareQuotResponse mdl = null;

            DateTime DepartureDt = DateTime.Now, ArrivalDt = DateTime.Now;


            string tboUrl = _config["TBO:API:FareQuote"];
            string jsonString = System.Text.Json.JsonSerializer.Serialize(FareQuoteRequestMap(request));
            var HaveResponse = GetResponse(jsonString, tboUrl);
            if (HaveResponse.Code == 0)
            {
                mdl = (JsonConvert.DeserializeObject<FareQuotResponse>(HaveResponse.Message));
            }

            if (mdl != null)
            {

                if (mdl.status.success)//success
                {
                    List<List<mdlSearchResult>> AllResults = new List<List<mdlSearchResult>>();
                    List<mdlSearchResult> Result1 = new List<mdlSearchResult>();
                    int ServiceProvider = (int)enmServiceProvider.TBO;
                    if (mdl.tboInfos != null)
                    {
                        Result1.AddRange(SearchResultMap(mdl.tboInfos));
                    }
                    if (Result1.Count() > 0)
                    {
                        AllResults.Add(Result1);
                    }
                    DateTime.TryParse(mdl.searchQuery?.routeInfos?.FirstOrDefault()?.travelDate, out DepartureDt);
                    mdlS = new mdlFareQuotResponse()
                    {

                        ServiceProvider = enmServiceProvider.TBO,
                        TraceId = request.TraceId,
                        BookingId = ServiceProvider + "_" + mdl.bookingId,
                        ResponseStatus = 1,
                        IsPriceChanged = mdl.alerts?.Any(p => p.oldFare != p.newFare) ?? false,
                        Error = new mdlError()
                        {
                            Code = 0,
                            Message = ""
                        },
                        Origin = mdl.searchQuery.routeInfos.FirstOrDefault()?.fromCityOrAirport.code,
                        Destination = mdl.searchQuery.routeInfos.FirstOrDefault()?.toCityOrAirport.code,
                        Results = AllResults,
                        TotalPriceInfo = new mdlTotalPriceInfo()
                        {
                            BaseFare = mdl.totalPriceInfo?.totalFareDetail?.fC?.BF ?? 0,
                            TaxAndFees = mdl.totalPriceInfo?.totalFareDetail?.fC?.TAF ?? 0,
                            TotalFare = mdl.totalPriceInfo?.totalFareDetail?.fC?.TF ?? 0,
                        },
                        SearchQuery = new Models.mdlFlightSearchWraper()
                        {
                            AdultCount = mdl.searchQuery?.paxInfo?.ADULT ?? 0,
                            ChildCount = mdl.searchQuery?.paxInfo?.CHILD ?? 0,
                            InfantCount = mdl.searchQuery?.paxInfo?.INFANT ?? 0,
                            CabinClass = (enmCabinClass)Enum.Parse(typeof(enmCabinClass), mdl.searchQuery?.cabinClass ?? (nameof(enmCabinClass.ECONOMY)), true),
                            JourneyType = enmJourneyType.OneWay,
                            DepartureDt = DepartureDt,
                            From = mdl.searchQuery?.routeInfos?.FirstOrDefault()?.fromCityOrAirport?.code,
                            To = mdl.searchQuery?.routeInfos?.FirstOrDefault()?.toCityOrAirport?.code
                        },
                        FareQuoteCondition = new mdlFareQuoteCondition()
                        {
                            dob = new mdlDobCondition()
                            {
                                adobr = mdl.conditions?.dob?.adobr ?? false,
                                cdobr = mdl.conditions?.dob?.cdobr ?? false,
                                idobr = mdl.conditions?.dob?.idobr ?? false,
                            },
                            GstCondition = new mdlGstCondition()
                            {
                                IsGstMandatory = mdl.conditions?.gst?.igm ?? false,
                                IsGstApplicable = mdl.conditions?.gst?.gstappl ?? true,
                            },
                            IsHoldApplicable = mdl.conditions?.isBA ?? false,
                            PassportCondition = new mdlPassportCondition()
                            {
                                IsPassportExpiryDate = mdl.conditions?.pcs?.pped ?? false,
                                isPassportIssueDate = mdl.conditions?.pcs?.pid ?? false,
                                isPassportRequired = mdl.conditions?.pcs?.pm ?? false,
                            }

                        }

                    };
                }
                else
                {
                    mdlS = new mdlFareQuotResponse()
                    {
                        ResponseStatus = 3,
                        Error = new mdlError()
                        {
                            Code = mdl.status.httpStatus,
                            Message = mdl.errors?.FirstOrDefault()?.message,
                        }
                    };
                }

            }
            else
            {
                mdlS = new mdlFareQuotResponse()
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

        #region ************************ Fare Quote Classes****************
        public class FareQuotRequest
        {
            public string[] priceIds { get; set; }
        }
        public class FareQuotResponse : SearchresultMulticity
        {
            public Alert[] alerts { get; set; }
            public Searchquery searchQuery { get; set; }
            public string bookingId { get; set; }
            public Totalpriceinfo totalPriceInfo { get; set; }
            public Status status { get; set; }
            public Conditions conditions { get; set; }
            public Error[] errors { get; set; }
        }

        #endregion

        public async Task<mdlFareRuleResponse> FareRuleAsync(mdlFareRuleRequest request)
        {
            mdlFareRuleResponse response = null;
            response = await FareRuleFromTBOAsync(request);
            return response;
        }

        public async Task<mdlBookingResponse> BookingAsync(mdlBookingRequest request)
        {
            return await BookingFromTBOAsync(request);
        }


        #region *******************Fare Rule Function ****************************
        private FareRuleRequest FareRuleRequestMap(mdlFareRuleRequest request)
        {
            FareRuleRequest mdl = new FareRuleRequest()
            {
                flowType = "SEARCH",
                id = request.ResultIndex.FirstOrDefault()
            };
            return mdl;
        }



        private async Task<mdlFareRuleResponse> FareRuleFromTBOAsync(mdlFareRuleRequest request)
        {


            mdlFareRuleResponse mdlS = null;
            FareRuleResponse mdl = null;
            string tboUrl = _config["TBO:API:FareRule"];
            string jsonString = System.Text.Json.JsonSerializer.Serialize(FareRuleRequestMap(request));
            var HaveResponse = GetResponse(jsonString, tboUrl);
            if (HaveResponse.Code == 0)
            {
                dynamic mdlTemp = JValue.Parse(HaveResponse.Message);
                if (Convert.ToBoolean(mdlTemp.status.success))
                {
                    JObject rss = JObject.Parse(Convert.ToString(mdlTemp.fareRule));
                    string property_name = string.Empty;
                    foreach (JProperty prop in rss.Properties())
                    {
                        property_name = prop.Name;
                    }
                    mdl = JsonConvert.DeserializeObject<FareRuleResponse>(Convert.ToString(rss[property_name]));
                    if (mdl != null)
                    {
                        mdlS = new mdlFareRuleResponse()
                        {
                            ResponseStatus = 1,
                            Error = new mdlError()
                            {
                                Code = 0,
                                Message = string.Empty,
                            },
                            FareRule = new mdlFareRule()
                            {
                                isML = mdl.isML,
                                cB = new mdlPassengerBagege()
                                {
                                    ADT = mdl.cB?.ADT ?? string.Empty,
                                    CNN = mdl.cB?.CNN ?? string.Empty,
                                    INF = mdl.cB?.INF ?? string.Empty,
                                },
                                hB = new mdlPassengerBagege()
                                {
                                    ADT = mdl.hB?.ADT ?? string.Empty,
                                    CNN = mdl.hB?.CNN ?? string.Empty,
                                    INF = mdl.hB?.INF ?? string.Empty,
                                },
                                rT = mdl.rT,
                                isHB = mdl.isHB,
                                fr = new mdlFarePolicy()
                                {
                                    CANCELLATION = new mdlAllPOlicy()
                                    {
                                        policyInfo = mdl.fr?.CANCELLATION?.DEFAULT?.policyInfo ?? string.Empty,
                                        amount = (mdl.fr?.CANCELLATION?.DEFAULT?.fcs?.ARF ?? 0) +
                                        (mdl.fr?.CANCELLATION?.DEFAULT?.fcs?.ARFT ?? 0) +
                                        (mdl.fr?.CANCELLATION?.DEFAULT?.fcs?.ACF ?? 0) +
                                        (mdl.fr?.CANCELLATION?.DEFAULT?.fcs?.ACFT ?? 0) +
                                        (mdl.fr?.CANCELLATION?.DEFAULT?.fcs?.CRF ?? 0) +
                                        (mdl.fr?.CANCELLATION?.DEFAULT?.fcs?.CRFT ?? 0) +
                                        (mdl.fr?.CANCELLATION?.DEFAULT?.fcs?.CCF ?? 0) +
                                        (mdl.fr?.CANCELLATION?.DEFAULT?.fcs?.CCFT ?? 0),
                                        additionalFee = 0,
                                    },
                                    DATECHANGE = new mdlAllPOlicy()
                                    {
                                        policyInfo = mdl.fr?.DATECHANGE?.DEFAULT?.policyInfo ?? string.Empty,
                                        amount = (mdl.fr?.DATECHANGE?.DEFAULT?.fcs?.ARF ?? 0) +
                                        (mdl.fr?.DATECHANGE?.DEFAULT?.fcs?.ARFT ?? 0) +
                                        (mdl.fr?.DATECHANGE?.DEFAULT?.fcs?.ACF ?? 0) +
                                        (mdl.fr?.DATECHANGE?.DEFAULT?.fcs?.ACFT ?? 0) +
                                        (mdl.fr?.DATECHANGE?.DEFAULT?.fcs?.CRF ?? 0) +
                                        (mdl.fr?.DATECHANGE?.DEFAULT?.fcs?.CRFT ?? 0) +
                                        (mdl.fr?.DATECHANGE?.DEFAULT?.fcs?.CCF ?? 0) +
                                        (mdl.fr?.DATECHANGE?.DEFAULT?.fcs?.CCFT ?? 0),
                                        additionalFee = 0,
                                    },
                                    SEAT_CHARGEABLE = new mdlAllPOlicy()
                                    {
                                        policyInfo = mdl.fr?.SEAT_CHARGEABLE?.DEFAULT?.policyInfo ?? string.Empty,
                                        amount = (mdl.fr?.SEAT_CHARGEABLE?.DEFAULT?.fcs?.ARF ?? 0) +
                                        (mdl.fr?.SEAT_CHARGEABLE?.DEFAULT?.fcs?.ARFT ?? 0) +
                                        (mdl.fr?.SEAT_CHARGEABLE?.DEFAULT?.fcs?.ACF ?? 0) +
                                        (mdl.fr?.SEAT_CHARGEABLE?.DEFAULT?.fcs?.ACFT ?? 0) +
                                        (mdl.fr?.SEAT_CHARGEABLE?.DEFAULT?.fcs?.CRF ?? 0) +
                                        (mdl.fr?.SEAT_CHARGEABLE?.DEFAULT?.fcs?.CRFT ?? 0) +
                                        (mdl.fr?.SEAT_CHARGEABLE?.DEFAULT?.fcs?.CCF ?? 0) +
                                        (mdl.fr?.SEAT_CHARGEABLE?.DEFAULT?.fcs?.CCFT ?? 0),
                                        additionalFee = 0,
                                    },

                                }
                            }
                        };
                    }
                }
                else
                {
                    mdlS = new mdlFareRuleResponse()
                    {
                        ResponseStatus = 3,
                        Error = new mdlError()
                        {
                            Code = Convert.ToInt32(mdlTemp.errors.errCode),
                            Message = Convert.ToString(mdlTemp.errors.message),
                        }
                    };
                }


            }
            if (mdlS == null)
            {
                mdlS = new mdlFareRuleResponse()
                {
                    ResponseStatus = 3,
                    Error = new mdlError()
                    {
                        Code = 12,
                        Message = "Unable to Process",
                    }
                };
            }
            return mdlS;
        }



        public class FareRuleRequest
        {
            public string id { get; set; }
            public string flowType { get; set; }
        }



        public class FareRuleResponse
        {
            public bool isML { get; set; }
            public bool isHB { get; set; }
            public string rT { get; set; }
            public PassengerBagege cB { get; set; }
            public PassengerBagege hB { get; set; }
            public FarePolicy fr { get; set; }
        }

        public class PassengerBagege
        {
            public string ADT { get; set; }
            public string CNN { get; set; }
            public string INF { get; set; }
        }


        public class FarePolicy
        {
            public AllPOlicy NO_SHOW { get; set; }
            public AllPOlicy DATECHANGE { get; set; }
            public AllPOlicy CANCELLATION { get; set; }
            public AllPOlicy SEAT_CHARGEABLE { get; set; }
        }

        public class AllPOlicy
        {
            public DEFAULT DEFAULT { get; set; }
        }

        public class DEFAULT
        {
            public double amount { get; set; }
            public double additionalFee { get; set; }
            public string policyInfo { get; set; }
            public Fcs fcs { get; set; }
        }


        public class Fcs
        {
            public double ARF { get; set; }
            public double CRFT { get; set; }
            public double CRF { get; set; }
            public double ARFT { get; set; }
            public double ACFT { get; set; }
            public double ACF { get; set; }
            public double CCF { get; set; }
            public double CCFT { get; set; }
        }








        #endregion


        #region ********************** Booking ****************************

        private BookingRequest BookingRequestMap(mdlBookingRequest request)
        {
            GstInfo _gstInfo = null;
            if (request.gstInfo != null)
            {
                _gstInfo = new GstInfo()
                {
                    address = request.gstInfo.address,
                    email = request.gstInfo.email,
                    gstNumber = request.gstInfo.gstNumber,
                    mobile = request.gstInfo.mobile,
                    registeredName = request.gstInfo.registeredName,
                };
            }
            PaymentInfos[] paymentInfos = null;
            if (request.paymentInfos != null)
            {
                paymentInfos = request.paymentInfos.Select(p => new PaymentInfos { amount = p.amount }).ToArray();
            }


            BookingRequest mdl = new BookingRequest()
            {
                bookingId = request.BookingId,
                gstInfo = _gstInfo,
                deliveryInfo = new Deliveryinfo()
                {
                    contacts = request.deliveryInfo?.contacts,
                    emails = request.deliveryInfo?.emails,
                },
                travellerInfo = request.travellerInfo.Select(p => new Travellerinfo
                {
                    ti = p.Title.ToUpper(),
                    fN = p.FirstName,
                    lN = p.LastName,
                    dob = p.dob.HasValue ? p.dob.Value.ToString("yyyy-MM-dd") : null,
                    eD = p.PassportExpiryDate.ToString("yyyy-MM-dd"),
                    pid = p.PassportIssueDate.ToString("yyyy-MM-dd"),
                    pNum = p.pNum,
                    pt = p.passengerType.ToString().Trim().ToUpper(),
                    ssrBaggageInfos = p.ssrBaggageInfos == null ? null : (new SSRS() { key = p.ssrBaggageInfos.key, value = p.ssrBaggageInfos.value }),
                    ssrSeatInfos = p.ssrSeatInfos == null ? null : (new SSRS() { key = p.ssrSeatInfos.key, value = p.ssrSeatInfos.value }),
                    ssrMealInfos = p.ssrMealInfos == null ? null : (new SSRS() { key = p.ssrMealInfos.key, value = p.ssrMealInfos.value }),
                    ssrExtraServiceInfos = p.ssrExtraServiceInfos == null ? null : (new SSRS() { key = p.ssrExtraServiceInfos.key, value = p.ssrExtraServiceInfos.value }),


                }).ToArray(),
                paymentInfos = paymentInfos
            };
            return mdl;
        }

        private async Task<mdlBookingResponse> BookingFromTBOAsync(mdlBookingRequest request)
        {

            mdlBookingResponse mdlS = null;
            BookingResponse mdl = null;
            //set the Upper case in pax type


            string tboUrl = _config["TBO:API:Book"];
            string jsonString = System.Text.Json.JsonSerializer.Serialize(BookingRequestMap(request));
            var HaveResponse = GetResponse(jsonString, tboUrl);
            if (HaveResponse.Code == 0)
            {
                mdl = (JsonConvert.DeserializeObject<BookingResponse>(HaveResponse.Message));
            }

            if (mdl != null)
            {
                if (mdl.status.success)//success
                {
                    mdlS = new mdlBookingResponse()
                    {
                        bookingId = mdl.bookingId,
                        Error = new mdlError()
                        {
                            Code = 0,
                            Message = ""
                        },
                        ResponseStatus = 1,

                    };
                }
                else
                {
                    mdlS = new mdlBookingResponse()
                    {
                        ResponseStatus = 3,
                        Error = new mdlError()
                        {
                            Code = 12,
                            Message = mdl.errors?.FirstOrDefault()?.message ?? "",
                        }
                    };
                }

            }
            else
            {
                dynamic data = JObject.Parse(HaveResponse.Message);
                mdlS = new mdlBookingResponse()
                {
                    ResponseStatus = 100,
                    Error = new mdlError()
                    {
                        Code = 100,
                        Message = data.errors[0].message ?? "Unable to Process",
                    }
                };
            }

            return mdlS;
        }


        #region *****************Booking Request classes *************************



        public class BookingRequest
        {
            public string bookingId { get; set; }
            public Travellerinfo[] travellerInfo { get; set; }
            public Deliveryinfo deliveryInfo { get; set; }
            public GstInfo gstInfo { get; set; }
            public PaymentInfos[] paymentInfos { get; set; }
        }

        public class PaymentInfos
        {
            public double amount { get; set; }
        }

        public class Deliveryinfo
        {
            public List<string> emails { get; set; }
            public List<string> contacts { get; set; }
        }

        public class Travellerinfo
        {
            public string ti { get; set; }
            public string fN { get; set; }
            public string lN { get; set; }
            public string pt { get; set; }
            public string dob { get; set; }
            public string pNum { get; set; }
            public string eD { get; set; }
            public string pid { get; set; }
            public SSRS ssrBaggageInfos { get; set; }
            public SSRS ssrMealInfos { get; set; }
            public SSRS ssrSeatInfos { get; set; }
            public SSRS ssrExtraServiceInfos { get; set; }
        }


        public class GstInfo
        {
            public string gstNumber { get; set; }
            public string email { get; set; }
            public string registeredName { get; set; }
            public string mobile { get; set; }
            public string address { get; set; }
        }

        public class SSRS
        {
            public string key { get; set; }
            public string value { get; set; }
        }


        public class BookingResponse
        {
            public string bookingId { get; set; }
            public Status status { get; set; }
            public Metainfo metaInfo { get; set; }
            public Error[] errors { get; set; }
        }




        #endregion



        #endregion


    }
}
