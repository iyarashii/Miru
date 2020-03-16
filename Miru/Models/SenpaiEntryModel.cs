using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miru.Models
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
