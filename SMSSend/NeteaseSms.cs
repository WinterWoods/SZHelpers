using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Helpers.SMSSend
{
    
    public class NeteaseSmsResult
    {
        public int code { get; set; }

        public string msg { get; set; }

        public string obj { get; set; }
    }
    public class NeteaseSms
    {
        private string appKey = "";
        private string appSecret = "";
        private static ConfigHelper config;
        private static NeteaseSms instance = null;
        private static readonly object padlock = new object();
        private static string sendCodeUrl = "https://api.netease.im/sms/sendcode.action";
        public static string sendtemplateUrl = "https://api.netease.im/sms/sendtemplate.action";

        public string getCheckSum(string appSecret, string nonce, string curTime)
        {
            return this.Sha1(appSecret + nonce + curTime);
        }

        private string getStr(bool b, int n)
        {
            string str = "0123456789abcdefghijklmnopqrstuvwxyz";
            if (b)
            {
                str = str + "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            }
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < n; i++)
            {
                builder.Append(str.Substring(random.Next(0, str.Length), 1));
            }
            return builder.ToString();
        }

        public NeteaseSmsResult SendForgetPasswordMessageCode(string mobile, string authCode)
        {
            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("templateid", config.Get("Sms", "MessageCodeForgetPasswordTemplateId", "")),
                new KeyValuePair<string, string>("mobile", mobile),
                new KeyValuePair<string, string>("authCode", authCode)
            };
            string sendCodeUrl = NeteaseSms.sendCodeUrl;
            return this.SendMessage(sendCodeUrl, data);
        }

        public NeteaseSmsResult SendMessage(string url, List<KeyValuePair<string, string>> data)
        {
            NeteaseSmsResult result = new NeteaseSmsResult();
            HttpClient client = new HttpClient();
            HttpContent content = new FormUrlEncodedContent(data);
            string nonce = this.getStr(false, 0x10);
            string curTime = DateTime.Now.ToFileTimeUtc().ToString();
            string str3 = this.getCheckSum(this.appSecret, nonce, curTime).ToLower();
            content.Headers.Remove("Content-Type");
            content.Headers.Remove("charset");
            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            content.Headers.Add("charset", "utf-8");
            content.Headers.Add("CurTime", curTime);
            content.Headers.Add("Nonce", nonce);
            content.Headers.Add("CheckSum", str3);
            content.Headers.Add("AppKey", this.appKey);
            HttpResponseMessage message = client.PostAsync(url, content).Result;
            if (message.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<NeteaseSmsResult>(message.Content.ReadAsStringAsync().Result);
            }
            string str5 = message.Content.ReadAsStringAsync().Result;
            return result;
        }

        public NeteaseSmsResult SendRegisterMessageCode(string mobile, string authCode)
        {
            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("templateid", config.Get("Sms", "MessageCodeRegisterTemplateId", "")),
                new KeyValuePair<string, string>("mobile", mobile),
                new KeyValuePair<string, string>("authCode", authCode)
            };
            string sendCodeUrl = NeteaseSms.sendCodeUrl;
            return this.SendMessage(sendCodeUrl, data);
        }

        public string Sha1(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] buffer2 = SHA1.Create().ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            foreach (byte num2 in buffer2)
            {
                builder.Append(num2.ToString("X2"));
            }
            return builder.ToString();
        }

        public static NeteaseSms Instance
        {
            get
            {
                object padlock = NeteaseSms.padlock;
                lock (padlock)
                {
                    if (instance == null)
                    {
                        Directory.SetCurrentDirectory(@"C:\");
                        FileInfo info = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);
                        Directory.SetCurrentDirectory(info.DirectoryName);
                        config = new ConfigHelper("config_NeteaseSms.txt");
                        instance = new NeteaseSms();
                        instance.appSecret = config.Get("Sms", "appSecret", "");
                        instance.appKey = config.Get("Sms", "appKey", "");
                    }
                    return instance;
                }
            }
        }
    }
}

