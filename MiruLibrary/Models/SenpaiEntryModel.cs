using System.Collections.Generic;

namespace MiruLibrary.Models
{
    // model for senpai JSON data serialization
    public class SenpaiEntryModel
    {
        public List<Item> Items { get; set; }

        public class Item
        {
            public long MALID { get; set; }
            public string airdate { get; set; }
        }
    }
}