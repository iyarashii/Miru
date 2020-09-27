using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiruLibrary.Settings
{
    public interface ISettingsWriter
    {
        void Write(object settingsData);
    }
}
