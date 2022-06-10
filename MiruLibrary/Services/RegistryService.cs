﻿// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Microsoft.Win32;

namespace MiruLibrary.Services
{
    public class RegistryService : IRegistryService
    {
        public RegistryKey OpenLocalMachineSubKey(string name)
        {
            return Registry.LocalMachine.OpenSubKey(name);
        }
    }
}
