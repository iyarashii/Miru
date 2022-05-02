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

            return JsonConvert.DeserializeObject(jsonFile, type, JsonSerializerSettings);
        }
    }
}
