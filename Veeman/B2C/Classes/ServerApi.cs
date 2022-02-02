using B2C.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace B2C.Classes
{
    public interface IServerApi
    {
        mdlResponse GetResponsePostMethod(string requestData, string url, string Token, string Method = "POST");
    }

    public class ServerApi : IServerApi
    {
        private readonly IConfiguration _config;
        private readonly string _baseUrl;
        public ServerApi(IConfiguration config)
        {
            _config = config;
            _baseUrl = _config["apiUrl:BaseUrl"];
        }

        public mdlResponse GetResponsePostMethod(string requestData, string url, string Token, string Method = "POST")
        {
            mdlResponse mdl = new mdlResponse();
            mdl.MessageType = enmMessageType.Error;
            mdl.Message = string.Empty;
            try
            {
                byte[] data = requestData.Length>0? Encoding.UTF8.GetBytes(requestData):null;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_baseUrl+url);
                request.Method = Method;
                request.ContentType = "application/json";
                request.Headers.Add("apikey", "Bearer " + Token);
                if (data != null)
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(data, 0, data.Length);
                    dataStream.Close();
                }
                WebResponse webResponse = request.GetResponse();
                var rsp = webResponse.GetResponseStream();
                if (rsp == null)
                {
                    mdl.Message = "No Response Found";
                }
                using (StreamReader readStream = new StreamReader(rsp))
                {
                    mdl.MessageType = enmMessageType.Success;
                    mdl.Message = readStream.ReadToEnd();//JsonConvert.DeserializeXmlNode(readStream.ReadToEnd(), "root").InnerXml;
                }
                return mdl;
            }
            catch (WebException webEx)
            {
                mdl.MessageType = enmMessageType.Error;
                //get the response stream
                WebResponse response = webEx.Response;
                if (response == null)
                {
                    mdl.Message = "No Response from server";
                }
                else
                {
                    Stream stream = response.GetResponseStream();
                    String responseMessage = new StreamReader(stream).ReadToEnd();
                    mdl.Message = responseMessage;
                }
            }
            catch (Exception ex)
            {
                mdl.MessageType = enmMessageType.Error;
                mdl.Message = ex.Message;
            }
            return mdl;
        }
    }
}
