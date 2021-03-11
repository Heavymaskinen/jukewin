using Juke.UI.Wpf.SearchLogics;
using System.Collections.Generic;

namespace Juke.UI.Wpf
{
    internal class SearchLogicFactory
    {
        internal IList<SearchLogic> CreateAll(JukeViewModel viewModel)
        {
            return new List<SearchLogic>
            {
                new SongSearchLogic(viewModel),
                new AlbumSearchLogic(viewModel),
                new ArtistSearchLogic(viewModel)
            };
        }
    }
}