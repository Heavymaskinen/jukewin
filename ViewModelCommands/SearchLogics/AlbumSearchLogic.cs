using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.UI.SearchLogics
{
    public class AlbumSearchLogic : SearchLogic
    {
        public string Name => "Album titles";

        private SelectionModel browser;
        private List<Song> list;

        public AlbumSearchLogic(SelectionModel browser)
        {
            this.browser = browser;
            list = new List<Song>();
        }

        public List<Song> Search(string input)
        {
            list.Clear();
            var lowerName = input.ToLower();
            var albumList = browser.SelectionTracker.Albums.ToArray();
            foreach (var al in albumList)
            {
                if (al.ToLower().Contains(lowerName))
                {
                    browser.SelectionTracker.SelectedAlbum = al;
                    list.AddRange(browser.SelectionTracker.Songs);
                }
            }

            browser.SelectionTracker.SelectedAlbum = Song.ALL_ALBUMS;
            return list;
        }

        public override string ToString() => Name;
    }
}