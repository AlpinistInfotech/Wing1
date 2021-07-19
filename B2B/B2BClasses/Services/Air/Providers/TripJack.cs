using B2BClasses.Database;
using B2BClasses.Services.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
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
    public interface ITripJack : IWingFlight
    {
    }

    public class TripJack : ITripJack
    {

        private readonly DBContext _context;
        private readonly IConfiguration _config;
        public TripJack(DBContext context, IConfiguration config)
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
                request.Headers.Add("apikey", _config["TripJack:Credential:apikey"]);
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
                WebResponse webResponse = request.GetResponse();
                var rsp = webResponse.GetResponseStream();
                if (rsp == null)
                {
                    mdl.Message = "No Response Found";
                }
                using (StreamReader readStream = new StreamReader(rsp))
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
                if (response == null)
                {
                    mdl.Message = "No Response from server";
                }
                else
                {
                    Stream stream = response.GetResponseStream();
                    String responseMessage = new StreamReader(stream).ReadToEnd();
                    mdl.Message = responseMessage;
                }


            }
            catch (Exception ex)
            {
                mdl.Code = 1;
                mdl.Message = ex.Message;
            }
            return mdl;
        }

        private mdlError GetResponseZip(string requestData, string url)
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
                request.Headers.Add("apikey", _config["TripJack:Credential:apikey"]);
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
                WebResponse webResponse = request.GetResponse();
                var rsp = webResponse.GetResponseStream();
                if (rsp == null)
                {
                    mdl.Message = "No Response Found";
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
                if (response != null)
                {
                    Stream stream = response.GetResponseStream();
                    String responseMessage = new StreamReader(stream).ReadToEnd();
                    mdl.Message = responseMessage;
                }
                else
                {
                    mdl.Message = "Invalid Connection";
                }

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
            if (request.JourneyType == enmJourneyType.OneWay)
            {
                response = SearchFromDb(request);
                if (response == null)
                {
                    response = await SearchFromTripJackAsync(request);
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
                        tempRe = await SearchFromTripJackAsync(request);
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
            int tripjackId = (int)enmServiceProvider.TripJack;
            response.Results?.ForEach(p =>
            {
                p.ForEach(q => q.TotalPriceList.ForEach(r => r.ResultIndex = "" + tripjackId + "_" + r.ResultIndex));
            });

            return response;
        }


        public async Task<mdlFareQuotResponse> FareQuoteAsync(mdlFareQuotRequest request)
        {
            mdlFareQuotResponse mdl = await FareQuoteFromTripJacAsync(request);
            if (mdl.IsPriceChanged)
            {
                await RemoveFromDb(request);
            }
            return mdl;
        }

        public async Task<mdlFareRuleResponse> FareRuleAsync(mdlFareRuleRequest request)
        {
            mdlFareRuleResponse response = null;
            response = await FareRuleFromTripJackAsync(request);
            return response;
        }

        public async Task<mdlBookingResponse> BookingAsync(mdlBookingRequest request)
        {
            return await BookingFromTripJacAsync(request);
        }


        #region *******************Search Class***************************

        private async Task<bool> Search_SaveAsync(mdlSearchRequest request, string TraceId, Tripinfos tripinfos)
        {
            if (tripinfos == null)
            {
                return false;
            }
            int ExpirationMinute = 14;
            int.TryParse(_config["TripJack:TraceIdExpiryTime"], out ExpirationMinute);
            double minFare = 0, minFareReturn = 0;

            List<tblTripJackTravelDetailResult> mdlDetail = new List<tblTripJackTravelDetailResult>();
            if (tripinfos.ONWARD != null)
            {
                if (tripinfos.ONWARD.Length > 0)
                {
                    minFare = tripinfos.ONWARD.Select(p => p.totalPriceList.Min(q => q.fd?.ADULT?.fC?.TF ?? 0)).Min();
                }


                mdlDetail.AddRange(tripinfos.ONWARD.Select(p => new tblTripJackTravelDetailResult
                {
                    segmentId = 0,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "OB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));



            }
            if (tripinfos.RETURN != null)
            {
                if (tripinfos.RETURN.Length > 0)
                {
                    minFareReturn = tripinfos.RETURN.Select(p => p.totalPriceList.Min(q => q.fd?.ADULT?.fC?.TF ?? 0)).Min();
                }
                mdlDetail.AddRange(tripinfos.RETURN.Select(p => new tblTripJackTravelDetailResult
                {
                    segmentId = 1,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "IB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));
            }
            if (tripinfos.COMBO != null)
            {
                if (tripinfos.COMBO.Length > 0)
                {
                    minFare = tripinfos.COMBO.Select(p => p.totalPriceList.Min(q => q.fd?.ADULT?.fC?.TF ?? 0)).Min();
                }
                mdlDetail.AddRange(tripinfos.COMBO.Select(p => new tblTripJackTravelDetailResult
                {
                    segmentId = 0,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "OB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));
            }
            if (tripinfos._0 != null)
            {
                if (tripinfos._0.Length > 0)
                {
                    minFare = tripinfos._0.Select(p => p.totalPriceList.Select(q => new { TotalPrice = q.fd?.ADULT.fC.TF ?? 0 }).Min()).Min().TotalPrice;
                }
                mdlDetail.AddRange(tripinfos._0.Select(p => new tblTripJackTravelDetailResult
                {
                    segmentId = 0,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "OB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));
            }
            if (tripinfos._1 != null)
            {
                if (tripinfos._1.Length > 0)
                {
                    minFare = tripinfos._1.Select(p => p.totalPriceList.Select(q => new { TotalPrice = q.fd?.ADULT.fC.TF ?? 0 }).Min()).Min().TotalPrice;
                }
                mdlDetail.AddRange(tripinfos._1.Select(p => new tblTripJackTravelDetailResult
                {
                    segmentId = 1,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "OB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));
            }
            if (tripinfos._2 != null)
            {
                if (tripinfos._2.Length > 0)
                {
                    minFare = tripinfos._2.Select(p => p.totalPriceList.Select(q => new { TotalPrice = q.fd?.ADULT.fC.TF ?? 0 }).Min()).Min().TotalPrice;
                }
                mdlDetail.AddRange(tripinfos._2.Select(p => new tblTripJackTravelDetailResult
                {
                    segmentId = 2,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "OB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));
            }
            if (tripinfos._3 != null)
            {
                if (tripinfos._3.Length > 0)
                {
                    minFare = tripinfos._3.Select(p => p.totalPriceList.Select(q => new { TotalPrice = q.fd?.ADULT.fC.TF ?? 0 }).Min()).Min().TotalPrice;
                }
                mdlDetail.AddRange(tripinfos._3.Select(p => new tblTripJackTravelDetailResult
                {
                    segmentId = 3,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "OB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));
            }
            if (tripinfos._4 != null)
            {
                if (tripinfos._4.Length > 0)
                {
                    minFare = tripinfos._4.Select(p => p.totalPriceList.Select(q => new { TotalPrice = q.fd?.ADULT.fC.TF ?? 0 }).Min()).Min().TotalPrice;
                }
                mdlDetail.AddRange(tripinfos._4.Select(p => new tblTripJackTravelDetailResult
                {
                    segmentId = 4,
                    ResultIndex = p.totalPriceList.FirstOrDefault()?.id,
                    ResultType = "OB",
                    OfferedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    PublishedFare = p.totalPriceList.Min(q => q.fd.ADULT?.fC.TF ?? 0),
                    JsonData = JsonConvert.SerializeObject(p)
                }));
            }
            if (tripinfos._5 != null)
            {
                if (tripinfos._5.Length > 0)
                {
                    minFare = tripinfos._5.Select(p => p.totalPriceList.Select(q => new { TotalPrice = q.fd?.ADULT.fC.TF ?? 0 }).Min()).Min().TotalPrice;
                }
                mdlDetail.AddRange(tripinfos._5.Select(p => new tblTripJackTravelDetailResult
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
            tblTripJackTravelDetail td = new tblTripJackTravelDetail()
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
                tblTripJackTravelDetailResult = mdlDetail,
            };
            _context.tblTripJackTravelDetail.Add(td);
            await _context.SaveChangesAsync();
            return true;
        }

        private SearchqueryWraper SearchRequestMap(mdlSearchRequest request)
        {
            enmCabinClass enmCabin = request.Segments[0].FlightCabinClass== enmCabinClass.ALL? enmCabinClass.ECONOMY: request.Segments[0].FlightCabinClass;
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

        private async Task<mdlSearchResponse> SearchFromTripJackAsync(mdlSearchRequest request)
        {
            SearchresultWraper mdl = null;
            mdlSearchResponse mdlS = null;
            string tboUrl = _config["TripJack:API:Search"];


            string jsonString = JsonConvert.SerializeObject(SearchRequestMap(request));
            var HaveResponse = GetResponseZip(jsonString, tboUrl);
            {
                if (HaveResponse.Code == 0)
                {
                    mdl = (JsonConvert.DeserializeObject<SearchresultWraper>(HaveResponse.Message));
                }
                if (mdl != null)
                {
                    if (mdl.status.success)//success
                    {
                        if (mdl.searchResult?.tripInfos == null)
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
                        var result = Search_SaveAsync(request, TraceId, mdl.searchResult.tripInfos);
                        List<List<mdlSearchResult>> AllResults = new List<List<mdlSearchResult>>();
                        List<mdlSearchResult> Result1 = new List<mdlSearchResult>();
                        List<mdlSearchResult> Result2 = new List<mdlSearchResult>();
                        List<mdlSearchResult> Result3 = new List<mdlSearchResult>();
                        List<mdlSearchResult> Result4 = new List<mdlSearchResult>();
                        List<mdlSearchResult> Result5 = new List<mdlSearchResult>();
                        List<mdlSearchResult> Result6 = new List<mdlSearchResult>();

                        if (mdl.searchResult.tripInfos != null)
                        {
                            if (mdl.searchResult.tripInfos.ONWARD != null)
                            {
                                Result1.AddRange(SearchResultMap(mdl.searchResult.tripInfos.ONWARD));
                            }
                            if (mdl.searchResult.tripInfos.RETURN != null)
                            {
                                Result2.AddRange(SearchResultMap(mdl.searchResult.tripInfos.RETURN));
                            }
                            if (mdl.searchResult.tripInfos.COMBO != null)
                            {
                                Result1.AddRange(SearchResultMap(mdl.searchResult.tripInfos.COMBO));
                            }
                            if (mdl.searchResult.tripInfos._0 != null)
                            {
                                Result1.AddRange(SearchResultMap(mdl.searchResult.tripInfos._0));
                            }
                            if (mdl.searchResult.tripInfos._1 != null)
                            {
                                Result2.AddRange(SearchResultMap(mdl.searchResult.tripInfos._1));
                            }
                            if (mdl.searchResult.tripInfos._2 != null)
                            {
                                Result3.AddRange(SearchResultMap(mdl.searchResult.tripInfos._2));
                            }
                            if (mdl.searchResult.tripInfos._3 != null)
                            {
                                Result4.AddRange(SearchResultMap(mdl.searchResult.tripInfos._3));
                            }
                            if (mdl.searchResult.tripInfos._4 != null)
                            {
                                Result5.AddRange(SearchResultMap(mdl.searchResult.tripInfos._4));
                            }
                            if (mdl.searchResult.tripInfos._5 != null)
                            {
                                Result6.AddRange(SearchResultMap(mdl.searchResult.tripInfos._5));
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
                            ServiceProvider = enmServiceProvider.TripJack,
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

        private mdlSearchResponse SearchFromDb(mdlSearchRequest request)
        {
            mdlSearchResponse mdlSearchResponse = null;
            DateTime CurrentTime = DateTime.Now;
            tblTripJackTravelDetail Data = null;
            if (request.JourneyType == enmJourneyType.OneWay ||
                request.JourneyType == enmJourneyType.Return ||
                request.JourneyType == enmJourneyType.SpecialReturn
                )
            {
                Data = _context.tblTripJackTravelDetail.Where(p => p.Origin == request.Segments[0].Origin && p.Destination == request.Segments[0].Destination
                  && request.AdultCount == p.AdultCount && p.ChildCount == request.ChildCount && p.InfantCount == request.InfantCount && p.CabinClass == request.Segments[0].FlightCabinClass
                  && p.TravelDate == request.Segments[0].TravelDt
                  && p.ExpireDt > CurrentTime
                ).Include(p => p.tblTripJackTravelDetailResult).OrderByDescending(p => p.ExpireDt).FirstOrDefault();
                if (Data != null)
                {
                    List<List<mdlSearchResult>> AllResults = new List<List<mdlSearchResult>>();
                    var disSegIds = Data.tblTripJackTravelDetailResult.Select(p => p.segmentId).Distinct().OrderBy(p => p);
                    foreach (var d in disSegIds)
                    {
                        AllResults.Add(
                           SearchResultMap(Data.tblTripJackTravelDetailResult.Where(p => p.segmentId == d).Select(p => JsonConvert.DeserializeObject<ONWARD_RETURN_COMBO>(p.JsonData)).ToArray()));
                    }

                    mdlSearchResponse = new mdlSearchResponse()
                    {
                        ServiceProvider = enmServiceProvider.TripJack,
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

        #region *************** Response **********************************

        public class Error
        {
            public string errCode { get; set; }
            public string message { get; set; }
            public string details { get; set; }
        }

        public class SearchresultWraper
        {
            public Searchresult searchResult { get; set; }
            public Status status { get; set; }
            public Metainfo metaInfo { get; set; }
        }


        public class SearchresultMulticity
        {
            public ONWARD_RETURN_COMBO[] tripInfos { get; set; }
        }

        public class Searchresult
        {
            public Tripinfos tripInfos { get; set; }
            public Alert[] alerts { get; set; }
            public Searchquery searchQuery { get; set; }
            public string bookingId { get; set; }
            public Totalpriceinfo totalPriceInfo { get; set; }
            public Status status { get; set; }
            public Conditions conditions { get; set; }
        }

        public class Tripinfos
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
            public string[] messages { get; set; }
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

        public class Metainfo
        {
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


        #endregion


        #endregion

        #region *******************Fare Quote ***************************

        private async Task RemoveFromDb(mdlFareQuotRequest request)
        {
            DateTime currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            var Existing = _context.tblTripJackTravelDetail.Where(p => p.TraceId == request.TraceId && p.ExpireDt > currentDate).FirstOrDefault();
            if (Existing != null)
            {
                _context.Database.ExecuteSqlRaw("delete from tblTripJackTravelDetailResult where TravelDetailId=@p0", Existing.TravelDetailId);
                _context.Database.ExecuteSqlRaw("delete from tblTripJackTravelDetail where TravelDetailId=@p0", Existing.TravelDetailId);
                await _context.SaveChangesAsync();
            }
        }

        private FareQuotRequest FareQuoteRequestMap(mdlFareQuotRequest request)
        {

            FareQuotRequest mdl = new FareQuotRequest()
            {
                priceIds = request.ResultIndex
            };
            return mdl;
        }

        private async Task<mdlFareQuotResponse> FareQuoteFromTripJacAsync(mdlFareQuotRequest request)
        {

            mdlFareQuotResponse mdlS = null;
            FareQuotResponse mdl = null;

            DateTime DepartureDt = DateTime.Now, ArrivalDt = DateTime.Now;


            string tboUrl = _config["TripJack:API:FareQuote"];
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
                    int ServiceProvider = (int)enmServiceProvider.TripJack;
                    if (mdl.tripInfos != null)
                    {
                        Result1.AddRange(SearchResultMap(mdl.tripInfos));
                    }
                    if (Result1.Count() > 0)
                    {
                        AllResults.Add(Result1);
                    }
                    DateTime.TryParse(mdl.searchQuery?.routeInfos?.FirstOrDefault()?.travelDate, out DepartureDt);
                    mdlS = new mdlFareQuotResponse()
                    {

                        ServiceProvider = enmServiceProvider.TripJack,
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

        #endregion

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



        private async Task<mdlFareRuleResponse> FareRuleFromTripJackAsync(mdlFareRuleRequest request)
        {


            mdlFareRuleResponse mdlS = null;
            FareRuleResponse mdl = null;
            string tboUrl = _config["TripJack:API:FareRule"];
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

        private async Task<mdlBookingResponse> BookingFromTripJacAsync(mdlBookingRequest request)
        {

            mdlBookingResponse mdlS = null;
            BookingResponse mdl = null;
            //set the Upper case in pax type


            string tboUrl = _config["TripJack:API:Book"];
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


        #region ********************** Flight Cancelation *************************
        private mdlCancelationRequest CancelRequestMap(mdlCancellationRequest request)
        {
            mdlCancelationRequest mdl = new mdlCancelationRequest()
            {
                bookingId = request.bookingId,
                remarks = request.remarks,
                type= request.type,
                trips = request?.trips.Select(p => new Trip
                {
                    src = p.srcAirport,
                    dest = p.destAirport,
                    departureDate = p.departureDate.ToString("yyyy-MM-dd"),
                    travellers = p.travellers?.Select(q => new Traveller { fn = q.FirstName, ln = q.LastName }).ToArray()
                }).ToArray()
            };
            return mdl;
        }

        public async Task<mdlFlightCancellationChargeResponse> CancelationChargeAsync(mdlCancellationRequest request)
        {

            mdlFlightCancellationChargeResponse mdlS = null;
            TripCancelChargesResponse mdl = null;
            //set the Upper case in pax type

            string tboUrl = _config["TripJack:API:CancellationCharge"];
            string jsonString = System.Text.Json.JsonSerializer.Serialize(CancelRequestMap(request));
            var HaveResponse = GetResponse(jsonString, tboUrl);
            if (HaveResponse.Code == 0)
            {
                mdl = (JsonConvert.DeserializeObject<TripCancelChargesResponse>(HaveResponse.Message));
            }
            if (mdl != null)
            {
                if (mdl?.status.success ?? false)//success
                {
                    mdlS = new mdlFlightCancellationChargeResponse()
                    {
                        status = new mdlStatus()
                        {
                            success = true,
                            httpStatus = 200,
                        },
                        bookingId = mdl.bookingId,
                        trips = mdl.trips?.Select(p => new mdlCancelCharges
                        {
                            airlines = p.airlines,
                            srcAirport = p.src,
                            destAirport = p.dest,
                            departureDate = Convert.ToDateTime(p.departureDate),
                            flightNumbers = p.flightNumbers,
                            amendmentInfo = new mdlAmendmentinfo()
                            {
                                ADULT = new mdlRefundAmount()
                                {
                                    amendmentCharges = p.amendmentInfo?.ADULT?.amendmentCharges ?? 0,
                                    refundAmount = p.amendmentInfo?.ADULT?.refundAmount ?? 0,
                                    totalFare = p.amendmentInfo?.ADULT?.totalFare ?? 0
                                },
                                CHILD = new mdlRefundAmount()
                                {
                                    amendmentCharges = p.amendmentInfo?.CHILD?.amendmentCharges ?? 0,
                                    refundAmount = p.amendmentInfo?.CHILD?.refundAmount ?? 0,
                                    totalFare = p.amendmentInfo?.CHILD?.totalFare ?? 0
                                },
                                INFANT = new mdlRefundAmount()
                                {
                                    amendmentCharges = p.amendmentInfo?.INFANT?.amendmentCharges ?? 0,
                                    refundAmount = p.amendmentInfo?.INFANT?.refundAmount ?? 0,
                                    totalFare = p.amendmentInfo?.INFANT?.totalFare ?? 0
                                },
                            }
                        }).ToList(),
                        ResponseStatus = 1,

                    };
                }
                else
                {
                    mdlS = new mdlFlightCancellationChargeResponse()
                    {
                        ResponseStatus = 2,
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
                mdlS = new mdlFlightCancellationChargeResponse()
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


        public async Task<mdlCancelationDetails> CancelationDetailsAsync(string request)
        {

            mdlCancelationDetails mdlS = null;
            clsCancelationDetails mdl = null;
            //set the Upper case in pax type

            string tboUrl = _config["TripJack:API:CancellationDetail"];
            var requestData = new { amendmentId = request };

            string jsonString = System.Text.Json.JsonSerializer.Serialize(requestData);
            var HaveResponse = GetResponse(jsonString, tboUrl);
            if (HaveResponse.Code == 0)
            {
                mdl = (JsonConvert.DeserializeObject<clsCancelationDetails>(HaveResponse.Message));
            }
            if (mdl != null)
            {
                if (mdl?.status.success ?? false)//success
                {
                    mdlS = new mdlCancelationDetails()
                    {
                        status = new mdlStatus()
                        {
                            success = true,
                            httpStatus = 200,
                        },
                        bookingId = mdl.bookingId,
                        trips = mdl.trips?.Select(p => new mdlCancelTrip
                        {
                            airlines = p.airlines,
                            src = p.src,
                            dest = p.dest,
                            date = p.date,
                            flightNumbers = p.flightNumbers,
                            travellers = p.travellers.Select(q => new mdlCancelTraveller
                            {
                                amendmentCharges = q.amendmentCharges,
                                refundableamount = q.refundAmount,
                                totalFare = q.totalFare,
                                fn = q.fn,
                                ln = q.ln

                            }).ToList(),
                        }).ToList(),
                        ResponseStatus = 1

                    };
                }
                else
                {
                    mdlS = new mdlCancelationDetails()
                    {
                        ResponseStatus = 2,
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
                mdlS = new mdlCancelationDetails()
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

        public async Task<mdlFlightCancellationResponse> CancellationAsync(mdlCancellationRequest request)
        {

            mdlFlightCancellationResponse mdlS = null;
            CancelationResponse mdl = null;
            //set the Upper case in pax type
            string tboUrl = _config["TripJack:API:Cancellation"];
            string jsonString = System.Text.Json.JsonSerializer.Serialize(CancelRequestMap(request));
            var HaveResponse = GetResponse(jsonString, tboUrl);
            if (HaveResponse.Code == 0)
            {
                mdl = (JsonConvert.DeserializeObject<CancelationResponse>(HaveResponse.Message));
            }

            if (mdl != null)
            {
                if (mdl.status.success)//success
                {
                    mdlS = new mdlFlightCancellationResponse()
                    {
                        bookingId = mdl.bookingId,
                        amendmentId = mdl.amendmentId,
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
                    mdlS = new mdlFlightCancellationResponse()
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
                mdlS = new mdlFlightCancellationResponse()
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

        #region ****************** Flight Cancelation classes**********************

        public class mdlCancelationRequest
        {
            public string bookingId { get; set; }
            public string type { get; set; }
            public string remarks { get; set; }
            public Trip[] trips { get; set; }
        }

        public class Trip
        {
            public string src { get; set; }
            public string dest { get; set; }
            public string departureDate { get; set; }
            public Traveller[] travellers { get; set; }
        }

        public class Traveller
        {
            public string fn { get; set; }
            public string ln { get; set; }
            public double amendmentCharges { get; set; }
            public double refundAmount { get; set; }
            public double totalFare { get; set; }
        }



        public class TripCancelChargesResponse
        {
            public string bookingId { get; set; }
            public TripCancelCharges[] trips { get; set; }
            public Status status { get; set; }
            public Error[] errors { get; set; }
            public Metainfo metaInfo { get; set; }
        }

        public class TripCancelCharges
        {
            public string src { get; set; }
            public string dest { get; set; }
            public string departureDate { get; set; }
            public string[] flightNumbers { get; set; }
            public string[] airlines { get; set; }
            public Amendmentinfo amendmentInfo { get; set; }


        }

        public class Amendmentinfo
        {
            public passenger ADULT { get; set; }
            public passenger CHILD { get; set; }
            public passenger INFANT { get; set; }
        }

        public class passenger
        {
            public double amendmentCharges { get; set; }
            public double refundAmount { get; set; }
            public double totalFare { get; set; }
        }


        public class CancelationResponse
        {
            public string bookingId { get; set; }
            public string amendmentId { get; set; }
            public Status status { get; set; }
            public Metainfo metaInfo { get; set; }
            public Error[] errors { get; set; }
        }

        public class clsCancelationDetails
        {
            public string bookingId { get; set; }
            public string amendmentId { get; set; }
            public string amendmentStatus { get; set; }
            public double amendmentCharges { get; set; }
            public double refundableamount { get; set; }
            public CancelTrip[] trips { get; set; }
            public Status status { get; set; }
            public Metainfo metaInfo { get; set; }
            public Error[] errors { get; set; }
        }

        
        public class CancelTrip
        {
            public string src { get; set; }
            public string dest { get; set; }
            public string date { get; set; }
            public string[] flightNumbers { get; set; }
            public string[] airlines { get; set; }
            public Traveller[] travellers { get; set; }
        }

        public class CancelTraveller
        {
            public string fn { get; set; }
            public string ln { get; set; }
            public double amendmentCharges { get; set; }
            public double refundableamount { get; set; }
            public double totalFare { get; set; }
        }
        #endregion
        #endregion

    }
}
