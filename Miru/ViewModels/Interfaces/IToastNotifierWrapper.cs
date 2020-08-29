using ToastNotifications.Core;

namespace Miru.ViewModels
{
    public interface IToastNotifierWrapper
    {
        void DisplayToastNotification(string message);
        MessageOptions DoNotFreezeOnMouseEnter { get; }
    }
}