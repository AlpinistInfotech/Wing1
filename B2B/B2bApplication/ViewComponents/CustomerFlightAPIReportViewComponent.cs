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
    public class CustomerFlightAPIReportViewComponent : ViewComponent
    {   private readonly IMarkup _markup;        
        public CustomerFlightAPIReportViewComponent(IMarkup markup)
        {
            _markup = markup;         
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var returndata= _markup.LoadCustomerFlightAPI();
            _markup.LoadMarkupAirlineCode(returndata);
            _markup.LoadMarkupCustomerCode(returndata);            
            List<mdlCustomerFlightAPIReport> result = returndata.Select(p=>new mdlCustomerFlightAPIReport
            { 
                Id=p.Id,
                 
                FlightType=p.DirectFlight.ToString(),
                ServiceProvider=p.IsAllProvider?"ALL":string.Join(',', p.MarkupServiceProvider),
                CustomerType=p.IsAllCustomerType?"ALL": string.Join(',',p.MarkupCustomerType),
                Customer= p.IsAllCustomer?"ALL" : string.Join(',',p.MarkupCustomerCode),
                
                FlightClass= p.IsAllFlightClass? "ALL" : string.Join(',', p.MarkupCabinClass),
                Airline=p.IsAllAirline? "ALL" : string.Join(',',p.MarkupAirlineCode ),
                
                EffectiveFromDt=p.EffectiveFromDt,
                EffectiveToDt = p.EffectiveToDt,
                remarks =p.remarks,
                
            }).ToList();
            return  View(result);
        }

    }
}
