using System.Diagnostics;

namespace Miru.ViewModels
{
    public interface IProcessProxy
    {
        //void Start(string URL);
        ProcessStartInfo StartInfo { get; set; }
        bool Start();
    }
}