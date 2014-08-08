using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using NsuWeatherMobile.Common;

namespace NsuWeatherMobile
{
    public partial class MainPage : PhoneApplicationPage
    {
        private const string PeriodicTaskName = "LiveTileUpdater";
        private const string TaskDescription = "NSU weather LiveTile agent";

        private Random random = new Random();

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            StartPeriodicAgent();
        }

        private void StartPeriodicAgent()
        {
            try
            {
                var periodicTask = ScheduledActionService.Find(PeriodicTaskName) as PeriodicTask;

                if (periodicTask != null)
                    ScheduledActionService.Remove(PeriodicTaskName);

                periodicTask = new PeriodicTask(PeriodicTaskName) {Description = TaskDescription};

                ScheduledActionService.Add(periodicTask);
            }
            catch
            {}
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}