using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

using NsuWeatherMobile.Common;
using NsuWeatherMobile.Common.Extensions;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace NsuWeatherMobile.ViewModel
{
    public class TemperatureModel : INotifyPropertyChanged 
    {
        public class GraphModel
        {
            private int? _minValue;
            public int MinValue
            {
                get
                {
                    return _minValue ?? -50;
                }
                set { /*_minValue = value;*/ }
            }

            private int? _maxValue;
            public int MaxValue
            {
                get
                {
                    return _maxValue ?? 50;
                }
                set { /*_maxValue = value;*/ }
            }

            public IList<Temperature> TemperatureCollection { get; set; }
        }

        #region Properties
        private int _temperature;
        public int Temperature
        {
            get { return _temperature; }
            set
            {
                _temperature = value;
                OnPropertyChanged("TemperatureAtNsu");
            }
        }

        private DateTime _updateTime;
        public DateTime UpdateTime
        {
            get { return _updateTime; }
            set
            {
                _updateTime = value;
                OnPropertyChanged("RefreshTimeString");
            }
        }

        public string RefreshTimeString
        {
            get { return string.Format("Обновлено в {0}", _updateTime.ToShortTimeString()); }
        }

        public string TemperatureAtNsu
        {
            get { return string.Format("{0}°", _temperature); }
        }

        private bool _isLoaded;
        private bool IsLoad
        {
            set
            {
                _isLoaded = value;
                OnPropertyChanged("IsVisibleTextBlock");
                OnPropertyChanged("IsVisibleBar");
            }
        }

        private bool _isError;
        public bool IsError
        {
            set
            {
                _isError = value;
                OnPropertyChanged("IsVisibleErrorMessage");
            }
        }

        private GraphModel _graph;
        public GraphModel Graph
        {
            get { return _graph; }
            set
            {
                _graph = value;
                OnPropertyChanged("Graph");
            }
        }

        private PlotModel _plotModel;
        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set
            {
                _plotModel = value;
                OnPropertyChanged("PlotModel");
            }
        }

        public bool IsVisibleTextBlock
        {
            get 
            {
                return _isLoaded && !_isError;
            }
        }

        public bool IsVisibleBar
        {
            get
            {
                return !_isLoaded && !_isError;
            }
        }

        public bool IsVisibleErrorMessage
        {
            get
            {
                return _isError;
            }
        }

        private UpdateTemperatureCommand _updateTemperatureCommand;
        public UpdateTemperatureCommand UpdateCommand
        {
            get { return _updateTemperatureCommand ?? (_updateTemperatureCommand = new UpdateTemperatureCommand(UpdateTemperature)); }
        }
        #endregion

        public TemperatureModel()
        {
            UpdateTemperature();
        }

        private async void UpdateTemperature()
        {
            IsError = false;
            IsLoad = false;

            try
            {
                var weatherData = await DataLoader.LoadTemperatureWithGraph();
                Temperature = (int) Math.Round(weatherData.Current, 0);
                Graph = new GraphModel
                {
                    TemperatureCollection = weatherData.Graphic.TemperatureList,
                    MinValue = (int) weatherData.Graphic.TemperatureList.Max(x => x.Value),
                    MaxValue = (int) weatherData.Graphic.TemperatureList.Min(x => x.Value)
                };

                PlotModel = GeneratePlotModel(weatherData.Graphic.TemperatureList);

                UpdateTime = DateTime.Now;
                IsLoad = true;
            }
            catch (Exception)
            {
                IsError = true;
                IsLoad = false;
            }
        }

        private PlotModel GeneratePlotModel(IList<Temperature> temperatureList)
        {
            var model = new PlotModel();
            model.Axes.Add(new DateTimeAxis
            {
                TickStyle = TickStyle.Inside,
                //MajorGridlineStyle = LineStyle.Dash,
                IsZoomEnabled = false,
                IsPanEnabled = false,
                StringFormat = "d-MMM",
            });
            model.Axes.Add(new LinearAxis
            {
                TickStyle = TickStyle.Inside,
                //MajorGridlineStyle = LineStyle.Dash,
                IsZoomEnabled = false,
                IsPanEnabled = false,
                StringFormat = "0°",
                FontSize = 20
            });
            model.PlotAreaBorderColor = OxyColors.Transparent;
            var areaSeries = new AreaSeries();

            for (int i = temperatureList.Count - 1; i >= 0; i -= 10)
            {
                areaSeries.Points.Add(new DataPoint
                {
                    X = DateTimeAxis.ToDouble(temperatureList[i].Timestamp.FromUnixEpochToDateTime()),
                    Y = (int)Math.Round(temperatureList[i].Value, 0)
                });

            }

            areaSeries.Color = OxyColor.FromArgb(255, 41, 149, 70);
            areaSeries.Fill = OxyColor.FromArgb(87, 41, 149, 70);

            areaSeries.Points2.Add(new DataPoint(areaSeries.Points[0].X, 0));
            areaSeries.Points2.Add(new DataPoint(areaSeries.Points.Last().X, 0));
            areaSeries.Color2 = OxyColor.FromRgb(255, 255, 255);

            model.Series.Add(areaSeries);
            return model;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class UpdateTemperatureCommand : ICommand
    {
        private Action execute;

        public UpdateTemperatureCommand(Action execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            execute();
        }

        public event EventHandler CanExecuteChanged;
    }

    public class GraphicModel
    {
        public DateTime Time { get; set; }

        public int Value { get; set; }
    }
}