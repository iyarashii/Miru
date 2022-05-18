// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System.Collections.Generic;

namespace MiruLibrary.Models
{
    // model for senpai JSON data serialization
    public class SenpaiEntryModel
    {
        public List<Item> Items { get; set; }

        public class Item
        {
            public long MalId { get; set; }
            public string Airdate { get; set; }
        }
    }
}