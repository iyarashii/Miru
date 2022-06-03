// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;
using System.Diagnostics.CodeAnalysis;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Miru.ViewModels
{
    [ExcludeFromCodeCoverage]
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

        public void DisplayToastNotification(string message)
        {
            ToastNotifier.ShowInformation(message, DoNotFreezeOnMouseEnter);
        }
    }
}
