﻿using JikanDotNet;
using System;
using System.IO;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace Miru
{
    public static class Constants
    {
        public static string SenpaiFilePath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "senpai.json");
        public static string ImageCacheFolderPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "MiruCache"); 
        public static string SenpaiDataSourceURL { get; } = @"http://www.senpai.moe/export.php?type=json&src=raw";
    }
}