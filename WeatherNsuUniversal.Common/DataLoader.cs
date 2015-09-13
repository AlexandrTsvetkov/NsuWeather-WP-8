using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using WeatherNsuUniversal.Common.Models;

namespace WeatherNsuUniversal.Common
{
    public class DataLoader : IDataLoader
    {
        private const string NsuWeatherUrl = "http://weather.nsu.ru/weather_brief.xml";
        private const string NsuWeatherUrlWithGraph = "http://weather.nsu.ru/weather.xml";

        private readonly Random RandomKey = new Random();

        private readonly XmlReaderSettings Setting = new XmlReaderSettings {DtdProcessing = DtdProcessing.Ignore};

        private async Task<Weather> LoadData(string url)
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

        public async Task<Weather> LoadTemperature()
        {
            return await LoadData(NsuWeatherUrl);
        }

        public async Task<Weather> LoadTemperatureWithGraph()
        {
            return await LoadData(NsuWeatherUrlWithGraph);
        }

        public Task<Forecast> LoadForecast()
        {
            //TODO: Impelement loading forecast data
            return null;
        }
    }
}
