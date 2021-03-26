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
using WingApi.Classes.Database;

namespace WingApi.Classes.TripJack
{
    public interface ITripJack : IWing
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

        public async Task<mdlSearchResponse> SearchAsync(mdlSearchRequest request)
        {
            mdlSearchResponse response = null;
            if (request.JourneyType == enmJourneyType.OneWay)//only Journey TYpe is one way then Fetch from DB else Fetch from tbo
            {
                response = SearchFromDb(request);
                if (response == null)//no data found in Db
                {
                    response = await SearchFromTripJackAsync(request);
                }
            }
            else
            {
                response = await SearchFromTripJackAsync(request);
            }
            return response;
        }

        private mdlSearchResponse SearchFromDb(mdlSearchRequest request)
        {
            mdlSearchResponse mdlSearchResponse = null;
            DateTime CurrentTime = DateTime.Now;
            tblTripJackTravelDetail Data = null;
            if (request.JourneyType == enmJourneyType.OneWay)
            {
                Data = _context.tblTripJackTravelDetail.Where(p => p.Origin == request.Segments[0].Origin && p.Destination == request.Segments[0].Destination
                  && request.AdultCount == p.AdultCount && p.ChildCount == request.ChildCount && p.InfantCount == request.InfantCount && p.CabinClass == request.Segments[0].FlightCabinClass
                  && p.TravelDate == request.Segments[0].TravelDt
                  && p.ExpireDt > CurrentTime
                ).Include(p => p.tblTripJackTravelDetailResult).OrderByDescending(p => p.ExpireDt).FirstOrDefault();
                if (Data != null)
                {
                    List<mdlSearchResult[]> AllResults = new List<mdlSearchResult[]>();
                    List<mdlSearchResult> ResultOB = new List<mdlSearchResult>();
                    List<mdlSearchResult> ResultIB = new List<mdlSearchResult>();
                    foreach (var dt in Data.tblTripJackTravelDetailResult)
                    {
                        if (dt.ResultType == "OB")
                        {
                            ResultOB.Add(System.Text.Json.JsonSerializer.Deserialize<mdlSearchResult>(dt.JsonData));
                        }
                        else if (dt.ResultType == "IB")
                        {
                            ResultIB.Add(System.Text.Json.JsonSerializer.Deserialize<mdlSearchResult>(dt.JsonData)); ;
                        }

                    }
                    AllResults.Add(ResultOB.ToArray());
                    AllResults.Add(ResultIB.ToArray());
                    mdlSearchResponse = new mdlSearchResponse()
                    {
                        ServiceProvider = enmServiceProvider.TripJack,
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




        private async Task<mdlSearchResponse> SearchFromTripJackAsync(mdlSearchRequest request)
        {
            SearchresultWraper mdl = null;
            SearchresultWraperMulticity mdlM = null;
            mdlSearchResponse mdlS = null;
            string tboUrl = _config["TripJack:API:Search"];


            string jsonString = System.Text.Json.JsonSerializer.Serialize(SearchRequestMap(request));
            var HaveResponse = GetResponse(jsonString, tboUrl);

            {
                if (HaveResponse.ErrorCode == 0)
                {
                    mdl = (JsonConvert.DeserializeObject<SearchresultWraper>(HaveResponse.Message));
                }
                if (mdl != null)
                {
                    if (mdl.status.success)//success
                    {

                        List<mdlSearchResult[]> AllResults = new List<mdlSearchResult[]>();
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
                                foreach (var dt in mdl.searchResult.tripInfos.ONWARD)
                                {
                                    Result1.AddRange(SearchResultMap(request.AdultCount, request.ChildCount, request.InfantCount, dt, "OB"));
                                }
                            }
                            if (mdl.searchResult.tripInfos.RETURN != null)
                            {
                                foreach (var dt in mdl.searchResult.tripInfos.RETURN)
                                {
                                    Result2.AddRange(SearchResultMap(request.AdultCount, request.ChildCount, request.InfantCount, dt, "IB"));
                                }
                            }
                            if (mdl.searchResult.tripInfos.COMBO != null)
                            {
                                foreach (var dt in mdl.searchResult.tripInfos.COMBO)
                                {
                                    Result1.AddRange(SearchResultMap(request.AdultCount, request.ChildCount, request.InfantCount, dt, "OB"));
                                }
                            }
                            if (mdl.searchResult.tripInfos._0 != null)
                            {
                                foreach (var dt in mdl.searchResult.tripInfos._0)
                                {
                                    Result1.AddRange(SearchResultMap(request.AdultCount, request.ChildCount, request.InfantCount, dt, "OB"));
                                }
                            }
                            if (mdl.searchResult.tripInfos._1 != null)
                            {
                                foreach (var dt in mdl.searchResult.tripInfos._1)
                                {
                                    Result2.AddRange(SearchResultMap(request.AdultCount, request.ChildCount, request.InfantCount, dt, "OB"));
                                }
                            }
                            if (mdl.searchResult.tripInfos._2 != null)
                            {
                                foreach (var dt in mdl.searchResult.tripInfos._2)
                                {
                                    Result3.AddRange(SearchResultMap(request.AdultCount, request.ChildCount, request.InfantCount, dt, "OB"));
                                }
                            }
                            if (mdl.searchResult.tripInfos._3 != null)
                            {
                                foreach (var dt in mdl.searchResult.tripInfos._3)
                                {
                                    Result4.AddRange(SearchResultMap(request.AdultCount, request.ChildCount, request.InfantCount, dt, "OB"));
                                }
                            }
                            if (mdl.searchResult.tripInfos._4 != null)
                            {
                                foreach (var dt in mdl.searchResult.tripInfos._4)
                                {
                                    Result5.AddRange(SearchResultMap(request.AdultCount, request.ChildCount, request.InfantCount, dt, "OB"));
                                }
                            }
                            if (mdl.searchResult.tripInfos._5 != null)
                            {
                                foreach (var dt in mdl.searchResult.tripInfos._5)
                                {
                                    Result6.AddRange(SearchResultMap(request.AdultCount, request.ChildCount, request.InfantCount, dt, "OB"));
                                }
                            }


                        }
                        if (Result1.Count() > 0)
                        {
                            AllResults.Add(Result1.ToArray());
                        }
                        if (Result2.Count() > 0)
                        {
                            AllResults.Add(Result2.ToArray());
                        }
                        if (Result3.Count() > 0)
                        {
                            AllResults.Add(Result3.ToArray());
                        }
                        if (Result4.Count() > 0)
                        {
                            AllResults.Add(Result4.ToArray());
                        }
                        if (Result5.Count() > 0)
                        {
                            AllResults.Add(Result5.ToArray());
                        }
                        if (Result6.Count() > 0)
                        {
                            AllResults.Add(Result6.ToArray());
                        }


                        mdlS = new mdlSearchResponse()
                        {
                            ServiceProvider = enmServiceProvider.TripJack,
                            TraceId = Guid.NewGuid().ToString(),
                            ResponseStatus = 1,
                            Error = new mdlError()
                            {
                                ErrorCode = 0,
                                ErrorMessage = "-"
                            },
                            Origin = request.Segments[0].Origin,
                            Destination = request.Segments[0].Destination,
                            Results = AllResults.ToArray()
                        };
                        var result = Search_SaveAsync(request, mdlS.TraceId, AllResults);
                        await result;
                    }
                    else
                    {
                        mdlS = new mdlSearchResponse()
                        {
                            ResponseStatus = 3,
                            Error = new mdlError()
                            {
                                ErrorCode = mdl.status.httpStatus,
                                ErrorMessage = "Invalid Request",
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
            }

            return mdlS;
        }


        private mdlSearchResult[] SearchResultMap(int AdultCount, int ChildCount, int InfantCount, ONWARD_RETURN_COMBO sr, string ResultType, Conditions conditions = null)
        {

            List<mdlSearchResult> mdls = new List<mdlSearchResult>();

            var FirstSegment = sr.sI.FirstOrDefault();
            foreach (var price in sr.totalPriceList)
            {
                mdlSearchResult mdl = new mdlSearchResult();
                mdl.IsHoldAllowedWithSSR = conditions?.isBA ?? false;
                mdl.ResultIndex = price.id;
                mdl.ResultType = ResultType;
                mdl.Source = 0;
                if (price.fd.ADULT == null)
                {
                    price.fd.ADULT = price.fd.CHILD;
                }
                if (FirstSegment != null)
                {
                    mdl.IsLCC = FirstSegment.fD.aI.isLcc;
                    mdl.IsRefundable = (price.fd.ADULT.rT == 1 || price.fd.ADULT.rT == 2) ? true : false;
                }
                mdl.IsPanRequiredAtBook = false;
                mdl.IsPanRequiredAtTicket = false;
                mdl.IsPassportRequiredAtBook = conditions?.pcs?.pm ?? false;
                mdl.IsPassportRequiredAtTicket = false;
                mdl.GSTAllowed = false;
                mdl.IsCouponAppilcable = false;
                mdl.IsGSTMandatory = conditions?.gst?.igm ?? false;
                mdl.AirlineRemark = price.messages;
                mdl.ResultFareType = price.fareIdentifier;
                mdl.TicketAdvisory = null;
                mdl.AirlineCode = FirstSegment.fD.aI.code;
                mdl.ValidatingAirline = FirstSegment.fD.aI.code;
                mdl.IsUpsellAllowed = false;
                mdl.Fare = new mdlFare();


                List<mdlFarebreakdown> mdlFarebreakdowns = new List<mdlFarebreakdown>();
                mdlFarebreakdown AdultFare = new mdlFarebreakdown()
                {
                    Currency = "INR",
                    PassengerType = enmPassengerType.Adult,
                    PassengerCount = AdultCount,
                    BaseFare = price.fd.ADULT.fC.BF,
                    Tax = price.fd.ADULT.fC.TAF,
                    YQTax = price.fd.ADULT.afC.TAF.YQ,
                    AdditionalTxnFeeOfrd = 0,
                    AdditionalTxnFeePub = 0,
                    PGCharge = 0,
                    SupplierReissueCharges = 0,
                };
                mdlFarebreakdowns.Add(AdultFare);
                if (price.fd.CHILD != null)
                {
                    mdlFarebreakdown ChildFare = new mdlFarebreakdown()
                    {
                        Currency = "INR",
                        PassengerType = enmPassengerType.Child,
                        PassengerCount = ChildCount,
                        BaseFare = price.fd.CHILD.fC.BF,
                        Tax = price.fd.CHILD.fC.TAF,
                        YQTax = price.fd.CHILD.afC.TAF.YQ,
                        AdditionalTxnFeeOfrd = 0,
                        AdditionalTxnFeePub = 0,
                        PGCharge = 0,
                        SupplierReissueCharges = 0,
                    };
                }
                if (price.fd.INFANT != null)
                {
                    mdlFarebreakdown InfantFare = new mdlFarebreakdown()
                    {
                        Currency = "INR",
                        PassengerType = enmPassengerType.Infant,
                        PassengerCount = InfantCount,
                        BaseFare = price.fd.INFANT.fC.BF,
                        Tax = price.fd.INFANT.fC.TAF,
                        YQTax = price.fd.INFANT.afC.TAF.YQ,
                        AdditionalTxnFeeOfrd = 0,
                        AdditionalTxnFeePub = 0,
                        PGCharge = 0,
                        SupplierReissueCharges = 0,
                    };
                }

                mdl.FareBreakdown = mdlFarebreakdowns.ToArray();

                List<Passenger> passengers = new List<Passenger>();
                passengers.Add(price.fd.ADULT);
                passengers.Add(price.fd.CHILD);
                passengers.Add(price.fd.INFANT);

                List<mdlTaxbreakup> mdlTaxbreakups = new List<mdlTaxbreakup>();
                mdlTaxbreakups.Add(new mdlTaxbreakup() { key = "MF", value = passengers.Select(p => p.afC.TAF.MF).Sum() });
                mdlTaxbreakups.Add(new mdlTaxbreakup() { key = "OT", value = passengers.Select(p => p.afC.TAF.OT).Sum() });
                mdlTaxbreakups.Add(new mdlTaxbreakup() { key = "MFT", value = passengers.Select(p => p.afC.TAF.MFT).Sum() });
                mdlTaxbreakups.Add(new mdlTaxbreakup() { key = "AGST", value = passengers.Select(p => p.afC.TAF.AGST).Sum() });
                mdlTaxbreakups.Add(new mdlTaxbreakup() { key = "YQ", value = passengers.Select(p => p.afC.TAF.YQ).Sum() });
                mdlTaxbreakups.Add(new mdlTaxbreakup() { key = "YR", value = passengers.Select(p => p.afC.TAF.YR).Sum() });
                mdlTaxbreakups.Add(new mdlTaxbreakup() { key = "WO", value = passengers.Select(p => p.afC.TAF.WO).Sum() });



                mdl.Fare.Currency = "INR";
                mdl.Fare.BaseFare = mdlFarebreakdowns.Select(p => p.BaseFare).Sum();
                mdl.Fare.Tax = mdlFarebreakdowns.Select(p => p.Tax).Sum();
                mdl.Fare.TaxBreakup = mdlTaxbreakups.ToArray();
                mdl.Fare.YQTax = mdlFarebreakdowns.Select(p => p.YQTax).Sum();
                mdl.Fare.AdditionalTxnFeeOfrd = 0;
                mdl.Fare.AdditionalTxnFeePub = 0;
                mdl.Fare.PGCharge = 0;
                mdl.Fare.OtherCharges = 0;
                mdl.Fare.ChargeBU = null;
                mdl.Fare.Discount = 0;
                mdl.Fare.PublishedFare = passengers.Select(p => p.fC.TF).Sum();
                mdl.Fare.CommissionEarned = passengers.Select(p => p.fC.NCM).Sum();
                mdl.Fare.PLBEarned = 0;
                mdl.Fare.IncentiveEarned = 0;
                mdl.Fare.OfferedFare = passengers.Select(p => p.fC.NF).Sum();

                mdl.Fare.TdsOnCommission = 0;
                mdl.Fare.TdsOnPLB = 0;
                mdl.Fare.TdsOnIncentive = 0;
                mdl.Fare.ServiceFee = 0;
                mdl.Fare.TotalBaggageCharges = passengers.Select(p => p.fC.SSRP).Sum();
                mdl.Fare.TotalMealCharges = 0;
                mdl.Fare.TotalSeatCharges = 0;
                mdl.Fare.TotalSpecialServiceCharges = 0;
                //mdl.LastTicketDate = sr.LastTicketDate;            
                List<mdlSegmentResponse> SegmentsResponse = new List<mdlSegmentResponse>();
                foreach (var sg in sr.sI)
                {
                    mdlSegmentResponse segmentsRespons = new mdlSegmentResponse();
                    segmentsRespons.Baggage = price.fd.ADULT.bI.iB;
                    segmentsRespons.Baggage = price.fd.ADULT.bI.cB;

                    segmentsRespons.CabinClass = (enmCabinClass)Enum.Parse(typeof(enmCabinClass), price.fd.ADULT.cc, true);

                    segmentsRespons.TripIndicator = 1;
                    segmentsRespons.SegmentIndicator = sg.sN;
                    segmentsRespons.Airline = new mdlAirline()
                    {
                        AirlineCode = sg?.fD?.aI?.code,
                        AirlineName = sg?.fD?.aI?.name,
                        FlightNumber = sg?.fD?.fN,
                        FareClass = price?.fd?.ADULT?.cB,
                        OperatingCarrier = sg?.oB?.name,
                    };
                    segmentsRespons.NoOfSeatAvailable = price.fd.ADULT.sR;

                    DateTime _DepTime = DateTime.Now;
                    DateTime.TryParse(sg.dt, out _DepTime);
                    segmentsRespons.Origin = new mdlOrigin()
                    {
                        Airport = new mdlAirport()
                        {
                            AirportCode = sg?.da?.code,
                            AirportName = sg?.da?.name,
                            Terminal = sg?.da?.terminal,
                            CityCode = sg?.da?.cityCode,
                            CityName = sg?.da?.city,
                            CountryCode = sg?.da?.countryCode,
                            CountryName = sg?.da?.country,
                        },
                        DepTime = _DepTime,
                    };
                    DateTime _ArrTime = DateTime.Now;
                    DateTime.TryParse(sg.at, out _ArrTime);
                    segmentsRespons.Destination = new mdlDestination()
                    {
                        Airport = new mdlAirport()
                        {
                            AirportCode = sg?.da?.code,
                            AirportName = sg?.da?.name,
                            Terminal = sg?.da?.terminal,
                            CityCode = sg?.da?.cityCode,
                            CityName = sg?.da?.city,
                            CountryCode = sg?.da?.countryCode,
                            CountryName = sg?.da?.country,
                        },
                        ArrTime = _ArrTime
                    };

                    segmentsRespons.Duration = sg.duration;
                    segmentsRespons.GroundTime = 0;
                    segmentsRespons.Mile = 0;
                    segmentsRespons.StopOver = false;
                    segmentsRespons.FlightInfoIndex = string.Empty;
                    segmentsRespons.StopPoint = string.Empty;
                    segmentsRespons.StopPointArrivalTime = _ArrTime;
                    segmentsRespons.StopPointDepartureTime = _DepTime;
                    segmentsRespons.Craft = string.Empty;
                    segmentsRespons.Remark = string.Empty;
                    segmentsRespons.IsETicketEligible = true;
                    segmentsRespons.FlightStatus = string.Empty;
                    segmentsRespons.Status = string.Empty;
                    segmentsRespons.AccumulatedDuration = sg.duration;
                    SegmentsResponse.Add(segmentsRespons);
                }

                List<mdlSegmentResponse[]> _mdlSegmentResponses = new List<mdlSegmentResponse[]>();
                _mdlSegmentResponses.Add(SegmentsResponse.ToArray());
                mdl.Segments = _mdlSegmentResponses.ToArray();
                mdls.Add(mdl);
            }

            return mdls.ToArray();// mdls;


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
                    isConnectingFlight = !request.DirectFlight
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



        private async Task<bool> Search_SaveAsync(mdlSearchRequest request, string TraceId, List<mdlSearchResult[]> IBresponse)
        {

            int ExpirationMinute = 14;
            int.TryParse(_config["TripJack:TraceIdExpiryTime"], out ExpirationMinute);
            double minFare = 0;
            List<tblTripJackTravelDetailResult> mdlDetail = new List<tblTripJackTravelDetailResult>();
            foreach (var Res in IBresponse)
            {
                minFare = minFare + Res.Min(p => p.Fare.PublishedFare);
                mdlDetail.AddRange(Res.Select(p => new tblTripJackTravelDetailResult { ResultIndex = p.ResultIndex, ResultType = p.ResultType, OfferedFare = p.Fare.OfferedFare, PublishedFare = p.Fare.PublishedFare, JsonData = System.Text.Json.JsonSerializer.Serialize(p) }).ToList());
            }
            DateTime TickGeration = DateTime.Now;
            tblTripJackTravelDetail td = new tblTripJackTravelDetail()
            {
                TravelDate = request.Segments[0].TravelDt,
                CabinClass = request.Segments[0].FlightCabinClass,
                Origin = request.Segments[0].Origin,
                Destination = request.Segments[0].Destination,
                MinPublishFare = minFare,
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


        private async Task RemoveFromDb(mdlFareQuotRequest request)
        {
            DateTime currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            var Existing = _context.tblTripJackTravelDetail.Where(p => p.TraceId == request.TraceId && p.ExpireDt > currentDate).FirstOrDefault();
            if (Existing != null)
            {
                _context.Database.ExecuteSqlRaw("delete from tblTripJackTravelDetailResult where TravelDetailId=@p0", parameters: new[] { Existing.TravelDetailId });
                _context.Database.ExecuteSqlRaw("delete from tblTripJackTravelDetail where TravelDetailId=@p0", parameters: new[] { Existing.TravelDetailId });
                await _context.SaveChangesAsync();
            }
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

            string tboUrl = _config["TripJack:API:FareQuote"];
            string jsonString = System.Text.Json.JsonSerializer.Serialize(FareQuoteRequestMap(request));
            var HaveResponse = GetResponse(jsonString, tboUrl);
            if (HaveResponse.ErrorCode == 0)
            {
                mdl = (System.Text.Json.JsonSerializer.Deserialize<FareQuotResponse>(HaveResponse.Message));
            }

            if (mdl != null)
            {
                if (mdl.status.success)//success
                {
                    List<mdlSearchResult[]> AllResults = new List<mdlSearchResult[]>();
                    List<mdlSearchResult> ResultOB = new List<mdlSearchResult>();
                    List<mdlSearchResult> ResultIB = new List<mdlSearchResult>();

                    if (mdl.tripInfos != null)
                    {
                        int AdultCount = 0, ChildCount = 0, InfantCount = 0;
                        AdultCount = mdl.searchQuery?.paxInfo?.ADULT ?? 0;
                        ChildCount = mdl.searchQuery?.paxInfo?.CHILD ?? 0;
                        InfantCount = mdl.searchQuery?.paxInfo?.INFANT ?? 0;

                        foreach (var dt in mdl.tripInfos)
                        {
                            ResultOB.AddRange(SearchResultMap(AdultCount, ChildCount, InfantCount, dt, "OB"));
                        }


                    }
                    if (ResultOB.Count > 0)
                    {
                        AllResults.Add(ResultOB.ToArray());
                    }
                    if (ResultIB.Count > 0)
                    {
                        AllResults.Add(ResultIB.ToArray());
                    }
                    mdlS = new mdlFareQuotResponse()
                    {

                        ServiceProvider = enmServiceProvider.TripJack,
                        TraceId = Guid.NewGuid().ToString(),
                        ResponseStatus = 1,
                        Error = new mdlError()
                        {
                            ErrorCode = 0,
                            ErrorMessage = ""
                        },
                        Origin = mdl.searchQuery.routeInfos.FirstOrDefault()?.fromCityOrAirport.code,
                        Destination = mdl.searchQuery.routeInfos.FirstOrDefault()?.toCityOrAirport.code,
                        Results = AllResults.ToArray()
                    };
                }
                else
                {
                    mdlS = new mdlFareQuotResponse()
                    {
                        ResponseStatus = 3,
                        Error = new mdlError()
                        {
                            ErrorCode = mdl.status.httpStatus,
                            ErrorMessage = mdl.errors?.FirstOrDefault()?.message,
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
                        ErrorCode = 100,
                        ErrorMessage = "Unable to Process",
                    }
                };
            }

            return mdlS;
        }


        #region *******************Fare Rule Function ****************************
        private FareRuleRequest FareRuleRequestMap(mdlFareRuleRequest request)
        {
            FareRuleRequest mdl = new FareRuleRequest()
            {
                flowType = "SEARCH",                
                id= request.ResultIndex.FirstOrDefault()                
            };
            return mdl;
        }

        private async Task<mdlFareRuleResponse> FareRuleFromDbAsync(mdlFareRuleRequest request)
        {
            mdlFareRuleResponse mdl = null;

            var result = _context.tblTripJackFareRule.Where(p => p.TraceId == request.TraceId && p.ResultIndex == request.ResultIndex.FirstOrDefault()).OrderByDescending(p => p.GenrationDt).FirstOrDefault();
            if (result != null)
            {
                mdl = JsonConvert.DeserializeObject<mdlFareRuleResponse>(result.JsonData);
            }
            await _context.Database.ExecuteSqlRawAsync("delete from tblTripJackFareRule where GenrationDt<@p0", parameters: new[] { DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") });
            await _context.SaveChangesAsync();
            return mdl;
        }

        private async Task FareRule_SaveAsync(mdlFareRuleRequest request, mdlFareRuleResponse mdl)
        {
            if (mdl != null)
            {
            }
            var result = _context.tblTripJackFareRule.Add(new tblTripJackFareRule()
            {
                GenrationDt = DateTime.Now,
                ResultIndex = request.ResultIndex.FirstOrDefault(),
                JsonData = JsonConvert.SerializeObject(mdl),
                TraceId = request.TraceId
            });
            await _context.SaveChangesAsync();
        }

        private async Task<mdlFareRuleResponse> FareRuleFromTripJackAsync(mdlFareRuleRequest request)
        {

            
            mdlFareRuleResponse mdlS = null;
            //string tboUrl = _config["TBO:API:FareRule"];
            
            //string jsonString = System.Text.Json.JsonSerializer.Serialize(FareRuleRequestMap(request));
            //var HaveResponse = GetResponse(jsonString, tboUrl);
            //if (HaveResponse.ErrorCode == 0)
            //{
            //    dynamic mdlTemp = JValue.Parse(HaveResponse.Message);
            //    if (mdlTemp.status.success)
            //    {
            //    }
            //    else
            //    {
            //        mdlS = new mdlFareRuleResponse()
            //        {
            //            ResponseStatus = 3,
            //            Error = new mdlError()
            //            {
            //                ErrorCode = mdlTemp.errors.errCode??0,
            //                ErrorMessage = mdlTemp.errors.message,
            //            }
            //        };
            //    }


            //}


            //if (mdl != null)
            //{
            //    if (mdl.ResponseStatus == 1)//success
            //    {
            //        List<mdlFarerule> mdlFareRules = new List<mdlFarerule>();
            //        foreach (var r in mdl.FareRules)
            //        {
            //            mdlFareRules.Add(new mdlFarerule()
            //            {
            //                Airline = r.Airline,
            //                FlightId = r.FlightId,
            //                Origin = r.Origin,
            //                Destination = r.Destination,
            //                FareBasisCode = r.FareBasisCode,
            //                FareRuleDetail = r.FareRuleDetail,
            //                FareRestriction = r.FareRestriction,
            //                FareFamilyCode = r.FareFamilyCode,
            //                FareRuleIndex = r.FareRuleIndex,
            //                DepartureTime = r.DepartureTime,
            //                ReturnDate = r.ReturnDate
            //            });
            //        }

            //        mdlS = new mdlFareRuleResponse()
            //        {
            //            Error = new mdlError()
            //            {
            //                ErrorCode = mdl.Error.ErrorCode,
            //                ErrorMessage = mdl.Error.ErrorMessage,
            //            },
            //            FareRules = mdlFareRules.ToArray(),
            //            ResponseStatus = mdl.ResponseStatus

            //        };
            //        await FareRule_SaveAsync(request, mdlS);
            //    }
            //    else
            //    {
            //        mdlS = new mdlFareRuleResponse()
            //        {
            //            ResponseStatus = 3,
            //            Error = new mdlError()
            //            {
            //                ErrorCode = mdl.Error.ErrorCode,
            //                ErrorMessage = mdl.Error.ErrorMessage,
            //            }
            //        };
            //    }

            //}
            //else
            //{
            //    mdlS = new mdlFareRuleResponse()
            //    {
            //        ResponseStatus = 100,
            //        Error = new mdlError()
            //        {
            //            ErrorCode = 100,
            //            ErrorMessage = "Unable to Process",
            //        }
            //    };
            //}

            return mdlS;
        }

        public async Task<mdlFareRuleResponse> FareRuleAsync(mdlFareRuleRequest request)
        {
            mdlFareRuleResponse response = null;

            //response = await FareRuleFromDbAsync(request);
            //if (response == null)//no data found in Db
            //{
            //    response = await FareRuleFromTboAsync(request);
            //}

            return response;
        }
        #endregion




        #region *******************Search Class***************************
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

        public class SearchresultWraperMulticity
        {
            public SearchresultMulticity searchResult { get; set; }
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
            public string dt { get; set; }
            public string at { get; set; }
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
            public object[] msri { get; set; }
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


        #region *******************Fare Quote Class***************************
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

        #region ******************************* Fare Rule ******************************


        public class FareRuleRequest
        {
            public string id { get; set; }
            public string flowType { get; set; }
        }

        
        #endregion

    }
}
