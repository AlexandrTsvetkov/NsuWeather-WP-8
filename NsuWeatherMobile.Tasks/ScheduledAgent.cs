using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

namespace NsuWeatherMobile.Tasks
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private int index;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            try
            {
                UpdateMainTile();
                ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromMinutes(10));
            }
            catch (Exception)
            {
                ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
            }

            NotifyComplete();
        }

        public void UpdateMainTile()
        {
            var mainTile = ShellTile.ActiveTiles.FirstOrDefault();

            if (null != mainTile)
            {
                FlipTileData tileData = new FlipTileData()
                                            {
                                                BackContent =
                                                    string.Format("Температура сейчас:{0}",
                                                                  NsuWeatherMobile.Common.DataLoader.LoadTemperature
                                                                      ().Result)
                                            };

                mainTile.Update(tileData);
            }
        }
    }
}