using DataModel;
using System.Collections.Generic;
using System.Linq;

namespace Juke.UI.SearchLogics
{
    public class ArtistSearchLogic : SearchLogic
    {
        public string Name => "Artist names";

        private SelectionModel browser;
        private List<Song> list;

        public ArtistSearchLogic(SelectionModel browser)
        {
            this.browser = browser;
            list = new List<Song>();
        }

        public List<Song> Search(string input)
        {
            if (input == string.Empty || input.Length < 2) return list;

            list.Clear();
            var lowerName = input.ToLower();
            var artistList = browser.SelectionTracker.Artists.ToArray();
            foreach (var art in artistList)
            {
                if (art.ToLower().StartsWith(lowerName))
                {
                    browser.SelectionTracker.SelectedArtist = art;
                    var songs = browser.SelectionTracker.Songs;
                    list.AddRange(songs);
                }
            }

            return list;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}