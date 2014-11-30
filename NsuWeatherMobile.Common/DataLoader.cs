using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace NsuWeatherMobile.Common
{
    public class DataLoader
    {
        private const string NsuWeatherUrl = "http://weather.nsu.ru/weather_brief.xml";

        private const string XmlNodeName = "current";

        private static Random randomKey = new Random();

        public async static Task<float> LoadTemperature()
        {
            HttpWebRequest webRequest =
                (HttpWebRequest)WebRequest.Create(string.Format("{0}?{1}", NsuWeatherUrl, randomKey.Next()));
            HttpWebResponse webResponse = (HttpWebResponse)
                                          await
                                          Task<WebResponse>.Factory.FromAsync(webRequest.BeginGetResponse,
                                                                              webRequest.EndGetResponse, null);

            XDocument xmlDoc = XDocument.Load(webResponse.GetResponseStream());
            string stringTemperature = (from n in xmlDoc.Root.Elements() where n.Name == XmlNodeName select n.Value).FirstOrDefault();
            float temp = float.Parse(stringTemperature, NumberStyles.Any, NumberFormatInfo.InvariantInfo);

            return temp;
        }
    }
}
