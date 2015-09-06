using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WeatherNsuUniversal.Common;
using WeatherNsuUniversal.Common.Helpers;

namespace WeatherNsuUniversal.Tasks
{
    //public sealed class BackgroundTask : IBackgroundTask
    //{
    //    private int _temperature;

    //    public void Run(IBackgroundTaskInstance taskInstance)
    //    {
    //        Deployment.Current.Dispatcher.BeginInvoke(async () =>
    //        {
    //            try
    //            {
    //                var weather = await DataLoader.LoadTemperature();
    //                _temperature = (int)Math.Round(weather.Current, 0);

    //                GenerateTileImage(Constants.SmallTileSize, Constants.SmallTileSize);
    //                GenerateTileImage(Constants.TileSize, Constants.TileSize);
    //                GenerateTileImage(Constants.WideTileSize, Constants.TileSize);

    //                UpdateMainTile();
    //            }
    //            catch (Exception)
    //            {
    //                ResetMainTitle();
    //            }

    //            NotifyComplete();
    //        });
    //    }

    //    private void GenerateTileImage(int width, int height)
    //    {
    //        var bitmap = new WriteableBitmap(width, height);
    //        var grid = new Grid {Height = bitmap.PixelHeight, Width = bitmap.PixelWidth};

    //        var textBlock = new TextBlock
    //        {
    //            Text = string.Format("{0}°", _temperature),
    //            VerticalAlignment = VerticalAlignment.Center,
    //            HorizontalAlignment = HorizontalAlignment.Center,
    //            Foreground = new SolidColorBrush(Colors.White),
    //            FontSize = height/2
    //        };

    //        grid.Children.Add(textBlock);
    //        grid.Arrange(new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

    //        bitmap.Render(grid, null);
    //        bitmap.Invalidate();

    //        using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
    //        {
    //            var fileName = string.Empty;
    //            switch (width)
    //            {
    //                case Constants.SmallTileSize:
    //                    fileName = "tileSmall";
    //                    break;
    //                case Constants.TileSize:
    //                    fileName = "tile";
    //                    break;
    //                case Constants.WideTileSize:
    //                    fileName = "tileWide";
    //                    break;
    //            }

    //            var filePath = string.Format("/Shared/ShellContent/{0}.jpg", fileName);

    //            using (var stream = isf.OpenFile(filePath, FileMode.Create))
    //            {
    //                bitmap.WritePNG(stream);
    //            }
    //        }
    //    }

        //public void UpdateMainTile()
        //{
        //    var mainTile = ShellTile.ActiveTiles.FirstOrDefault();

        //    if (mainTile != null)
        //    {
        //        FlipTileData tileData = new FlipTileData
        //        {
        //            BackgroundImage = new Uri("isostore:/Shared/ShellContent/tile.jpg", UriKind.Absolute),
        //            WideBackgroundImage = new Uri("isostore:/Shared/ShellContent/tileWide.jpg", UriKind.Absolute),
        //            SmallBackgroundImage = new Uri("isostore:/Shared/ShellContent/tileSmall.jpg", UriKind.Absolute)
        //        };

        //        mainTile.Update(tileData);
        //    }
        //}

    //    public void ResetMainTitle()
    //    {
    //        var mainTile = ShellTile.ActiveTiles.FirstOrDefault();

    //        if (mainTile != null)
    //        {
    //            mainTile.Update(new FlipTileData());
    //        }
    //    }
    //}

    public sealed class DownloadTemperatureBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text01);

            tileXml.GetElementsByTagName("text")[0].InnerText = DateTime.Now.ToLocalTime().ToString();
            updater.Update(new TileNotification(tileXml));






            deferral.Complete();
        }
    }
}