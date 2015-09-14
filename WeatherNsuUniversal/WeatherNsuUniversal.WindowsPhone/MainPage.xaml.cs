using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WeatherNsuUniversal.Tasks;

namespace WeatherNsuUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainPage : Page
    {
        private const string TaskName = "WeatherNsuLiveTileUpdater";

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RegisterBackgroundTask();
        }

        private void RegisterBackgroundTask()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == TaskName)
                    return;
            }


            var builder = new BackgroundTaskBuilder();
            builder.Name = TaskName;
            builder.TaskEntryPoint = typeof (DownloadTemperatureBackgroundTask).ToString();
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
                //TODO: Handle exception of background registration
            }
        }

        private double _startedPosition;
        private double _completedPosition;

        private CompositeTransform _plotViewCompositeTransform;
        private CompositeTransform _forecastCompositeTransform;

        private void Page_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _startedPosition = e.Position.X;

            _plotViewCompositeTransform = plotView.RenderTransform as CompositeTransform;
            _forecastCompositeTransform = ForecastControl.RenderTransform as CompositeTransform;
        }

        private void Page_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            _completedPosition = e.Position.X;

            if (_startedPosition > _completedPosition)
                VisualStateManager.GoToState(this, ChartState.Name, true);
            else if (_startedPosition < _completedPosition)
                VisualStateManager.GoToState(this, ForecastState.Name, true);
        }

        private void Page_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (VisualStateGroup.CurrentState == ForecastState)
            {
                if (_forecastCompositeTransform.TranslateX + e.Delta.Translation.X > 0)
                    return;
            }

            if (VisualStateGroup.CurrentState == ChartState)
            {
                if (_plotViewCompositeTransform.TranslateX + e.Delta.Translation.X < 0)
                    return;
            }

            _plotViewCompositeTransform.TranslateX += e.Delta.Translation.X;
            _forecastCompositeTransform.TranslateX += e.Delta.Translation.X;
        }
    }
}
