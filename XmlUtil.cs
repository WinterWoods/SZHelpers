using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Helpers
{
    public static class XmlUtil
    {

        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string xmlPath)
        {
            try
            {
                using(FileStream fs =new  FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    XmlSerializer xmldes = new XmlSerializer(typeof(T));
                    return (T)xmldes.Deserialize(fs);
                }
                
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region 序列化XML文件
        /// <summary>
        /// 序列化XML文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="xmlPath"></param>
        public static void Serializer<T>(this T obj,string xmlPath)
        {

            //创建序列化对象  
            XmlSerializer xml = new XmlSerializer(typeof(T));
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(xmlPath,false))
            {
                //序列化对象  
                xml.Serialize(file, obj);
                file.Flush();
            }
        }
        #endregion

    }
}
