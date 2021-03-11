using System.Collections.Generic;
using System.Linq;
using DataModel;

namespace Juke.IO
{
    public class SongConverter
    {
        public IList<PersistedSong> ConvertSongs(IList<Song> songs)
        {
            return songs.Select(song => new PersistedSong(song)).ToList();
        }

        public IList<Song> ConvertPersistedSongs(IList<PersistedSong> songs)
        {
            var converted = new List<Song>();
            foreach (var ps in songs)
            {
                converted.Add(ps.ToSong());
            }

            return converted;
        }
    }
}