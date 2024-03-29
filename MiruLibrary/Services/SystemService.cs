﻿// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MiruLibrary.Services
{
    public class SystemService : ISystemService
    {
        public RegistryKey OpenLocalMachineSubKey(string name)
        {
            return Registry.LocalMachine.OpenSubKey(name);
        }

        [ExcludeFromCodeCoverage]
        public Process StartProcess(string fileName)
        {
            return Process.Start(fileName);
        }

        [ExcludeFromCodeCoverage]
        public void ExitEnvironment(int exitCode)
        {
            Environment.Exit(exitCode);
        }
    }
}
