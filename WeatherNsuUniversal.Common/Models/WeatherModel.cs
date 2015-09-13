using System.Collections.Generic;
using System.Xml.Serialization;

namespace WeatherNsuUniversal.Common.Models
{
    [XmlRoot("weather")]
    public class Weather
    {
        [XmlElement("average")]
        public float Average { get; set; }

        [XmlElement("current")]
        public float Current { get; set; }

        [XmlElement("graph")]
        public Graph Graphic { get; set; }
    }

    [XmlRoot("graph")]
    public class Graph
    {
        [XmlElement("temp")]
        public List<Temperature> TemperatureList = new List<Temperature>();
    }

    [XmlRoot("temp")]
    public class Temperature
    {
        [XmlAttribute("timestamp")]
        public long Timestamp { get; set; }

        [XmlText]
        public float Value { get; set; }

    }
}
