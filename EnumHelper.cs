using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Helpers
{

    public class EnumHelper
    {
        public static string GetStringValue(Enum value)
        {
            // Get the type 
            Type type = value.GetType();
            // Get fieldinfo for this type 
            FieldInfo fieldInfo = type.GetField(value.ToString());
            // Get the stringvalue attributes 
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
            // Return the first if there was a match. 
            return (attribs.Length > 0 ? attribs[0].StringValue : "");
        }
        public static T GetEnum<T>(T enumType, string name)
        {
            T returnValue = (T)Enum.Parse(typeof(T), name);
            return returnValue;
        }
        public static List<Dictionary<string, string>> GetEnumList(Type enumType) 
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
             foreach (var value in enumType.GetEnumValues())
             {
                 Dictionary<string, string> tmp = new Dictionary<string, string>();
                 tmp.Add("Key", ((int)value).ToString());
                 if (Enum.IsDefined(enumType, (int)value))
                 {
                     tmp.Add("Value", GetStringValue((Enum)value));
                 }
                 
                 result.Add(tmp);
             }
            return result;
        }
        public static List<Dictionary<T, string>> GetEnumList1<T>()
        {
            List<Dictionary<T, string>> result = new List<Dictionary<T, string>>();
            foreach (var value in typeof(T) .GetEnumValues())
            {
                Dictionary<T, string> tmp = new Dictionary<T, string>();

                if (Enum.IsDefined(typeof(T), (int)value))
                {
                    tmp.Add((T)value,  GetStringValue((Enum)value));
                }

                result.Add(tmp);
            }
            return result;
        }
        public static Dictionary<T, string> GetEnumList2<T>()
        {
            Dictionary<T, string> tmp = new Dictionary<T, string>();
            foreach (var value in typeof(T).GetEnumValues())
            {
                if (Enum.IsDefined(typeof(T), (int)value))
                {
                    tmp.Add((T)value, GetStringValue((Enum)value));
                }
            }
            return tmp;
        }
    }
    public class StringValueAttribute : Attribute
    {

        private string _StringValue;

        public string StringValue
        {

            get { return _StringValue; }

            protected set { _StringValue = value; }

        }

        public StringValueAttribute(string value)
        {

            this.StringValue = value;

        }

    }
}
