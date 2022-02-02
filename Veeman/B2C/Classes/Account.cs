using B2C.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2C.Classes
{
    public interface IAccount
    {
        mdlLoginResponseWraper LoginAsync(mdlLogin mdl);
        mdlCaptchaWraper LoadCaptcha();
    }

    public class Account : IAccount
    {
        private readonly IServerApi _IServerApi;
        public Account(IServerApi IServerApi)
        {
            _IServerApi = IServerApi;
        }

        public mdlLoginResponseWraper LoginAsync(mdlLogin mdl)
        {
            mdlLoginResponseWraper returnData = new mdlLoginResponseWraper() { messageType = enmMessageType.None };

            string jsonString = JsonConvert.SerializeObject(mdl);
            var responseData = _IServerApi.GetResponsePostMethod(jsonString, "User/Login", "");
            if (responseData.MessageType == enmMessageType.Success)
            {
                returnData = JsonConvert.DeserializeObject<mdlLoginResponseWraper>(responseData.Message);
            }
            else
            {
                throw new Exception(responseData.Message);
            }
            return returnData;

        }

        public mdlCaptchaWraper LoadCaptcha()
        {
            mdlCaptchaWraper returnData = new mdlCaptchaWraper();
            
            var responseData = _IServerApi.GetResponsePostMethod("", "user/GenrateLoginCaptcha/0", "","GET");
            if (responseData.MessageType == enmMessageType.Success)
            {
                returnData = JsonConvert.DeserializeObject<mdlCaptchaWraper>(responseData.Message);
            }
            else
            {
                throw new Exception(responseData.Message);
            }
            return returnData;
        }
    }
}
