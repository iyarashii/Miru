using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miru.Models
{
    public class MiruAiringAnimeModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long MalId { get; set; }
        public string Broadcast { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public string ImageURL { get; set; }
        public DateTime? LocalBroadcastTime { get; set; }
        public DateTime? JSTBroadcastTime { get; set; }
        public bool IsOnWatchingList { get; set; }
        public int? WatchedEpisodes { get; set; }

        private int? _totalEpisodes;

        public int? TotalEpisodes
        {
            get 
            { 
                if(_totalEpisodes == 0)
                {
                    return null;
                }
                return _totalEpisodes;
            }
            set { _totalEpisodes = value; }
        }

    }
}
