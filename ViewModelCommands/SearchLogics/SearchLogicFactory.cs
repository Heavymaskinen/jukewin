using System.Collections.Generic;

namespace Juke.UI.SearchLogics
{
    public class SearchLogicFactory
    {
        public IList<SearchLogic> CreateAll(SelectionModel viewModel)
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