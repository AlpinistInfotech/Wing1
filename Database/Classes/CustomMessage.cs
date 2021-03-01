using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Classes
{
    public class CustomMessage
    {
        public enmMessageType MessageType { get; set; }
        public string Message{ get; set; }
        public string ReturnNumber { get; set; }
    }
}
