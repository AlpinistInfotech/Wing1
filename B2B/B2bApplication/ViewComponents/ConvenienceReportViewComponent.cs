using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B2bApplication.Models;
using B2BClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;


namespace B2bApplication.ViewComponents
{
    public class ConvenienceReportViewComponent : ViewComponent
    {   private readonly IMarkup _markup;        
        public ConvenienceReportViewComponent(IMarkup markup)
        {
            _markup = markup;         
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var returndata= _markup.LoadConvenience();
            _markup.LoadMarkupAirlineCode(returndata);
            _markup.LoadMarkupCustomerCode(returndata);            
            List<mdlConvenienceReport> result = returndata.Select(p=>new mdlConvenienceReport
            { 
                Id=p.Id,
                Applicability=p.Applicability.ToString(),
                FlightType=p.DirectFlight.ToString(),
                ServiceProvider=p.IsAllProvider?"ALL":string.Join(',', p.MarkupServiceProvider),
                CustomerType=p.IsAllCustomerType?"ALL": string.Join(',',p.MarkupCustomerType),
                Customer= p.IsAllCustomer?"ALL" : string.Join(',',p.MarkupCustomerCode),
                PassengerType=p.IsAllPessengerType?"ALL": string.Join(',',p.MarkupPassengerType),
                FlightClass= p.IsAllFlightClass? "ALL" : string.Join(',', p.MarkupCabinClass),
                Airline=p.IsAllAirline? "ALL" : string.Join(',',p.MarkupAirlineCode ),
                Amount=p.Amount,
                EffectiveFromDt=p.EffectiveFromDt,
                EffectiveToDt = p.EffectiveToDt,
                remarks =p.remarks,
                gender=p.Gender,
            }).ToList();
            return  View(result);
        }

    }
}
