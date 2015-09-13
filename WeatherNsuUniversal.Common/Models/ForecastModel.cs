using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WeatherNsuUniversal.Common.Models
{
    [XmlRoot("forecast")]
    public class Forecast
    {
        [XmlElement("daily_forecast")]
        public List<DailyForecast> DailyForecasts = new List<DailyForecast>(); 
    }

    [XmlRoot("daily_forecast")]
    public class DailyForecast
    {
        [XmlAttribute("timestamp")]
        public long Timestamp { get; set; }

        [XmlElement("day")]
        public float DayTemperature { get; set; }

        [XmlElement("night")]
        public float NightTemperature { get; set; }
    }
}
