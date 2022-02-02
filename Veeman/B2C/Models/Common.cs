using B2C.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2C.Models
{
    public class mdlResponse
    {
        public enmMessageType MessageType { get; set; }
        public string Message { get; set; }
    }
    public class mdlReturnData
    {
        public enmMessageType MessageType { get; set; } = enmMessageType.None;
        public string Message { get; set; }
        public dynamic ReturnId { get; set; }
    }
    public class mdlCommonReturn
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
    public class mdlCommonReturnWithParentID : mdlCommonReturn
    {
        public int ParentId { get; set; }
    }
    public class mdlCommonReturnUlong
    {
        public ulong Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
    public class mdlDeleteData
    {
        public int Id { get; set; }
        public string Remarks { get; set; }
    }
}
