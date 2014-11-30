using System;
using System.ComponentModel;
using System.Windows.Input;
using NsuWeatherMobile.Common;

namespace NsuWeatherMobile.ViewModel
{
    public class TemperatureModel : INotifyPropertyChanged 
    {
        private int temperature;
        private bool isLoaded;
        private bool isError;
        private DateTime updateTime;

        private UpdateTemperatureCommand updateTemperatureCommand;
        
        public async void UpdateTemperature()
        {
            IsError = false;
            IsLoad = false;
            try
            {
                Temperature = (int) Math.Round(await DataLoader.LoadTemperature(), 0);
                UpdateTime = DateTime.Now;
                IsLoad = true;
            }
            catch (Exception)
            {
                IsError = true;
                IsLoad = false;
            }

        }

        public int Temperature
        {
            get { return temperature; }
            set
            {
                temperature = value;
                OnPropertyChanged("TemperatureAtNsu");
            }
        }

        public DateTime UpdateTime
        {
            get { return updateTime; }
            set
            {
                updateTime = value;
                OnPropertyChanged("RefreshTimeString");
            }
        }

        public string RefreshTimeString
        {
            get { return string.Format("Обновлено в {0}", updateTime.ToShortTimeString()); }
        }

        public string TemperatureAtNsu
        {
            get { return string.Format("{0}°", temperature); }
        }

        private bool IsLoad
        {
            set
            {
                isLoaded = value;
                OnPropertyChanged("IsVisibleTextBlock");
                OnPropertyChanged("IsVisibleBar");
            }
        }

        public bool IsError
        {
            set
            {
                isError = value;
                OnPropertyChanged("IsVisibleErrorMessage");
            }
        }

        public bool IsVisibleTextBlock
        {
            get 
            {
                return isLoaded && !isError;
            }
        }

        public bool IsVisibleBar
        {
            get
            {
                return !isLoaded && !isError;
            }
        }

        public bool IsVisibleErrorMessage
        {
            get
            {
                return isError;
            }
        }

        public UpdateTemperatureCommand UpdateCommand
        {
            get { return updateTemperatureCommand ?? (updateTemperatureCommand = new UpdateTemperatureCommand(UpdateTemperature)); }
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
}