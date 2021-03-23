using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WingGateway.Models;
using Microsoft.Extensions.Configuration;

namespace WingGateway.ViewComponents
{
    public class MarkUpReportViewComponent : ViewComponent
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        public MarkUpReportViewComponent(DBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<IViewComponentResult> InvokeAsync(int TcNid)
        {
            mdlTcMarkUpReportWraper returnData = new mdlTcMarkUpReportWraper();
            WingGateway.Classes.ConsProfile consProfile = new Classes.ConsProfile(_context, _config);
            returnData.TcMarkUpWrapers = consProfile.GetMarkUpDetails(TcNid);
            return View(returnData);
        }

    }
}
