using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public List<GraphNode> GraphList = new List<GraphNode>();
    }

    [XmlRoot("temp")]
    public class GraphNode
    {
        [XmlAttribute("timestamp")]
        public int Timestamp;

        [XmlText]
        public float Temperature;
    }
}
