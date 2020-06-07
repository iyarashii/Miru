using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Windows.Media.Imaging;

namespace Miru.Models
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

        public string LocalImagePath { get; set; }

        // returns cached image to the view
        public BitmapImage LocalImageSource
        {
            get
            {
                var source = new BitmapImage();
                source.BeginInit();
                source.UriSource = new Uri(LocalImagePath, UriKind.RelativeOrAbsolute);
                source.CacheOption = BitmapCacheOption.OnLoad;
                source.EndInit(); 
                // not sure if GC still needs this source.Freeze();
                return source;
            }
        }
        public DateTime? LocalBroadcastTime { get; set; }
        public DateTime? JSTBroadcastTime { get; set; }
        public bool IsOnWatchingList { get; set; }

        // is the anime currently airing status flag
        public bool CurrentlyAiring { get; set; }
        public int? WatchedEpisodes { get; set; }

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
    }
}