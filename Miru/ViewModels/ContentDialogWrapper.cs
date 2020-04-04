using ModernWpf.Controls;

namespace Miru.ViewModels
{
    public class ContentDialogWrapper : ContentDialog, IContentDialogWrapper
    {
        public void Config(object title, string primaryButtonText = "Yes", string closeButtonText = "No",
            ModernWpf.Controls.ContentDialogButton defaultButton = ModernWpf.Controls.ContentDialogButton.Primary)
        {
            Title = title;
            PrimaryButtonText = primaryButtonText;
            CloseButtonText = closeButtonText;
            DefaultButton = defaultButton;
        }
    }
}
