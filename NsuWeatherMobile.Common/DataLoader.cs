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
        private const string NsuWeatherUrlWithGraph = "http://weather.nsu.ru/weather.xml";

        private static readonly Random RandomKey = new Random();

        private static readonly XmlReaderSettings Setting = new XmlReaderSettings {DtdProcessing = DtdProcessing.Ignore};

        private static async Task<Weather> LoadData(string url)
        {
            HttpWebRequest webRequest =
                (HttpWebRequest) WebRequest.Create(string.Format("{0}?{1}", url, RandomKey.Next()));
            HttpWebResponse webResponse = (HttpWebResponse)
                await
                    Task<WebResponse>.Factory.FromAsync(webRequest.BeginGetResponse,
                        webRequest.EndGetResponse, null);

            XmlReader xmlReader = XmlReader.Create(webResponse.GetResponseStream(), Setting);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof (Weather));
            Weather weather = (Weather) xmlSerializer.Deserialize(xmlReader);

            return weather;
        }

        public static async Task<Weather> LoadTemperature()
        {
            return await LoadData(NsuWeatherUrl);
        }

        public static async Task<Weather> LoadTemperatureWithGraph()
        {
            return await LoadData(NsuWeatherUrlWithGraph);
        }   
    }
}
