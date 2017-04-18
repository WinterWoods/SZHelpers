using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Helpers
{
    public class ConfigHelper
    {
        private string configpath;

        public string Configpath
        {
            get { return configpath; }
        }
        private XDocument xDoc;

        public XDocument XDoc
        {
            get { return xDoc; }
        }

        public ConfigHelper(string _configPath)
        {
            configpath = _configPath;
            var file = new FileInfo(_configPath);
            Directory.CreateDirectory(file.DirectoryName);
            if (!File.Exists(configpath))
            {
                XDocument x = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),//添加XML文件声明
                new XElement("Config")
                );
                x.Save(configpath);
            }
            xDoc = XDocument.Load(configpath);
        }
        public string Get(string type, string key)
        {
            if (string.IsNullOrEmpty(configpath)) throw new Exception("没有初始化配置文件路径.");
            xDoc = XDocument.Load(configpath);
            var _type = xDoc.Root.Element(type);
            if (_type == null)
            {
                xDoc.Root.Add(new XElement(type));
                _type = xDoc.Root.Element(type);
            }
            var _key = _type.Element(key);
            if (_key == null)
            {
                _type.Add(new XElement(key, ""));
                _key = _type.Element(key);
            }
            xDoc.Save(configpath);
            return _key.Value;
        }
        public void Set(string type, string key, string value)
        {
            if (string.IsNullOrEmpty(configpath)) throw new Exception("没有初始化配置文件路径.");
            xDoc = XDocument.Load(configpath);
            var _type = xDoc.Root.Element(type);
            if (_type == null)
            {
                xDoc.Root.Add(new XElement(type));
                _type = xDoc.Root.Element(type);
            }
            var _key = _type.Element(key);
            if (_key == null)
            {
                _type.Add(new XElement(key, value));
                _key = _type.Element(key);
            }
            if(value==null)
            {
                value = "";
            }
            _type.Element(key).Value = value;
            xDoc.Save(configpath);
        }
    }
}
