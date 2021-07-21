using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B2bApplication.Models;
using B2BClasses;
using B2BClasses.Database;
using Microsoft.AspNetCore.Mvc;
using B2bApplication.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;


namespace B2bApplication.ViewComponents 
{
    public class PaymentApprovalViewComponent : ViewComponent
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        public PaymentApprovalViewComponent(DBContext context,IConfiguration config)
        {
            _context= context;
            _config = config;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        { 
            string filePath = "wwwroot/" + _config["FileUpload:PaymentRequestFilePath"];

            var result = _context.tblPaymentRequest.Where(p => p.Status == 0).Select(p => new mdlPaymentRequestWraper { Id = p.Id, CreatedDt = p.CreatedDt, CustomerId = p.CustomerId, RequestedAmt = p.RequestedAmt, CreatedRemarks = p.CreatedRemarks, CustomerName = p.tblCustomerMaster.CustomerName, Code = p.tblCustomerMaster.Code, RequestType = p.RequestType, UploadImages = filePath + "/" + p.UploadImages }).ToList();

            return View(result);
        }

    }
}
