using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiruLibrary.Settings
{
    public class SettingsReader : ISettingsReader
    {
        private readonly string _configurationFilePath;
        private readonly string _sectionNameSuffix;
        private readonly IFileSystemService _fileSystemService;

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public SettingsReader(IFileSystemService fileSystemService,
            string configurationFilePath, string sectionNameSuffix = "Settings")
        {
            _fileSystemService = fileSystemService;
            _configurationFilePath = configurationFilePath;
            _sectionNameSuffix = sectionNameSuffix;
        }

        public T Load<T>() where T : class, new() => Load(typeof(T)) as T;

        public object Load(Type type)
        {
            if (!_fileSystemService.FileSystem.File.Exists(_configurationFilePath))
                return Activator.CreateInstance(type);

            var jsonFile = _fileSystemService.FileSystem.File.ReadAllText(_configurationFilePath);

            return JsonConvert.DeserializeObject(jsonFile, type, JsonSerializerSettings);
        }
    }
}
