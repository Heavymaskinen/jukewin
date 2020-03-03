using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class SongUpdate
    {
        public SongUpdate(Song source)
        {
            SongSource = source;
        }

        public Song SongSource { get; set; }
        public string NewAlbum { get; set; }
        public string NewArtist { get; set; }
        public string NewName { get; set; }
        public string NewTrackNo { get; set; }

    }
}
