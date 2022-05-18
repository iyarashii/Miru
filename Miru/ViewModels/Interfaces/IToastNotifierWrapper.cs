// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using ToastNotifications.Core;

namespace Miru.ViewModels
{
    public interface IToastNotifierWrapper
    {
        void DisplayToastNotification(string message);
        MessageOptions DoNotFreezeOnMouseEnter { get; }
    }
}