﻿// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace MiruLibrary.Models
{
    // data model for the anime entries used by this app
    public class MiruAnimeModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long MalId { get; set; }
        public string Broadcast { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public string ImageURL { get; set; }

        private string _localImagePath;
        public string LocalImagePath 
        { 
            get => _localImagePath;
            set
            {
                if (value.Contains(@"\"))
                {
                    _localImagePath = value;
                }
                else
                {
                    _localImagePath = Path.Combine(Constants.ImageCacheFolderPath, $"{ value }.jpg");
                }
            }
        }

        // returns cached image to the view
        public BitmapImage LocalImageSource
        {
            get
            {
                if (!File.Exists(LocalImagePath))
                {
                    return null;
                }
                var source = new BitmapImage();
                source.BeginInit();
                source.UriSource = new Uri(LocalImagePath, UriKind.RelativeOrAbsolute);
                source.CacheOption = BitmapCacheOption.OnLoad;
                source.EndInit();
                return source;
            }
        }
        public DateTime? LocalBroadcastTime { get; set; }
        public DateTime? JSTBroadcastTime { get; set; }
        public bool IsOnWatchingList { get; set; }
        public bool IsOnSenpai { get; set; }

        // is the anime currently airing status flag
        public bool CurrentlyAiring { get; set; }
        public int? WatchedEpisodes { get; set; }
        public string AgeRating { get; set; } // "G", "PG", "R" etc.

        // "Movie", "TV" etc.
        public string Type { get; set; }

        private int? _totalEpisodes;

        public int? TotalEpisodes
        {
            get
            {
                if (_totalEpisodes == 0)
                {
                    return null;
                }
                return _totalEpisodes;
            }
            set { _totalEpisodes = value; }
        }

        public bool Dropped { get; set; }

        private string _openingThemes;
        //     Anime's opening themes numerically indexed with array values.
        public string OpeningThemes 
        {
            get => FormatThemesOutput("OP", _openingThemes);
            set => _openingThemes = value;
        }
        private string _endingThemes;

        //     Anime's ending themes numerically indexed with array values.
        public string EndingThemes
        {
            get => FormatThemesOutput("ED", _endingThemes);
            set => _endingThemes = value;
        }

        public string FormatThemesOutput(string themeType, string themesJsonContent)
        {
            StringBuilder uiOutput = new StringBuilder($"{themeType}\n");
            try
            {
                var themes = JsonConvert.DeserializeObject<List<string>>(themesJsonContent);
                for (int i = 0; i < themes.Count; i++)
                {
                    uiOutput.Append($"{themes[i]}\n");
                }
            }
            catch (Exception)
            {
                uiOutput = new StringBuilder(themesJsonContent);
            }

            return uiOutput.ToString();
        }
    }
}