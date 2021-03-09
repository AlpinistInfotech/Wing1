using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Database.Classes.APIs
{
    class TBO_tokengeneration
    {
        public static string TBO_TokenGeneration()
        {

            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            string myIP = "";
            string api_name = "http://api.tektravels.com/SharedServices/SharedData.svc/rest/Authenticate";
            string json_request = "{\"ClientId\": \"ApiIntegrationNew\",\"UserName\": \"Wing\",\"Password\": \"Wing@1234\",\"EndUserIp\": \"" + myIP + "\"}";

            string response = APIcalling.GetResponse(json_request, api_name);
            string token_value = "";

            return token_value;
        }

    }


}
