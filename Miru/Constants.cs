using JikanDotNet;
using System;
using System.IO;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace Miru
{
    public static class Constants
    {
        // Initialize JikanWrapper
        public readonly static IJikan jikan = new Jikan();

        public readonly static Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: App.Current.MainWindow,
                corner: Corner.BottomCenter,
                offsetX: 0,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = App.Current.Dispatcher;
        });

        public readonly static MessageOptions messageOptions = new MessageOptions { FreezeOnMouseEnter = false };
        //public static string SenpaiFilePath { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "senpai.json"); } }
        public readonly static string SenpaiFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "senpai.json");

        //public readonly static string senpai = MyInternetConnectionLibrary.InternetConnection.client.GetStringAsync(@"http://www.senpai.moe/export.php?type=json&src=raw").Result;
        //public readonly static string senpai = File.ReadAllText(@"H:\senpai.json");
    }
}