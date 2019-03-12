using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nex.Core
{
    public static class NConverter
    {
        public static byte[] HexStringToBytes(string value)
        {
            value = value.Replace(" ", "").Replace("-", "").Replace(":", "");
            return Enumerable.Range(0, value.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => System.Convert.ToByte(value.Substring(x, 2), 16))
                     .ToArray();
        }

        public static JObject GetJSON(byte[] data)
        {
            var str = Encoding.UTF8.GetString(data, 0, data.Length);
            return JObject.Parse(str);
        }

        public static byte[] GetBytes(JObject node)
        {
            var str = node.ToString();
            return Encoding.UTF8.GetBytes(str);
        }

        public static XElement GetXml(byte[] data)
        {
            var str = Encoding.UTF8.GetString(data, 0, data.Length);
            XDocument doc = XDocument.Parse(str);
            return doc.LastNode as XElement;
        }
        public static byte[] GetBytes(XElement node)
        {
            var str = node.ToString();
            return Encoding.UTF8.GetBytes(str);
        }
        public static XElement ToXml(Exception exception)
        {
            // Validate arguments
            if (exception == null)
                throw new ArgumentNullException("exception");

            // The root element is the Exception's type
            XElement root = new XElement("Exception", new XAttribute("Type", exception.GetType().ToString()));
            if (exception.Message != null)
                root.Add(new XElement("Message", exception.Message));

            // StackTrace can be null, e.g.:
            // new ExceptionAsXml(new Exception())
            if (exception.StackTrace != null)
                root.Add(
                    new XElement("StackTrace",
                    from frame in exception.StackTrace.Split('\n')
                    let prettierFrame = frame.Substring(6).Trim()
                    select new XElement("Frame", prettierFrame))
                );

            if (exception.Data.Count > 0)
            {
                root.Add(
                    new XElement("Data",
                        from entry in exception.Data.Cast<DictionaryEntry>()
                        let key = entry.Key.ToString()
                        let value = (entry.Value == null) ? "null" : entry.Value.ToString()
                        select new XElement(key, value))
                );
            }

            if (exception.InnerException != null)
                root.Add(ToXml(exception.InnerException));

            return root;
        }
    }
}
