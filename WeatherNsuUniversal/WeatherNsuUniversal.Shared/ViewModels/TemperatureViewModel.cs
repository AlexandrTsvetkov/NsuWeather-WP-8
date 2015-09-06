using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using WeatherNsuUniversal.Common;
using WeatherNsuUniversal.Common.Extensions;


namespace WeatherNsuUniversal.ViewModels
{
    public class TemperatureViewModel : INotifyPropertyChanged
    {
        public string Sign
        {
            get { return _temperature < 0 ? "–" : string.Empty; }
        }

        private int _temperature;
        public int Temperature
        {
            get { return Math.Abs(_temperature); }
            set
            {
                _temperature = value;
                OnPropertyChanged();
            }
        }

        private PlotModel _plotModel;
        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set
            {
                _plotModel = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ForecastViewModel> _forecastCollection;
        public ObservableCollection<ForecastViewModel> ForecastCollection
        {
            get { return _forecastCollection; }
        }

        public void ManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            //TODO: Handler manipulation started event here
        }

        public void ManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            //TODO: Handler manipulation completed event here
        }

        private ManipulationCommand _manipulationCommand;

        public ManipulationCommand ManipulationCommand
        {
            get { return _manipulationCommand; }
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
            get { return string.Format("Обновлено в {0}", _updateTime); }
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

        private ButtonClickCommand _updateTemperatureCommand;
        public ButtonClickCommand UpdateTemperatureCommand
        {
            get { return _updateTemperatureCommand ?? (_updateTemperatureCommand = new ButtonClickCommand(UpdateTemperature)); }
        }

        public TemperatureViewModel()
        {
            _manipulationCommand = new ManipulationCommand(ManipulationStarted, ManipulationCompleted);

            UpdateTemperature();
        }

        public async void UpdateTemperature()
        {
            IsError = false;
            IsLoad = false;

            try
            {
                var weatherData = await DataLoader.LoadTemperatureWithGraph();
                Temperature = (int)Math.Round(weatherData.Current, 0);
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
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
                IsAxisVisible = false
            });
            model.Axes.Add(new LinearAxis
            {
                TickStyle = TickStyle.Inside,
                //MajorGridlineStyle = LineStyle.Dash,
                IsZoomEnabled = false,
                IsPanEnabled = false,
                StringFormat = "0°",
                FontSize = 20,
                IsAxisVisible = false
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

            areaSeries.Color = OxyColors.White;
            areaSeries.StrokeThickness = 3;
            areaSeries.Fill = OxyColors.Transparent;
            

            model.Series.Add(areaSeries);
            return model;
        }
    }

    public class ButtonClickCommand : ICommand
    {
        private Action _execute;

        public ButtonClickCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged;
    }

    public class ManipulationCommand : ICommand
    {
        private Action<ManipulationStartedRoutedEventArgs> _executeStarted;
        private Action<ManipulationCompletedRoutedEventArgs> _executeCompleted;

        public ManipulationCommand(Action<ManipulationStartedRoutedEventArgs> started,
            Action<ManipulationCompletedRoutedEventArgs> completed)
        {
            _executeStarted = started;
            _executeCompleted = completed;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is ManipulationStartedRoutedEventArgs)
                _executeStarted((ManipulationStartedRoutedEventArgs) parameter);
            else if (parameter is ManipulationCompletedRoutedEventArgs)
                _executeCompleted((ManipulationCompletedRoutedEventArgs)parameter);
        }

        public event EventHandler CanExecuteChanged;
    }

    public class ForecastViewModel
    {
        public string WeekDay { get; set; }

        public string DayTemperature { get; set; }

        public string NightTemperature { get; set; }
    }
}