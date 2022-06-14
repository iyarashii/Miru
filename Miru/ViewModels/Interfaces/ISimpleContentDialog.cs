// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using ModernWpf.Controls;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Miru.ViewModels
{
    public interface ISimpleContentDialog
    {
        Task<ContentDialogResult> ShowAsync();
        object Title { get; set; }
        string PrimaryButtonText { get; set; }
        string CloseButtonText { get; set; }
        ContentDialogButton DefaultButton { get; set; }

        [ExcludeFromCodeCoverage]
        void Config(object title, string primaryButtonText = "Yes", string closeButtonText = "No",
            ContentDialogButton defaultButton = ContentDialogButton.Primary, object content = null, string secondaryButtonText = "");
    }
}