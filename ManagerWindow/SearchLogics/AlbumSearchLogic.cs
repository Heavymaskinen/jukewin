using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.UI.Wpf.SearchLogics
{
    public class AlbumSearchLogic : SearchLogic
    {
        public string Name => "Album titles";

        private JukeViewModel browser;
        private List<Song> list;

        public AlbumSearchLogic(JukeViewModel browser)
        {
            this.browser = browser;
            list = new List<Song>();
        }

        public List<Song> Search(string input)
        {
            list.Clear();
            var lowerName = input.ToLower();
            var albumList = browser.Albums.ToArray();
            foreach (var al in albumList)
            {
                if (al.ToLower().Contains(lowerName))
                {
                    browser.SelectedAlbum = al;
                    list.AddRange(browser.Songs);
                }
            }

            browser.SelectedAlbum = Song.ALL_ALBUMS;
            return list;
        }

        public override string ToString() => Name;
    }
}