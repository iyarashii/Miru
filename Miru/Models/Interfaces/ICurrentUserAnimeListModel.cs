using JikanDotNet;
using System.Threading.Tasks;

namespace Miru.Models
{
    public interface ICurrentUserAnimeListModel
    {
        UserAnimeList UserAnimeListData { get; }

        Task<(bool Success, string ErrorMessage)> GetCurrentUserAnimeList(string malUsername);
    }
}