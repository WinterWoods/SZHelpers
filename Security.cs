using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public static class Security
    {
        private static string _QueryStringKey = "sydzl521"; //URL传输参数加密Key
        private static string _LongQueryStringKey = ",sydzl521,./!@#$%^&*())(";//24位默认密钥

        public static string QueryStringKey
        {
            set { _QueryStringKey = value; }
            get { return _QueryStringKey; }
        }
        public static string LongQueryStringKey
        {
            set { _LongQueryStringKey = value; }
            get { return _LongQueryStringKey; }
        }

        /// <summary>
        /// 标准 8位DEC 加密过程 默认密钥
        /// </summary>
        /// <param name="QueryString"></param>
        /// <returns></returns>
        public static string EncryptQueryString(this string QueryString)
        {
            return Encrypt(QueryString, _QueryStringKey);
        }

        /// <summary>
        /// 标准 8位DEC 解密过程 默认密钥
        /// </summary>
        /// <param name="QueryString"></param>
        /// <returns></returns>
        public static string DecryptQueryString(this string QueryString)
        {
            return Decrypt(QueryString, _QueryStringKey);
        }

        /// <summary>
        ///  标准 8位DEC 加密过程
        /// </summary>
        /// <param name="pToEncrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Encrypt(this string pToEncrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider(); //把字符串放到byte数组中 

            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            //byte[] inputByteArray=Encoding.Unicode.GetBytes(pToEncrypt); 

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey); //建立加密对象的密钥和偏移量
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);   //原文使用ASCIIEncoding.ASCII方法的GetBytes方法 
            MemoryStream ms = new MemoryStream();     //使得输入密码必须输入英文文本
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        /// <summary>
        ///  标准 8位DEC 解密过程
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Decrypt(this string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey); //建立加密对象的密钥和偏移量，此值重要，不能修改 
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder(); //建立StringBuild对象，CreateDecrypt使用的是流对象，必须把解密后的文本变成流对象 

            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }
        /// <summary>
        /// 24位默认密钥加密
        /// </summary>
        /// <param name="QueryString"></param>
        /// <returns></returns>
        public static string LongEncryptQueryString(this string QueryString)
        {
            return LongEncrypt(QueryString, _LongQueryStringKey);
        }
        /// <summary>
        /// 24位默认密钥解密
        /// </summary>
        /// <param name="QueryString"></param>
        /// <returns></returns>
        public static string LongDecryptQueryString(this string QueryString)
        {
            return LongDecrypt(QueryString, _LongQueryStringKey);
        }
        /// <summary>
        /// 24位加密过程
        /// </summary>
        /// <param name="a_strString"></param>
        /// <param name="a_strKey"></param>
        /// <returns></returns>
        public static string LongEncrypt(this string a_strString, string a_strKey)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(a_strKey);
            DES.Mode = CipherMode.ECB;
            ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(a_strString);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }
        /// <summary>
        /// 24位解密过程
        /// </summary>
        /// <param name="a_strString"></param>
        /// <param name="a_strKey"></param>
        /// <returns></returns>
        public static string LongDecrypt(this string a_strString, string a_strKey)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(a_strKey);
            DES.Mode = CipherMode.ECB;
            DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            ICryptoTransform DESDecrypt = DES.CreateDecryptor();
            string result = "";
            try
            {
                byte[] Buffer = Convert.FromBase64String(a_strString);
                result = ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch (Exception)
            {
            }
            return result;
        }
        /// <summary>
        /// 检查己加密的字符串是否与原文相同
        /// </summary>
        /// <param name="EnString"></param>
        /// <param name="FoString"></param>
        /// <returns></returns>
        public static bool ValidateString(this string EnString, string FoString)
        {
            if (Decrypt(EnString, _QueryStringKey) == FoString.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string Base64Code(this string Message)
        {
            byte[] bytes = Encoding.Default.GetBytes(Message);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string Base64Decode(string Message)
        {
            byte[] bytes = Convert.FromBase64String(Message);
            return Encoding.Default.GetString(bytes);
        }
    }
}
