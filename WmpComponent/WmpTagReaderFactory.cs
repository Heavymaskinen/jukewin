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
        public TagReader Create(string filename)
        {
            return new WmpTagReader(filename);
        }
    }

    class WmpTagReader : TagReader
    {
        private IWMPMedia media;

        public WmpTagReader(string filename)
        {
            var player = new WindowsMediaPlayer();
            media = player.newMedia(filename);
            player.close();
        }

        public void Dispose()
        {
        }

        public string Title => media.getItemInfo("Title");

        public string Album => media.getItemInfo("Album");

        public string Artist => media.getItemInfo("Author");

        public string TrackNo => media.getItemInfo("OriginalIndex");

        public override string ToString()
        {
            return TrackNo+Title + Album + Artist;
        }
    }
}
