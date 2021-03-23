﻿using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WingGateway.Models;
using Microsoft.Extensions.Configuration;
using WingGateway.Classes;

namespace WingGateway.ViewComponents
{
    public class MarkUpReportViewComponent : ViewComponent
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        private readonly ICurrentUsers _currentUsers;
        public MarkUpReportViewComponent(DBContext context, IConfiguration config, ICurrentUsers currentUsers)
        {
            _context = context;
            _config = config;
            _currentUsers = currentUsers;
        }
        public async Task<IViewComponentResult> InvokeAsync([FromServices] IConsProfile consProfile)
        {
            var result = await consProfile.GetMarkUpDetails(_currentUsers.TcNid);
            return  View(result);
        }

    }
}
