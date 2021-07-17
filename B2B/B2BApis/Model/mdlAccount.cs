using B2BClasses.Database;
using B2BClasses.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BApis.Model
{
    public  class mdlUserMasterApi
    {
        public string TokenData { get; set; }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }
    public class mdlTookenRequest
    {
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public enmCustomerType customerType { get; set; }
        public string Name { get; set; }
    }
}
