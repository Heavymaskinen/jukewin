using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.UI.Wpf.SearchLogics
{
    public class ArtistSearchLogic : SearchLogic
    {
        public string Name => "Artist names";

        private JukeViewModel browser;
        private List<Song> list;

        public ArtistSearchLogic(JukeViewModel browser)
        {
            this.browser = browser;
            list = new List<Song>();
        }

        public List<Song> Search(string input)
        {
            if (input == string.Empty || input.Length < 2) return list;

            list.Clear();
            var lowerName = input.ToLower();
            var artistList = browser.Artists.ToArray();
            foreach (var art in artistList)
            {
                if (art.ToLower().StartsWith(lowerName))
                {
                    var songs = browser.GetSongsByArtist(art);
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