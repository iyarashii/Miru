// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Newtonsoft.Json;
using System;

namespace MiruLibrary.Settings
{
    public class SettingsReader : ISettingsReader
    {
        private readonly string _configurationFilePath;
        private readonly IFileSystemService _fileSystemService;

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public SettingsReader(IFileSystemService fileSystemService, string configurationFilePath)
        {
            _fileSystemService = fileSystemService;
            _configurationFilePath = configurationFilePath;
        }

        public T Load<T>() where T : class, new() => Load(typeof(T)) as T;

        public object Load(Type type)
        {
            if (!_fileSystemService.FileSystem.File.Exists(_configurationFilePath))
                return Activator.CreateInstance(type);
            
            var jsonFile = _fileSystemService.FileSystem.File.ReadAllText(_configurationFilePath);

            // this is a fix for very annoying bug - when config.json becomes empty for any reason
            // without this check the app will crash on launch with very weird errors like
            /*
            Description: The process was terminated due to an unhandled exception.
            Exception Info: Autofac.Core.DependencyResolutionException
            at Autofac.Core.Activators.Delegate.DelegateActivator.ActivateInstance(Autofac.IComponentContext, System.Collections.Generic.IEnumerable`1<Autofac.Core.Parameter>)
            at Autofac.Core.Resolving.InstanceLookup.CreateInstance(System.Collections.Generic.IEnumerable`1<Autofac.Core.Parameter>)
            */
            if (string.IsNullOrEmpty(jsonFile))
            {
                jsonFile = JsonConvert.SerializeObject(Activator.CreateInstance(type), Formatting.Indented, JsonSerializerSettings);
                _fileSystemService.FileSystem.File.WriteAllText(_configurationFilePath, jsonFile);
            }

            return JsonConvert.DeserializeObject(jsonFile, type, JsonSerializerSettings);
        }
    }
}
