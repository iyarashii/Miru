using ModernWpf.Controls;
using System.Threading.Tasks;

namespace Miru.ViewModels
{
    public interface IContentDialogWrapper
    {
        Task<ContentDialogResult> ShowAsync();
        object Title { get; set; }
        string PrimaryButtonText { get; set; }
        string CloseButtonText { get; set; }
        ContentDialogButton DefaultButton { get; set; }
    }
}