using System;
using System.ComponentModel;
using System.Windows.Input;

namespace NsuWeatherMobile.ViewModel
{
    public class TemperatureModel : INotifyPropertyChanged 
    {
        private float temperature;
        private bool isLoaded;
        private UpdateTemperatureCommand updateTemperatureCommand;
        private DateTime updateTime;
        private bool isError;

        public async void UpdateTemperature()
        {
            IsError = false;
            IsLoad = false;
            try
            {
                Temperature = await Common.DataLoader.LoadTemperature();
                UpdateTime = DateTime.Now;
                IsLoad = true;
            }
            catch (Exception)
            {
                IsError = true;
                IsLoad = false;
            }

        }

        public float Temperature
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
            

        public string IsVisibleTextBlock
        {
            get 
            {
                if (isLoaded && !isError)
                    return "Visible";
                return "Collapsed";
            }
        }

        public string IsVisibleBar
        {
            get
            {
                if (!isLoaded && !isError)
                    return "Visible";
                return "Collapsed";
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

        public string IsVisibleErrorMessage
        {
            get
            {
                if (!isError)
                    return "Collapsed";
                return "Visible";
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