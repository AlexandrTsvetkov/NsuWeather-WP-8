using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using NsuWeatherMobile.Common;

namespace NsuWeatherMobile.ViewModel
{
    public class TemperatureModel : INotifyPropertyChanged 
    {
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

        private IList<Temperature> _temperatureCollection;
        public IList<Temperature> TemperatureCollection
        {
            get { return _temperatureCollection; }
            set
            {
                _temperatureCollection = value;
                OnPropertyChanged("TemperatureCollection");
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

        public async void UpdateTemperature()
        {
            IsError = false;
            IsLoad = false;

            try
            {
                var weatherData = await DataLoader.LoadTemperatureWithGraph();
                Temperature = (int) Math.Round(weatherData.Current, 0);
                TemperatureCollection = weatherData.Graphic.TemperatureList;

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