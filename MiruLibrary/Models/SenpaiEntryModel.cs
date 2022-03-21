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