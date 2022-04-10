﻿using Newtonsoft.Json;
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
    public class SettingsWriter : ISettingsWriter
    {
        private readonly string _configurationFilePath;
        private readonly IFileSystemService _fileSystemService;

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new SettingsReaderContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public SettingsWriter(IFileSystemService fileSystemService, string configurationFilePath)
        {
            _fileSystemService = fileSystemService;
            _configurationFilePath = configurationFilePath;
        }

        public void Write(object settingsData)
        {
            var jsonString = JsonConvert.SerializeObject(settingsData, Formatting.Indented, JsonSerializerSettings);
            _fileSystemService.FileSystem.File.WriteAllText(_configurationFilePath, jsonString);
        }

        private class SettingsReaderContractResolver : DefaultContractResolver
        {
            public SettingsReaderContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy();
            }
        }
    }
}
