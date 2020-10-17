using System;
using System.IO;

namespace MiruLibrary
{
    public static class Constants
    {
        public static string SenpaiFilePath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), @"MiruCache\senpai-data.json");

        public static string ImageCacheFolderPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "MiruCache");

        public static string SenpaiDataSourceURL { get; } = @"http://www.senpai.moe/export.php?type=json&src=raw";
        public static string SettingsPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), @"MiruCache\config.json");
    }
}