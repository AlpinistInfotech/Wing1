using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Database.Classes.APIs
{
    class TBO_tokengeneration
    {

        
        string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
        static string myIP = "";
        static string  api_name = "http://api.tektravels.com/SharedServices/SharedData.svc/rest/Authenticate";
        static string json_request = "{\"ClientId\": \"ApiIntegrationNew\",\"UserName\": \"Wing\",\"Password\": \"Wing@1234\",\"EndUserIp\": \""+ myIP + "\"}";

        string response = APIcalling.GetResponse(json_request, api_name);


    }


}
