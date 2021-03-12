using DataModel;
using System;
using System.Collections.Generic;

namespace Juke.UI.Wpf.SearchLogics
{
    public class SongSearchLogic : SearchLogic
    {
        private SelectionModel browser;
        private List<Song> list;

        public SongSearchLogic(SelectionModel browser)
        {
            this.browser = browser;
            list = new List<Song>();
        }

        public string Name => "Song titles";

        public List<Song> Search(string input)
        {
            list.Clear();
            var perfects = new List<Song>();
            var lowerInput = input.ToLower();
            foreach (var song in browser.Songs)
            {
                var lowerName = song.Name.ToLower();
                if (lowerName.Equals(lowerInput))
                {
                    perfects.Add(song);
                }
                else
                {
                    if (lowerName.StartsWith(lowerInput))
                    {
                        perfects.Add(song);
                    }
                    else if (lowerName.Contains(lowerInput))
                    {
                        list.Add(song);
                    }
                }
            }

            list.Sort(Song.Comparison);
            perfects.AddRange(list);

            return perfects;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}