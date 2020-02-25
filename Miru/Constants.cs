using JikanDotNet;
using System;
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
    }
}