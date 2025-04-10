// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System.ComponentModel;

// file used as a storage for enums used in this app
namespace MiruLibrary
{
    public enum MiruAppStatus
    {
        Busy,
        Idle,
        InternetConnectionProblems
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum AnimeListType
    {
        [Description("Airing & Watching")]
        AiringAndWatching,

        [Description("Watching")]
        Watching,

        [Description("Current Season")]
        Season,

        [Description("Senpai - Current Season")]
        Senpai
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum AnimeType
    {
        [Description("TV & ONA")]
        Both,
        [Description("TV")]
        TV,
        [Description("ONA")]
        ONA,
        [Description("Any")]
        Any,
        [Description("Movie")]
        Movie,
        [Description("Special")]
        Special,
        [Description("OVA")]
        OVA
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum AgeRating
    {
        //[Description("G - All Ages")]
        //AllAges,
        //[Description("PG - Children")]
        //Children,
        [Description("Any")]
        Any,
        [Description("No PG and G rated (kids)")]
        ExcludeAllAgesAndChildren,
    }
}