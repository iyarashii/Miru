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
