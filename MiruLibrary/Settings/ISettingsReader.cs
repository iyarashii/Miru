// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;

namespace MiruLibrary.Settings
{
    public interface ISettingsReader
    {
        T Load<T>() where T : class, new();
        object Load(Type type);
    }
}
