using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miru.ViewModels
{
    public class ClipboardWrapper : IClipboardWrapper
    {
        public void SetText(string text)
        {
            System.Windows.Clipboard.SetText(text);
        }
    }
}
