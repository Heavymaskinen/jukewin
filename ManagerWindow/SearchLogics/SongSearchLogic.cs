using DataModel;
using System;
using System.Collections.Generic;

namespace Juke.UI.Wpf.SearchLogics
{
    class SongSearchLogic : SearchLogic
    {
        private JukeViewModel browser;
        private List<Song> list;

        public SongSearchLogic(JukeViewModel browser)
        {
            this.browser = browser;
            list = new List<Song>();
        }

        public string Name => "Song titles";

        public List<Song> Search(string input)
        {
            list.Clear();
            var lowerInput = input.ToLower();
            foreach (var song in browser.Songs)
            {
                if (song.Name.ToLower().Contains(lowerInput))
                {
                    list.Add(song);
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