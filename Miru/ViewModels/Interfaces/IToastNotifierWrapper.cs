using ToastNotifications.Core;

namespace Miru.ViewModels
{
    public interface IToastNotifierWrapper
    {
        void ShowInformation(string message, MessageOptions displayOptions);
        MessageOptions DoNotFreezeOnMouseEnter { get; }
    }
}