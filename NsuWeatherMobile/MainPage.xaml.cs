using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;

namespace NsuWeatherMobile
{
    public partial class MainPage : PhoneApplicationPage
    {
        private const string PeriodicTaskName = "LiveTileUpdater";
        private const string TaskDescription = "NSU weather LiveTile agent";

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
            PeriodicTask periodicTask = ScheduledActionService.Find(PeriodicTaskName) as PeriodicTask;
            if (periodicTask == null)
            {
                periodicTask = new PeriodicTask(PeriodicTaskName);
                periodicTask.Description = TaskDescription;

                ScheduledActionService.Add(periodicTask);
                ScheduledActionService.LaunchForTest(PeriodicTaskName, TimeSpan.FromSeconds(10));
            }
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