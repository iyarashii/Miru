using System.Diagnostics;

namespace Miru.ViewModels
{
    public interface IProcessProxy
    {
        ProcessStartInfo StartInfo { get; set; }
        bool Start();
    }
}