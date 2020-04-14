using Juke.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace Juke.External.Wmp
{
    class TaglibTagReaderFactory : TagReaderFactory
    {
        public TagReader Create(string filename)
        {
            return new TaglibTagReader(filename);
        }
    }

    internal class TaglibTagReader : TagReader
    {
        private Tag tag;
        private string filename;
        internal TaglibTagReader(string filename)
        {
            tag = File.Create(filename).Tag;
            this.filename = filename;
        }
        public string Title
        {
            get
            {
                return tag.Title != null ? tag.Title : FindFileName();
            }
        }

        private string FindFileName()
        {
            var parts = filename.Split('/','\\');
            var replacement = parts[parts.Length - 1];
            return replacement;
        }

        public string Album => tag.Album;

        public string Artist => tag.FirstPerformer;

        public string TrackNo => tag.Track.ToString();
    }
}
