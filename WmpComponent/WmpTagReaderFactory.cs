using Juke.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace Juke.External.Wmp
{
    public class WmpTagReaderFactory : TagReaderFactory
    {
        public TagReaderFactory BackupFactory { get; set; }

        public TagReader Create(string filename)
        {
            return new WmpTagReader(filename);
        }
    }

    class WmpTagReader : TagReader
    {
        public WmpTagReader(string filename)
        {
            var player = new WindowsMediaPlayer();
            var media = player.newMedia(filename);
            player.close();

            title = media.getItemInfo("Title");
            album = media.getItemInfo("Album");
            artist = media.getItemInfo("Author");
            trackNo = media.getItemInfo("OriginalIndex");
        }

        public void Dispose()
        {
        }

        private string title;
        private string album;
        private string artist;
        private string trackNo;

        public string Title => title;

        public string Album => album;

        public string Artist => artist;

        public string TrackNo => trackNo;

        public override string ToString()
        {
            return TrackNo+Title + Album + Artist;
        }
    }
}
