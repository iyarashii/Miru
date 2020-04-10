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
        public static Notifier ToastNotifier { get; } = new Notifier(config =>
        {
            config.PositionProvider = new WindowPositionProvider(
                parentWindow: App.Current.MainWindow,
                corner: Corner.BottomCenter,
                offsetX: 0,
                offsetY: 10);

            config.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            config.Dispatcher = App.Current.Dispatcher;
        });

        public static MessageOptions MessageOptions { get; } = new MessageOptions { FreezeOnMouseEnter = false };
        public static string SenpaiFilePath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "senpai.json");
        public static string ImageCacheFolderPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "MiruCache"); 
        public static string SenpaiDataSourceURL { get; } = @"http://www.senpai.moe/export.php?type=json&src=raw";
    }
}