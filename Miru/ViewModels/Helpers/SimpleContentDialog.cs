﻿// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using ModernWpf.Controls;

namespace Miru.ViewModels
{
    public class SimpleContentDialog : ContentDialog, ISimpleContentDialog
    {
        public void Config(object title, string primaryButtonText = "Yes", string closeButtonText = "No",
            ContentDialogButton defaultButton = ContentDialogButton.Primary, object content = null, string secondaryButtonText = "")
        {
            Title = title;
            PrimaryButtonText = primaryButtonText;
            CloseButtonText = closeButtonText;
            DefaultButton = defaultButton;
            Content = content;
            SecondaryButtonText = secondaryButtonText;
        }
    }
}
