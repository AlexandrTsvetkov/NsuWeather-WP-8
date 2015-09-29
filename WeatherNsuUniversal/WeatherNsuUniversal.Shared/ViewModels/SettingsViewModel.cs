using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WeatherNsuUniversal.Tasks;

namespace WeatherNsuUniversal.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private string _taskName;

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsViewModel()
        {
            ResourceLoader loader = new ResourceLoader("Resources");
            _taskName = loader.GetString("TaskName");

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == _taskName)
                    IsChecked = true;
            }
        }

        public void Toogled(object sender, RoutedEventArgs e)
        {
            var toogle = e.OriginalSource as ToggleSwitch;
            if (toogle == null)
                return;

            if (toogle.IsOn)
            {
                var builder = new BackgroundTaskBuilder
                {
                    Name = _taskName,
                    TaskEntryPoint = typeof (DownloadTemperatureBackgroundTask).ToString()
                };
                builder.SetTrigger(new TimeTrigger(15, false));

                try
                {
#if WINDOWS_PHONE_APP
                    BackgroundExecutionManager.RequestAccessAsync();
#endif
                    builder.Register();
                }
                catch
                {
                    MessageDialog mg = new MessageDialog("Не удалось зарегистрировать фоновый процесс");
                    mg.ShowAsync();
                    toogle.IsOn = false;
                }
            }
            else
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == _taskName)
                    {
                        task.Value.Unregister(false);
                        return;
                    }
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
