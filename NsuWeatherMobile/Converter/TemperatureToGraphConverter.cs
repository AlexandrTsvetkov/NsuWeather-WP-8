using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using NsuWeatherMobile.Common;
using NsuWeatherMobile.Common.Extensions;
using NsuWeatherMobile.ViewModel;

namespace NsuWeatherMobile
{
    public class TemperatureToGraphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<GraphicModel> collection = new ObservableCollection<GraphicModel>();

            var temperatureList = value as List<Temperature>;
            if (temperatureList == null)
                return collection;
        
            for (int i = temperatureList.Count - 1; i >= 0; i -= 10)
            {
                collection.Add(new GraphicModel
                {
                    Time = temperatureList[i].Timestamp.FromUnixEpochToDateTime(),
                    Value = (int) Math.Round(temperatureList[i].Value, 0)
                });
            }

            collection = new ObservableCollection<GraphicModel>(collection.Reverse());
            return collection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Not supported, dont use this!
            return new List<Temperature>();
        }
    }
}
