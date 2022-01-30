using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiruLibrary.Models
{
    public class MiruAnimeModelExtensionsWrapper : IMiruAnimeModelExtensionsWrapper
    {
        public void FilterByTitle(List<MiruAnimeModel> animeList, string title)
        {
            animeList.FilterByTitle(title);
        }
    }
}
