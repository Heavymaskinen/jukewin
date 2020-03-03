using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Song : IComparable
    {
        public const string ALL_ARTISTS = "<All>";
        public const string ALL_ALBUMS = "<All>";

        public string Name { get; private set; }
        public string Artist { get; private set; }
        public string Album { get; private set; }
        public string TrackNo { get; private set; }
        public string FilePath { get; private set; }

        public Song() : this("", "", "")
        {
        }

        public Song(string artist, string album, string name)
        {
            Name = name;
            Artist = artist;
            Album = album;
            FilePath = "";
            TrackNo = "";
        }

        public Song(string artist, string album, string name, string trackNo, string filePath) : this(artist, album, name)
        {
            TrackNo = trackNo;
            FilePath = filePath;
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Song)
            {
                var other = obj as Song;
                return Name.Equals(other.Name) && Artist.Equals(other.Artist) && FilePath.Equals(other.FilePath);
            }

            return base.Equals(obj);
        }

        public int CompareTo(object obj)
        {
            if (obj is Song)
            {
                var otherSong = (obj as Song);
                if (TrackNo == otherSong.TrackNo)
                {
                    return Name.CompareTo(otherSong.Name);
                }

                return TrackNo.CompareTo(otherSong.TrackNo);
            }

            return obj.ToString().CompareTo(ToString());
        }

       
    }
}
