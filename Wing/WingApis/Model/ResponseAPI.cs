using Database;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WingApis
{
    public class ResponseAPI
    {

        private readonly DBContext _context;
        private readonly IConfiguration _config;
        public ResponseAPI(DBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public ResponseAPI()
        {
        }

        public class mdlResponsedata
        {
            public int Code { get; set; }
            public string Message { get; set; }
        }

        public Task<mdlResponsedata> GetResponse(string url, string requestData,  string tokendata)
        {
            
            mdlResponsedata mdl = new mdlResponsedata();
            mdl.Code = 1;
            mdl.Message = string.Empty;
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", tokendata);
                Stream dataStream =  request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
                WebResponse webResponse = request.GetResponse();
                var rsp = webResponse.GetResponseStream();
                if (rsp == null)
                {
                    mdl.Message = "No Response Found";
                }
                using (StreamReader readStream = new StreamReader(rsp))
                {
                    mdl.Code = 0;
                    mdl.Message = readStream.ReadToEnd();//JsonConvert.DeserializeXmlNode(readStream.ReadToEnd(), "root").InnerXml;
                }
                return Task.FromResult(mdl);
            }
            catch (WebException webEx)
            {
                mdl.Code = 1;
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
                mdl.Code = 1;
                mdl.Message = ex.Message;
            }
            return Task.FromResult(mdl);
        }

    }
}
