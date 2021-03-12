using System;
using Juke.Control;

namespace DataModel
{
    public class Song : IComparable
    {
        public const string ALL_ARTISTS = "<All>";
        public const string ALL_ALBUMS = "<All>";
        private static Comparison<Song> comparison;

        public string Name { get; private set; }
        public string Artist { get; private set; }
        public string Album { get; private set; }
        public string TrackNo { get; private set; }
        public string FilePath { get; private set; }

        public string ID => string.IsNullOrEmpty(FilePath) ? Name + Album + Artist + TrackNo : FilePath;

        public void Merge(Song other)
        {
            if (other.ID == ID)
            {
                Name = Name.Contains(".mp3") || (Name.StartsWith("0") && !string.IsNullOrEmpty(other.Name))
                    ? other.Name
                    : Name;
                Artist = Artist == "<unknown>" ? other.Artist : Artist;
                Album = Album == "<unknown>" ? other.Album : Album;
                TrackNo = string.IsNullOrEmpty(TrackNo) ? other.TrackNo : TrackNo;

                Messenger.Log("Merged to" + Name + " - " + Artist + " " + Album);
            }
            else
            {
                Messenger.Log("Merged with different ID?");
            }
        }

        public static Comparison<Song> Comparison
        {
            get
            {
                if (comparison != null)
                {
                    return comparison;
                }

                comparison = new Comparison<Song>((a, b) =>
                {
                    var result = a.Artist.CompareTo(b.Artist);
                    if (result != 0)
                    {
                        return result;
                    }

                    result = a.Album.CompareTo(b.Album);
                    if (result != 0)
                    {
                        return result;
                    }

                    if (a.TrackNo != b.TrackNo)
                    {
                        if (!string.IsNullOrEmpty(a.TrackNo) && !string.IsNullOrEmpty(b.TrackNo))
                        {
                            return int.Parse(a.TrackNo).CompareTo(int.Parse(b.TrackNo));
                        }
                    }

                    return String.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
                });

                return comparison;
            }
        }

        public Song() : this("", "", "")
        {
        }

        public Song(string artist, string album, string name)
        {
            Name = string.IsNullOrEmpty(name) ? "<unknown>" : name;
            Artist = string.IsNullOrEmpty(artist) ? "<unknown>" : artist;
            Album = string.IsNullOrEmpty(album) ? "<unknown>" : album;
            FilePath = "";
            TrackNo = "";
        }

        public Song(string artist, string album, string name, string trackNo, string filePath) : this(artist, album,
            name)
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
                return Name.Equals(other.Name) && Artist.Equals(other.Artist) && Album.Equals(other.Album) &&
                       FilePath.Equals(other.FilePath);
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