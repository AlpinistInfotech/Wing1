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
    public class WingMarkUpReportViewComponent : ViewComponent
    {   private readonly IMarkup _markup;        
        public WingMarkUpReportViewComponent(IMarkup markup)
        {
            _markup = markup;         
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var returndata= _markup.LoadMarkup();
            _markup.LoadMarkupAirlineCode(returndata);
            _markup.LoadMarkupCustomerCode(returndata);            
            List<mdlWingMarkupReport> result =  _markup.LoadMarkup().Select(p=>new mdlWingMarkupReport { 
                Id=p.Id,
                Applicability=p.Applicability.ToString(),
                FlightType=p.DirectFlight.ToString(),
                ServiceProvider=p.IsAllProvider?"ALL":string.Join( p.MarkupServiceProvider?.ToString(),","),
                CustomerType=p.IsAllCustomerType?"ALL": string.Join(p.MarkupCustomerType?.ToString(), ","),
                Customer= p.IsAllCustomer?"ALL" : string.Join(p.MarkupCustomerCode?.ToString(), ","),
                PassengerType=p.IsAllPessengerType?"ALL": string.Join(p.MarkupPassengerType?.ToString(), ","),
                FlightClass= p.IsAllFlightClass? "ALL" : string.Join(p.MarkupCabinClass?.ToString(), ","),
                Airline=p.IsAllAirline? "ALL" : string.Join(p.MarkupAirlineCode?.ToString(), ","),
                Amount=p.Amount,
                EffectiveFromDt=p.EffectiveFromDt,
                remarks=p.remarks
            }).ToList();
            return  View(result);
        }

    }
}
