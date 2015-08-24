using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WeatherNsuUniversal.Common;

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

        private UpdateTemperatureCommand _updateTemperatureCommand;
        public UpdateTemperatureCommand UpdateCommand
        {
            get { return _updateTemperatureCommand ?? (_updateTemperatureCommand = new UpdateTemperatureCommand(UpdateTemperature)); }
        }

        public TemperatureViewModel()
        {
            UpdateTemperature();
        }

        public async void UpdateTemperature()
        {
            IsError = false;
            IsLoad = false;

            try
            {
                var weatherData = await DataLoader.LoadTemperatureWithGraph();
                Temperature = (int) Math.Round(weatherData.Current, 0);

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