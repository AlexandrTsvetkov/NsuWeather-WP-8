using System.Collections.Generic;
using System.Xml.Serialization;

namespace NsuWeatherMobile.Common
{
    [XmlRoot("weather")]
    public class Weather
    {
        [XmlElement("average")]
        public float Average;

        [XmlElement("current")]
        public float Current;

        [XmlElement("graph")]
        public Graph Graphic;
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
        public long Timestamp;

        [XmlText]
        public float Value;
    }
}
