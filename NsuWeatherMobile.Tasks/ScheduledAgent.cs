using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using NsuWeatherMobile.Common;
using NsuWeatherMobile.Common.Helpers;

namespace NsuWeatherMobile.Tasks
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private int temperature;

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

            Deployment.Current.Dispatcher.BeginInvoke(async () =>
            {
                try
                {
                    temperature = (int) Math.Round(await DataLoader.LoadTemperature(), 0);

                    GenerateTileImage(Constants.TileSize, Constants.TileSize);
                    GenerateTileImage(Constants.WideTileSize, Constants.TileSize);

                    UpdateMainTile();
                }
                catch (Exception)
                {
                    ResetMainTitle();
                }

                NotifyComplete();
            });
        }

        private void GenerateTileImage(int width, int height)
        {
            var bitmap = new WriteableBitmap(width, height);

            var grid = new Grid {Height = bitmap.PixelHeight, Width = bitmap.PixelWidth};

            var textBlock = new TextBlock
                                {
                                    Text = string.Format("{0}°", temperature),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    Foreground = new SolidColorBrush(Colors.White),
                                    FontSize = 150
                                };

            grid.Children.Add(textBlock);
            grid.Arrange(new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

            bitmap.Render(grid, null);
            bitmap.Invalidate();

            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var filePath = string.Format("/Shared/ShellContent/{0}.jpg", width == Constants.WideTileSize ? "tileWide" : "tile");

                using (var stream = isf.OpenFile(filePath, FileMode.Create))
                {
                    bitmap.WritePNG(stream);
                }
            }
        }

        public void UpdateMainTile()
        {
            var mainTile = ShellTile.ActiveTiles.FirstOrDefault();

            if (mainTile != null)
            {
                FlipTileData tileData = new FlipTileData
                                            {
                                                BackgroundImage = new Uri("isostore:" + "/Shared/ShellContent/tile.jpg", UriKind.Absolute),
                                                WideBackgroundImage = new Uri("isostore:" + "/Shared/ShellContent/tileWide.jpg", UriKind.Absolute)
                                            };

                mainTile.Update(tileData);
            }
        }

        public void ResetMainTitle()
        {
            var mainTile = ShellTile.ActiveTiles.FirstOrDefault();

            if (mainTile != null)
            {
                mainTile.Update(new FlipTileData());
            }
        }
    }
}