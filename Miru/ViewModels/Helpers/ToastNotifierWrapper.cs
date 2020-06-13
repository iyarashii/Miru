using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Miru.ViewModels
{
    public class ToastNotifierWrapper : IToastNotifierWrapper
    {
        private Notifier ToastNotifier { get; } = new Notifier(config =>
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

        public MessageOptions DoNotFreezeOnMouseEnter { get; } = new MessageOptions { FreezeOnMouseEnter = false };

        public void ShowInformation(string message, MessageOptions displayOptions)
        {
            ToastNotifier.ShowInformation(message, displayOptions);
        }
    }
}
