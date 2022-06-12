// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Microsoft.Win32;
using System.Diagnostics;

namespace MiruLibrary.Services
{
    public interface ISystemService
    {
        void ExitEnvironment(int exitCode);
        RegistryKey OpenLocalMachineSubKey(string name);
        Process StartProcess(string fileName);
    }
}