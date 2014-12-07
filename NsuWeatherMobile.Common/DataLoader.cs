using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NsuWeatherMobile.Common
{
    public class DataLoader
    {
        private const string NsuWeatherUrl = "http://weather.nsu.ru/weather_brief.xml";

        private static Random randomKey = new Random();

        private static XmlReaderSettings setting = new XmlReaderSettings {DtdProcessing = DtdProcessing.Ignore};

        public static async Task<float> LoadTemperature()
        {
            HttpWebRequest webRequest =
                (HttpWebRequest) WebRequest.Create(string.Format("{0}?{1}", NsuWeatherUrl, randomKey.Next()));
            HttpWebResponse webResponse = (HttpWebResponse)
                await
                    Task<WebResponse>.Factory.FromAsync(webRequest.BeginGetResponse,
                        webRequest.EndGetResponse, null);

            XmlReader xmlReader = XmlReader.Create(webResponse.GetResponseStream(), setting);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Weather));
            Weather weather = (Weather) xmlSerializer.Deserialize(xmlReader);

            return weather.Current;
        }
    }
}
