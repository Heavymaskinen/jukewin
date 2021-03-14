namespace DataModel
{
    public class SongUpdate
    {
        public SongUpdate(Song source)
        {
            SongSource = source;
            NewAlbum = source.Album;
            NewArtist = source.Artist;
            NewName = source.Name;
        }

        public Song SongSource { get; }
        public string NewAlbum { get; set; }
        public string NewArtist { get; set; }
        public string NewName { get; set; }
        public string NewTrackNo { get; set; }

        public Song ToSong()
        {
            return new Song(NewArtist ?? SongSource.Artist, NewAlbum ?? SongSource.Album, NewName ?? SongSource.Name,
                NewTrackNo ?? SongSource.TrackNo, SongSource.FilePath ?? SongSource.FilePath);
        }

        public SongUpdate CloneWith(Song song)
        {
            NewArtist = NewArtist ?? "<unknown>";
            NewAlbum = NewAlbum ?? "<unknown>";
            NewName = NewName ?? "<unknown>";
            return new SongUpdate(song)
            {
                NewArtist = NewArtist != "<unknown>" ? NewArtist : song.Artist,
                NewAlbum = NewAlbum != "<unknown>" ? NewAlbum : song.Album, NewName = NewName != "<unknown>" ? NewName : song.Name,
                NewTrackNo = string.IsNullOrEmpty(NewTrackNo) ? NewTrackNo : song.TrackNo
            };
        }

        public override string ToString()
        {
            return NewArtist + " " + NewAlbum + " " + NewTrackNo + " " + NewName;
        }
    }
}