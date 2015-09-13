using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherNsuUniversal.Common.Models;

namespace WeatherNsuUniversal.Common
{
    public interface IDataLoader
    {
        Task<Weather> LoadTemperature();

        Task<Weather> LoadTemperatureWithGraph();

        Task<Forecast> LoadForecast();
    }
}
