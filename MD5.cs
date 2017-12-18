using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Helpers
{
    public static class MD51
    {
        public static string FileMD5(this string path)
        {
            try
            {
                FileStream get_file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                System.Security.Cryptography.MD5CryptoServiceProvider get_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                
                byte[] hash_byte = get_md5.ComputeHash(get_file);
                string resule = System.BitConverter.ToString(hash_byte);
                resule = resule.Replace("-", "");
                get_file.Close();
                return resule;
            }
            catch
            {
                throw;
            }
        }
        public static string StrMD5(string str)
        {
            try
            {
                byte[] result = Encoding.Default.GetBytes(str);
                System.Security.Cryptography.MD5CryptoServiceProvider get_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

                byte[] hash_byte = get_md5.ComputeHash(result);
                string resule = System.BitConverter.ToString(hash_byte);
                resule = resule.Replace("-", "");
                return resule;
            }
            catch
            {
                throw;
            }
        }
        public static string GetEncMD5(string txtPwd)
        {
            string strRandom;

            //获取3位随机字母
            Random r = new Random();
            strRandom = ((char)(r.Next(65, 65 + 26))).ToString() + ((char)(r.Next(65, 65 + 26))).ToString() + ((char)(r.Next(65, 65 + 26))).ToString();

            return strRandom + StrMD5(strRandom + txtPwd);//设定加密方式，也可以在此进行多次Md5加强密码安全度
        }
        public static bool PwdIsRight(string sqlPwd, string txtPwd)
        {
            string str = sqlPwd.Substring(0, 3);
            string tmp = str + StrMD5(str + txtPwd);
            return sqlPwd.ToLower() == tmp.ToLower() ? true : false;
        }
    }
}
