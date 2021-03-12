using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WingGateway.Models;

namespace WingGateway.Controllers
{
    [Authorize]
    public class WingController : Controller
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        public WingController(DBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [Authorize(policy:nameof( enmDocumentMaster.Emp_Dashboard))]
        public IActionResult Index()
        {
            return View();
        }


        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_BankDetails))]
        public IActionResult BankDetails()
        {
            mdlTcBankReportWraper returnDataMdl = new mdlTcBankReportWraper();
            returnDataMdl.FilterModel = new mdlFilterModel() {dateFilter= new mdlDateFilter() , idFilter=new mdlIdFilter(),IsReport=true };
            returnDataMdl.TcBankWrapers = new List<mdlTcBankWraper>();
            return View(returnDataMdl);
        }
        [HttpPost]
        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_BankDetails))]
        public IActionResult BankDetails(mdlFilterModel mdl, enmLoadData submitdata)
        {
            mdlTcBankReportWraper returnData = new mdlTcBankReportWraper();
            if (mdl.dateFilter == null)
            {
                mdl.dateFilter = new mdlDateFilter();
            }
            if (mdl.idFilter == null)
            {
                mdl.idFilter = new mdlIdFilter();
            }
            mdl.dateFilter.FromDt =Convert.ToDateTime( mdl.dateFilter.FromDt.ToString("dd-MMM-yyyy"));
            mdl.dateFilter.ToDt = Convert.ToDateTime(mdl.dateFilter.ToDt.AddDays(1).ToString("dd-MMM-yyyy"));
            WingGateway.Classes.ConsProfile consProfile = new Classes.ConsProfile(_context, _config);
            returnData.TcBankWrapers = consProfile.GetBankDetails(submitdata, mdl, 0, false);
            returnData.FilterModel = mdl;
            return View(returnData);
        }


    }
}
