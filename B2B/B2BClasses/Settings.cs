using B2BClasses.Database;
using B2BClasses.Services.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace B2BClasses
{

    public interface ISettings
    {
        string GetErrorMessage(enmMessage message);
    }

    public class Settings : ISettings
    {
        private  const string EncryptKey = "HareRamaKrishana";
        private readonly DBContext _context;
        private IConfiguration _config;
        public Settings(DBContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }

        public string GetErrorMessage(enmMessage message)
        {
            return _config[string.Concat("ErrorMessages:", message.ToString())] ?? message.ToString();
        }

        public static string Encrypt(string input)
        {
            string key = Settings.EncryptKey;
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            byte[] inputArray1 = UTF8Encoding.UTF8.GetBytes(key);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string input )
        {
            string key= Settings.EncryptKey;
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static string TokenGenrate()
        {
            Random random = new Random();
            return  random.Next(1, 9999).ToString("D4");            
        }


    }
}
