using ModernWpf.Controls;
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
        void Config(object title, string primaryButtonText = "Yes", string closeButtonText = "No",
            ContentDialogButton defaultButton = ContentDialogButton.Primary);
    }
}