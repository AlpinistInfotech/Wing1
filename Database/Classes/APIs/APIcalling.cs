using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Database.Classes.APIs
{
    class APIcalling
    {
        public static string GetResponse(string requestData, string url)
        {
            string responseXML = string.Empty;
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Accept-Encoding", "gzip");
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
                WebResponse webResponse = request.GetResponse();
                var rsp = webResponse.GetResponseStream();
                if (rsp == null)
                {
                    //throw exception
                }
                using (StreamReader readStream = new StreamReader(new GZipStream(rsp, CompressionMode.Decompress)))
                {
                    responseXML = JsonConvert.DeserializeXmlNode(readStream.ReadToEnd()).InnerXml;
                }
                return responseXML;
            }
            catch (WebException webEx)
            {
                //get the response stream
                WebResponse response = webEx.Response;
                Stream stream = response.GetResponseStream();
                String responseMessage = new StreamReader(stream).ReadToEnd();
                return responseMessage;
            }
        }


    }
}
