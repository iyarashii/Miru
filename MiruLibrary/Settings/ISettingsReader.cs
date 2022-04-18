using System;

namespace MiruLibrary.Settings
{
    public interface ISettingsReader
    {
        T Load<T>() where T : class, new();
        object Load(Type type);
    }
}
