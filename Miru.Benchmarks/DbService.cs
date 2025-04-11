// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Autofac;
using BenchmarkDotNet.Attributes;
using JikanDotNet;
using MiruDatabaseLogicLayer;
using MiruLibrary;
using MiruLibrary.Models;
using MiruLibrary.Services;
using MyInternetConnectionLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgeRating = MiruLibrary.AgeRating;

namespace Miru.Benchmarks
{
    public class DbService
    {
        private MiruDbService _miruDbService;

        [GlobalSetup]
        public void Setup()
        {
            var builder = new ContainerBuilder();

            // Register dependencies as in Bootstrapper.cs
            builder.RegisterType<MiruDbService>().As<IMiruDbService>();
            builder.RegisterType<UserDataService>().As<IUserDataService>();
            builder.RegisterType<Jikan>().As<IJikan>();
            builder.RegisterType<WebService>().As<IWebService>();
            builder.RegisterType<FileSystemService>().As<IFileSystemService>();
            builder.RegisterType<MiruAnimeModel>();
            builder.RegisterType<MiruDbContext>().As<IMiruDbContext>();

            // Build the container
            var container = builder.Build();

            // Resolve the MiruDbService
            _miruDbService = container.Resolve<MiruDbService>();
        }

        [Benchmark]
        public void ChangeDisplayedAnimeList_ByTitle()
        {
            // Arrange
            var animeListType = AnimeListType.Watching;
            var selectedTimeZone = TimeZoneInfo.Local;
            var selectedAnimeBroadcastType = MiruLibrary.AnimeType.TV;
            var animeTitleToFilterBy = "Some Anime Title";

            // Act
            _miruDbService.ChangeDisplayedAnimeList(animeListType, selectedTimeZone, selectedAnimeBroadcastType, animeTitleToFilterBy, AgeRating.Any);
        }
    }
}
