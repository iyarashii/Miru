using System.Collections.Generic;

namespace MiruLibrary.Models
{
    public interface IMiruAnimeModelExtensionsWrapper
    {
        void FilterByTitle(List<MiruAnimeModel> animeList, string title);
    }
}