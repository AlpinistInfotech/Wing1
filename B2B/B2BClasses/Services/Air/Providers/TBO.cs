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
	public interface ITBO : IWingFlight
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
				//request.Headers.Add("Accept-Encoding", "gzip");
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
				using (Stream stream = rsp)
				{
					using (var reader = new StreamReader(stream))
					{
						mdl.Code = 0;
						mdl.Message = reader.ReadToEnd();
					}
				}
				//using (StreamReader readStream = new StreamReader(new GZipStream(rsp, CompressionMode.Decompress)))
				//{
				//    mdl.Code = 0;
				//    mdl.Message = rsp.ReadToEnd();//JsonConvert.DeserializeXmlNode(readStream.ReadToEnd(), "root").InnerXml;
				//}
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
			//Add Provider Previx in Result index
			int tboid = (int)enmServiceProvider.TBO;
			response.Results?.ForEach(p =>
			{
				p.ForEach(q => q.TotalPriceList.ForEach(r => r.ResultIndex = "" + tboid + "_" + r.ResultIndex));
			});

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

		#region ********************* Login Classes ***********************


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
					ErrorCode = 1,
					ErrorMessage = "Invalid Login",
				};
			}

			return mdl;
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


		//#if (false)
		#region *********************** Fare Quotation **********************

		public class FareQuotRequest
		{
			public string EndUserIp { get; set; }
			public string TokenId { get; set; }
			public string TraceId { get; set; }
			public string ResultIndex { get; set; }
		}

		public class FareQuotResponseWraper
		{
			public FareQuotResponse Response { get; set; }
		}
		public class FareQuotResponse
		{
			public int ResponseStatus { get; set; }
			public bool IsPriceChanged { get; set; }
			public Error Error { get; set; }
			public string TraceId { get; set; }
			public string Origin { get; set; }
			public string Destination { get; set; }
			public SearchResult Results { get; set; }
			public string bookingId { get; set; }
		}

		#endregion
		#region fare rule
		public class FareRuleRequest : FareQuotRequest
		{

		}
		public class FareRuleResponseWraper
		{
			public FareRuleResponse Response { get; set; }
		}

		public class FareRuleResponse
		{
			public Error Error { get; set; }
			public Farerule[] FareRules { get; set; }
			public int ResponseStatus { get; set; }
			public string TraceId { get; set; }
		}

		private FareQuotRequest FareQuoteRequestMap(mdlFareQuotRequest request, string TokenId)
		{
			FareQuotRequest mdl = new FareQuotRequest()
			{
				EndUserIp = "::1",
				TokenId = TokenId,
				TraceId = request.TraceId,
				ResultIndex = request.ResultIndex.FirstOrDefault()
			};
			return mdl;
		}

		private async Task<mdlFareQuotResponse> FareQuoteFromTboAsync(mdlFareQuotRequest request)
		{

			int MaxLoginAttempt = 1, LoginAttempt = 0;
			mdlFareQuotResponse mdlS = null;
			FareQuotResponse mdl = null;
			FareQuotResponseWraper mdlTemp = null;

			

			string tboUrl = _config["TBO:API:FareQuote"];
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

			string jsonString = System.Text.Json.JsonSerializer.Serialize(FareQuoteRequestMap(request, TokenDetails.TokenId));
			var HaveResponse = GetResponse(jsonString, tboUrl);
			if (HaveResponse.Code == 0)
			{
				mdlTemp = (System.Text.Json.JsonSerializer.Deserialize<FareQuotResponseWraper>(HaveResponse.Message));
				if (mdlTemp != null)
				{
					mdl = mdlTemp.Response;
				}
			}

			if (mdl != null)
			{
				if (mdl.ResponseStatus == 1)//success
				{
					var tempdata = mdl.Results;
					List<List<mdlSearchResult>> AllResults = new List<List<mdlSearchResult>>();
					List<mdlSearchResult> ResultOB = new List<mdlSearchResult>();
					List<mdlSearchResult> ResultIB = new List<mdlSearchResult>();
					int ServiceProvider = (int)enmServiceProvider.TripJack;
					
					{
						if (tempdata.ResultIndex.Contains("OB"))
						{
							ResultOB.Add(SearchResultMap(tempdata, "OB"));
						}
						else if (tempdata.ResultIndex.Contains("IB"))
						{
							ResultIB.Add(SearchResultMap(tempdata, "IB"));
						}
						else
						{
							ResultOB.Add(SearchResultMap(tempdata, "OB"));
						}
					}
					AllResults.Add(ResultOB);
					AllResults.Add(ResultIB);
					DateTime DepartureDt = DateTime.Now, ArrivalDt = DateTime.Now;
					DateTime.TryParse(mdl.Results?.LastTicketDate, out DepartureDt);
					mdlS = new mdlFareQuotResponse()
					{
						ServiceProvider = enmServiceProvider.TBO,
						TraceId = mdl.TraceId,
						BookingId = ServiceProvider + "_" + mdl.bookingId,
						ResponseStatus = 1,
						Error = new mdlError()
						{
							Code = 0,
							Message = "-"
						},
						Origin = mdl.Results?.FareRules?.FirstOrDefault()?.Origin ?? string.Empty,
						Destination = mdl.Results?.FareRules?.FirstOrDefault()?.Destination ?? string.Empty,
						Results = AllResults,

                        TotalPriceInfo = new mdlTotalPriceInfo()
                        {
                            BaseFare = mdl.Results?.Fare?.BaseFare ?? 0,
                            TaxAndFees = mdl.Results?.Fare?.Tax ?? 0,
                            TotalFare = mdl.Results?.Fare?.PublishedFare ?? 0,
                        },
                        SearchQuery = new Models.mdlFlightSearchWraper()
                        {
                            AdultCount = mdl.Results.FareBreakdown.Where(x=>x.PassengerType==enmPassengerType.Adult).Select(x=>x.PassengerCount).FirstOrDefault(),
                            ChildCount = mdl.Results.FareBreakdown.Where(x => x.PassengerType == enmPassengerType.Child).Select(x => x.PassengerCount).FirstOrDefault(),
                            InfantCount = mdl.Results.FareBreakdown.Where(x => x.PassengerType == enmPassengerType.Infant).Select(x => x.PassengerCount).FirstOrDefault(),
							CabinClass = (enmCabinClass)Enum.Parse(typeof(enmCabinClass), mdl.Results?.Segments[0][0]?.CabinClass.ToString() ?? (nameof(enmCabinClass.ECONOMY)), true),
                            JourneyType = enmJourneyType.OneWay,
                            DepartureDt = DepartureDt,
                            From = mdl.Results?.FareRules?.FirstOrDefault()?.Origin??string.Empty,
                            To = mdl.Results?.FareRules?.FirstOrDefault()?.Destination ?? string.Empty,
						},
                        FareQuoteCondition = new mdlFareQuoteCondition()
                        {
                            dob = new mdlDobCondition()
                            {
                                adobr = false,
                                cdobr =  false,
                                idobr =false,
                            },
                            GstCondition = new mdlGstCondition()
                            {
                                IsGstMandatory = mdl.Results?.IsGSTMandatory ?? false,
                                IsGstApplicable = mdl.Results?.GSTAllowed ?? true,
                            },
                            IsHoldApplicable = mdl.Results?.IsHoldAllowed ?? false,
                            PassportCondition = new mdlPassportCondition()
                            {
                                IsPassportExpiryDate =  false,
                                isPassportIssueDate = false,
                                isPassportRequired = mdl.Results?.IsPassportRequiredAtBook?? false,
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
							Code = mdl.Error.ErrorCode,
							Message = mdl.Error.ErrorMessage,
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


		public async Task<mdlFareQuotResponse> FareQuoteAsync(mdlFareQuotRequest request)
		{
			mdlFareQuotResponse mdl = await FareQuoteFromTboAsync(request);
			if (mdl.IsPriceChanged)
			{
				await RemoveFromDb(request);
			}
			return mdl;
		}

		private FareRuleRequest FareRuleRequestMap(mdlFareRuleRequest request, string TokenId)
		{
			FareRuleRequest mdl = new FareRuleRequest()
			{
				EndUserIp = "157.37.219.253",
				TokenId = TokenId,
				TraceId = request.TraceId,
				ResultIndex = request.ResultIndex.FirstOrDefault()
			};
			return mdl;
		}

		private async Task<mdlFareRuleResponse> FareRuleFromDbAsync(mdlFareRuleRequest request)
		{
			mdlFareRuleResponse mdl = null;

			var result = _context.tblTboFareRule.Where(p => p.TraceId == request.TraceId && p.ResultIndex == request.ResultIndex.FirstOrDefault()).OrderByDescending(p => p.GenrationDt).FirstOrDefault();
			if (result != null)
			{
				mdl = JsonConvert.DeserializeObject<mdlFareRuleResponse>(result.JsonData);
			}
			await _context.Database.ExecuteSqlRawAsync("delete from tblTboFareRule where GenrationDt<@p0", parameters: new[] { DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") });
			await _context.SaveChangesAsync();
			return mdl;
		}

		private async Task FareRule_SaveAsync(mdlFareRuleRequest request, mdlFareRuleResponse mdl)
		{
			if (mdl != null)
			{
			}
			var result = _context.tblTboFareRule.Add(new tblTboFareRule()
			{
				GenrationDt = DateTime.Now,
				ResultIndex = request.ResultIndex.FirstOrDefault(),
				JsonData = JsonConvert.SerializeObject(mdl),
				TraceId = request.TraceId
			});
			await _context.SaveChangesAsync();
		}

		private async Task<mdlFareRuleResponse> FareRuleFromTboAsync(mdlFareRuleRequest request)
		{

			int MaxLoginAttempt = 1, LoginAttempt = 0;
			mdlFareRuleResponse mdlS = null;
			FareRuleResponse mdl = null;
			FareRuleResponseWraper mdlTemp = null;
			string tboUrl = _config["TBO:API:FareRule"];
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

			string jsonString = System.Text.Json.JsonSerializer.Serialize(FareRuleRequestMap(request, TokenDetails.TokenId));
			var HaveResponse = GetResponse(jsonString, tboUrl);
			if (HaveResponse.Code == 0)
			{
				mdlTemp = (System.Text.Json.JsonSerializer.Deserialize<FareRuleResponseWraper>(HaveResponse.Message));
				if (mdlTemp != null)
				{
					mdl = mdlTemp.Response;
				}
			}


			if (mdl != null)
			{
				if (mdl.ResponseStatus == 1)//success
				{
					List<mdlFareRule> mdlFareRules = new List<mdlFareRule>();
					foreach (var r in mdl.FareRules)
					{
						mdlFareRules.Add(new mdlFareRule()
						{
							Airline = r.Airline,
							FlightId = r.FlightId.ToString(),
							Origin = r.Origin,
							Destination = r.Destination,
							FareBasisCode = r.FareBasisCode,
							FareRuleDetail = r.FareRuleDetail,
							FareRestriction = r.FareRestriction,
							FareFamilyCode = r.FareFamilyCode,
							FareRuleIndex = r.FareRuleIndex,
							DepartureTime = r.DepartureTime.ToString("dd-MMM-yyyy HH:mm:ss"),
							ReturnDate = r.ReturnDate.ToString("dd-MMM-yyyy HH:mm:ss")
						});
					}

					mdlS = new mdlFareRuleResponse()
					{
						Error = new mdlError()
						{
							Code = mdl.Error.ErrorCode,
							Message = mdl.Error.ErrorMessage,
						},
						tboFareRule = mdlFareRules.ToArray(),
						ResponseStatus = mdl.ResponseStatus

					};
					await FareRule_SaveAsync(request, mdlS);
				}
				else
				{
					mdlS = new mdlFareRuleResponse()
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
				mdlS = new mdlFareRuleResponse()
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

		public async Task<mdlFareRuleResponse> FareRuleAsync(mdlFareRuleRequest request)
		{
			mdlFareRuleResponse response = null;

			response = await FareRuleFromDbAsync(request);
			if (response == null)//no data found in Db
			{
				response = await FareRuleFromTboAsync(request);
			}

			return response;
		}
		#endregion


		#region *******************Search Function***************************

		private mdlSearchResult SearchResultMap(SearchResult sr, string ResultType)
		{

			mdlSearchResult mdls = new mdlSearchResult();
			List<mdlSegment> lmseg = new List<mdlSegment>();
			List<mdlTotalpricelist> TotalPriceList = new List<mdlTotalpricelist>();
			mdls.ServiceProvider = enmServiceProvider.TBO;
			foreach (var sg in sr.Segments)
			{


				lmseg.AddRange(sg.Select(p => new mdlSegment
				{

					Airline = new mdlAirline()
					{
						Code = p.Airline.AirlineCode,
						Name = p.Airline.AirlineName,
						FlightNumber = p.Airline.FlightNumber,
						OperatingCarrier = p.Airline.OperatingCarrier,
					},
					Origin = new mdlAirport()
					{
						AirportCode = p.Origin.Airport.AirportCode ?? string.Empty,
						AirportName = p.Origin.Airport.AirportName ?? string.Empty,
						CityCode = p.Origin.Airport.CityCode ?? string.Empty,
						CityName = p.Origin.Airport.CityName ?? string.Empty,
						CountryCode = p.Origin.Airport.CountryCode ?? string.Empty,
						CountryName = p.Origin.Airport.CountryName ?? string.Empty,
						Terminal = p.Origin.Airport.Terminal ?? string.Empty
					},
					Destination = new mdlAirport()
					{
						AirportCode = p.Destination.Airport.AirportCode ?? string.Empty,
						AirportName = p.Destination.Airport.AirportName ?? string.Empty,
						CityCode = p.Destination.Airport.CityCode ?? string.Empty,
						CityName = p.Destination.Airport.CityName ?? string.Empty,
						CountryCode = p.Destination.Airport.CountryCode ?? string.Empty,
						CountryName = p.Destination.Airport.CountryName ?? string.Empty,
						Terminal = p.Destination.Airport.Terminal ?? string.Empty
					},
					ArrivalTime = p.Destination.ArrTime,
					DepartureTime = p.Origin.DepTime,
					Duration = p.Duration,
					Mile = p.Mile,
					TripIndicator = p.TripIndicator,
					BaggageInformation = new mdlBaggageInformation()
					{
						CabinBaggage = p.CabinBaggage,
						CheckingBaggage = p.Baggage
					},
					CabinClass = p.CabinClass,
					ClassOfBooking = p.Airline.FareClass,
					FareBasis = string.Empty,
					IsFreeMeel = false,
					RefundableType = sr.IsRefundable == true ? 1 : 0,
					SeatRemaing = p.NoOfSeatAvailable,
				}).ToList());

				TotalPriceList = sr.FareBreakdown.Select(q => new mdlTotalpricelist
				{
					fareIdentifier = string.Empty,
					ResultIndex = sr.ResultIndex,
					sri = string.Empty,
					msri = null,
					ADULT = new mdlPassenger()
					{
						FareComponent = new mdlFareComponent()
						{
							BaseFare = (int)q.PassengerType == 1 ? q.BaseFare : 0,
							IGST = (int)q.PassengerType == 1 ? q.Tax : 0,
							TaxAndFees = (int)q.PassengerType == 1 ? q.YQTax + q.AdditionalTxnFeeOfrd + q.AdditionalTxnFeePub + q.PGCharge + q.SupplierReissueCharges : 0,
							TotalFare = (int)q.PassengerType == 1 ? q.Tax + q.BaseFare + q.YQTax + q.AdditionalTxnFeeOfrd + q.AdditionalTxnFeePub + q.PGCharge + q.SupplierReissueCharges : 0,
							NetFare = (int)q.PassengerType == 1 ? q.Tax + q.BaseFare + q.YQTax + q.AdditionalTxnFeeOfrd + q.AdditionalTxnFeePub + q.PGCharge + q.SupplierReissueCharges : 0,
						},

					},
					CHILD = new mdlPassenger()
					{
						FareComponent = new mdlFareComponent()
						{
							BaseFare = (int)q.PassengerType == 2 ? q.BaseFare : 0,
							IGST = (int)q.PassengerType == 2 ? q.Tax : 0,
							TaxAndFees = (int)q.PassengerType == 2 ? q.YQTax + q.AdditionalTxnFeeOfrd + q.AdditionalTxnFeePub + q.PGCharge + q.SupplierReissueCharges : 0,
							TotalFare = (int)q.PassengerType == 2 ? q.Tax + q.BaseFare + q.YQTax + q.AdditionalTxnFeeOfrd + q.AdditionalTxnFeePub + q.PGCharge + q.SupplierReissueCharges : 0,
							NetFare = (int)q.PassengerType == 2 ? q.Tax + q.BaseFare + q.YQTax + q.AdditionalTxnFeeOfrd + q.AdditionalTxnFeePub + q.PGCharge + q.SupplierReissueCharges : 0,
						},
					},
					INFANT = new mdlPassenger()
					{
						FareComponent = new mdlFareComponent()
						{
							BaseFare = (int)q.PassengerType == 3 ? q.BaseFare : 0,
							IGST = (int)q.PassengerType == 3 ? q.Tax : 0,
							TaxAndFees = (int)q.PassengerType == 3 ? q.YQTax + q.AdditionalTxnFeeOfrd + q.AdditionalTxnFeePub + q.PGCharge + q.SupplierReissueCharges : 0,
							TotalFare = (int)q.PassengerType == 3 ? q.Tax + q.BaseFare + q.YQTax + q.AdditionalTxnFeeOfrd + q.AdditionalTxnFeePub + q.PGCharge + q.SupplierReissueCharges : 0,
							NetFare = (int)q.PassengerType == 3 ? q.Tax + q.BaseFare + q.YQTax + q.AdditionalTxnFeeOfrd + q.AdditionalTxnFeePub + q.PGCharge + q.SupplierReissueCharges : 0,
						},
					},
				}).ToList();

				mdls.Segment = lmseg;
				mdls.TotalPriceList = TotalPriceList;

			}




			return mdls;

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
					List<List<mdlSearchResult>> AllResults = new List<List<mdlSearchResult>>();

					List<mdlSearchResult> ResultOB = new List<mdlSearchResult>();
					List<mdlSearchResult> ResultIB = new List<mdlSearchResult>();
					foreach (var dt in Data.tblTboTravelDetailResult)
					{
						if (dt.ResultType == "OB")
						{
							ResultOB.Add(SearchResultMap(System.Text.Json.JsonSerializer.Deserialize<SearchResult>(dt.JsonData), "OB"));
						}
						else if (dt.ResultType == "IB")
						{
							ResultIB.Add(SearchResultMap(System.Text.Json.JsonSerializer.Deserialize<SearchResult>(dt.JsonData), "IB"));
						}
						else
						{
							ResultOB.Add(SearchResultMap(System.Text.Json.JsonSerializer.Deserialize<SearchResult>(dt.JsonData), "OB"));
						}
					}
					AllResults.Add(ResultOB);
					AllResults.Add(ResultIB);
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

		private async Task RemoveFromDb(mdlFareQuotRequest request)
		{
			DateTime currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
			var Existing = _context.tblTboTravelDetail.Where(p => p.TraceId == request.TraceId && p.ExpireDt > currentDate).FirstOrDefault();
			if (Existing != null)
			{
				_context.Database.ExecuteSqlRaw("delete from tblTboTravelDetailResult where TravelDetailId=@p0", parameters: new[] { Existing.TravelDetailId });
				_context.Database.ExecuteSqlRaw("delete from tblTboTravelDetail where TravelDetailId=@p0", parameters: new[] { Existing.TravelDetailId });
				await _context.SaveChangesAsync();
			}
		}
		private async Task<mdlSearchResponse> SearchFromTboAsync(mdlSearchRequest request)
		{

			int MaxLoginAttempt = 1, LoginAttempt = 0;
			int.TryParse(_config["TBO:MaxLoginAttempt"], out MaxLoginAttempt);
			mdlSearchResponse mdlS = null;
			SearchResponse mdl = null;
			SearchResponseWraper mdlTemp = null;
			string tboUrl = _config["TBO:API:Search"];
			request.EndUserIp = "::1";
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
			request.TokenId = TokenDetails.TokenId;
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
					var tempdata = mdl.Results.SelectMany(p => p).ToArray();
					var result = Search_SaveAsync(request, request.TokenId, mdl.TraceId, tempdata);
					List<List<mdlSearchResult>> AllResults = new List<List<mdlSearchResult>>();
					List<mdlSearchResult> ResultOB = new List<mdlSearchResult>();
					List<mdlSearchResult> ResultIB = new List<mdlSearchResult>();
					foreach (var dt in tempdata)
					{
						if (dt.ResultIndex.Contains("OB"))
						{
							ResultOB.Add(SearchResultMap(dt, "OB"));
						}
						else if (dt.ResultIndex.Contains("IB"))
						{
							ResultIB.Add(SearchResultMap(dt, "IB"));
						}
						else
						{
							ResultOB.Add(SearchResultMap(dt, "OB"));
						}
					}
					if (ResultOB.Count() > 0)
					{
						AllResults.Add(ResultOB);
					}
					if (ResultIB.Count() > 0)
					{
						AllResults.Add(ResultIB);
					}



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

		//public class Error
		//{
		//    public string errCode { get; set; }
		//    public string message { get; set; }
		//    public string details { get; set; }
		//}

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
			public bool IsHoldAllowed { get; set; }
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
			public string LastTicketDate { get; set; }
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

		//#endif






	}
}
