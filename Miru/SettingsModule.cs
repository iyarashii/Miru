// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Autofac;
using MiruLibrary;
using MiruLibrary.Settings;
using System;
using System.Linq;
using System.Reflection;

namespace Miru
{
    public class SettingsModule : Autofac.Module
    {
        private readonly string _sectionNameSuffix;
        public SettingsModule(string sectionNameSuffix = "Settings")
        {
            _sectionNameSuffix = sectionNameSuffix;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SettingsReader>()
                .As<ISettingsReader>()
                .WithParameter("configurationFilePath", Constants.SettingsPath)
                .SingleInstance();

            builder.RegisterType<SettingsWriter>()
                .As<ISettingsWriter>()
                .WithParameter("configurationFilePath", Constants.SettingsPath)
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
