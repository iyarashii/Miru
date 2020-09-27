using Autofac;
using MiruLibrary.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Miru
{
    public class SettingsModule : Autofac.Module
    {
        private readonly string _configurationFilePath;
        private readonly string _sectionNameSuffix;

        public SettingsModule(string configurationFilePath, string sectionNameSuffix = "Settings")
        {
            _configurationFilePath = configurationFilePath;
            _sectionNameSuffix = sectionNameSuffix;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new SettingsReader(_configurationFilePath, _sectionNameSuffix))
                .As<ISettingsReader>()
                .SingleInstance();

            builder.RegisterInstance(new SettingsWriter(_configurationFilePath))
                .As<ISettingsWriter>()
                .SingleInstance();

            var settings = Assembly.GetAssembly(typeof(MiruLibrary.Models.UserSettings))
                .GetTypes()
                .Where(t => t.Name.EndsWith(_sectionNameSuffix, StringComparison.InvariantCulture))
                .ToList();

            settings.ForEach(type =>
            {
                builder.Register(c => c.Resolve<ISettingsReader>().Load(type))
                    .As(type)
                    .SingleInstance();
            });
        }
    }
}
