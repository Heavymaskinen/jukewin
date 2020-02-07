using System;
using DataModel;

namespace Juke.IO
{
    [Serializable]
    public class PersistedSong
    {
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string TrackNo { get; set; }
        public string FilePath { get; set; }

        public PersistedSong() { }

        public PersistedSong(Song song)
        {
            Name = song.Name;
            Artist = song.Artist;
            Album = song.Album;
            TrackNo = song.TrackNo;
            FilePath = song.FilePath;
        }

        public Song ToSong()
        {
            return new Song(Artist, Album, Name, TrackNo, FilePath);
        }
    }
}
