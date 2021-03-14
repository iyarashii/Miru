using System;
using System.Collections.Generic;
using System.Text;

namespace MyInternetConnectionLibrary
{
    public interface IWebClientWrapper : IDisposable
    {
        void DownloadFile(string address, string fileName);
    }
}
